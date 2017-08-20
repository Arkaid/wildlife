using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    public partial class PlayArea : MonoBehaviour
    {
        // --- Class Declaration ------------------------------------------------------------------------
        public class Mask
        {
            // --- Events -----------------------------------------------------------------------------------
            public event System.Action maskCleared;

            // --- Constants --------------------------------------------------------------------------------
            // --- Static Properties ------------------------------------------------------------------------
            // --- Static Methods ---------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------
            // --- Properties -------------------------------------------------------------------------------
            int imageWidth;
            int imageHeight;

            // if set to true, this[,] will return "Cleared" when asking for out of bounds areas
            bool outOfBoundsAsCleared = false;

            // --- Properties -------------------------------------------------------------------------------
            /// <summary> Shorthand to access mask data </summary>
            public byte this[int x, int y]
            {
                get
                {
                    if (outOfBoundsAsCleared)
                    {
                        if (x < 0 || x >= imageWidth || y < 0 || y >= imageHeight)
                            return Cleared;
                    }
                    return data[y * imageWidth + x];
                }
                set { data[y * imageWidth + x] = value; }
            }

            /// <summary> Mask data </summary>
            byte[] data;

            /// <summary> Texture </summary>
            public Texture2D texture { get; private set; }

            /// <summary> Number of pixels maked as white (255) in the shadow area </summary>
            int totalShadowArea = 0;

            /// <summary> Number of pixels that have been cleared </summary>
            int clearedShadowArea = 0;

            /// <summary> Cleared ratio, between 0 and 1 </summary>
            public float clearedRatio { get { return (float)clearedShadowArea / totalShadowArea; } }

            /// <summary> Shadow image. Must be grayscale </summary>
            byte[] shadowImage;
            // --- MonoBehaviour ----------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------

            // --- Methods ----------------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            public Mask(Texture2D shadowImage)
            {
                imageWidth = PlayArea.imageWidth;
                imageHeight = PlayArea.imageHeight;

                texture = new Texture2D(imageWidth, imageHeight, TextureFormat.Alpha8, false);
                texture.filterMode = FilterMode.Point;
                this.shadowImage = shadowImage.GetRawTextureData();

                data = new byte[imageWidth * imageHeight];
                for (int i = 0; i < imageWidth * imageHeight; i++)
                {
                    data[i] = Shadowed;
                    totalShadowArea += this.shadowImage[i];
                }
                totalShadowArea /= 255;
            }

            // -----------------------------------------------------------------------------------
            /// <summary>
            /// Applies changes to the mask into the texture
            /// </summary>
            public void Apply()
            {
                texture.LoadRawTextureData(data);
                texture.Apply();
            }

            // -----------------------------------------------------------------------------------
            /// <summary>
            /// Clears any areas that are surrounded by path, leaving the boss outside of it
            /// </summary>
            public void Clear(int bossX, int bossY)
            {
                // first pass:
                // clear everything, but leave the path
                for (int i = 0; i < imageWidth * imageHeight; i++)
                    data[i] = (data[i] == Safe || data[i] == Cut) ? Safe : Cleared;

                // second pass:
                // fill area from boss into "shadowed"
                Fill(bossX, bossY, Shadowed);

                // third pass:
                // find the edges between shadow and clear,
                // turn them into safe paths
                outOfBoundsAsCleared = true;
                clearedShadowArea = 0;
                for (int i = 0; i < imageWidth; i++)
                {
                    for (int j = 0; j < imageHeight; j++)
                    {
                        int idx = j * imageWidth + i;

                        // NOTE: we consider the outer edges as "Safe"
                        if (i == 0 || i == imageWidth - 1 || j == 0 || j == imageHeight - 1)
                            data[idx] = Safe;

                        // can only turn shadow into path
                        if (data[idx] != Safe)
                        {
                            // count pixels in the shadow that have been cleared
                            if (data[idx] == Cleared)
                                clearedShadowArea += shadowImage[idx] * data[idx];
                            continue;
                        }

                        // If any combination of opposite sides is cleared, so is the center
                        bool isCleared = this[i - 1, j - 1] + this[i + 1, j + 1] 
                                       + this[i + 1, j - 1] + this[i - 1, j + 1] == Cleared;
                        isCleared = isCleared || this[i - 1, j] + this[i + 1, j] == Cleared;
                        isCleared = isCleared || this[i, j - 1] + this[i, j + 1] == Cleared;
                        this[i, j] = isCleared ? Cleared : Safe;
                    }

                }
                outOfBoundsAsCleared = false;

                if (maskCleared != null)
                    maskCleared();
            }

            // -----------------------------------------------------------------------------------
            /// <summary>
            /// Scanline floodfill
            /// </summary>
            void Fill(int x, int y, byte value)
            {
                Stack<Point> stack = new Stack<Point>();
                stack.Push(new Point(x, y));
                byte refColor = this[x, y];

                while (stack.Count > 0)
                {
                    Point eval = stack.Pop();
                    x = eval.x;
                    y = eval.y;

                    int xx = x;
                    bool pushUp = true; // if true, push the "up" pixel into the queue
                    bool pushDw = true; // if true, push the "down" pixel into the queue

                    // rewind to the beginning of the scanline (or color changes)
                    while (xx >= 0 && this[xx, y] == refColor)
                        xx--;
                    xx++;

                    // fill until the end of the scanline (or color changes)
                    while (xx < imageWidth && this[xx, y] == refColor)
                    {
                        this[xx, y] = value;
                        if (y > 0)
                        {
                            // can the line below be filled?
                            if (pushDw && this[xx, y - 1] == refColor)
                            {
                                stack.Push(new Point(xx, y - 1));
                                pushDw = false; // stop checking until the color below changes
                            }
                            // did the color below changed?
                            else if (!pushDw && this[xx, y - 1] != refColor)
                                pushDw = true; // start checking again
                        }

                        if (y < imageHeight - 1)
                        {
                            // can the line above be filled?
                            if (pushUp && this[xx, y + 1] == refColor)
                            {
                                stack.Push(new Point(xx, y + 1));
                                pushUp = false; // stop checking until the color above changes
                            }
                            // did the color above changed?
                            else if (!pushUp && this[xx, y + 1] != refColor)
                                pushUp = true; // start checking again
                        }
                        xx++;
                    }
                }
            }
        }
    }
}