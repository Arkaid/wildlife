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
            /// <summary> Argument is centroid of the cleared area </summary>
            public event System.Action<Point> maskCleared;

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
                // create a copy for comparison
                byte[] oldData = data.Clone() as byte[];
                
                // center of the area we cleared
                Vector2 center = new Vector2();
                int diffCount = 0;

                // first pass:
                // clear everything, but leave the path
                List<int> path = new List<int>();
                for (int i = 0; i < imageWidth * imageHeight; i++)
                {
                    // we consider the borders as safe
                    int x = i % imageWidth;
                    int y = i / imageWidth;
                    bool isBorder = (x == 0 || x == imageWidth - 1 || y == 0 || y == imageHeight - 1);

                    if (isBorder || data[i] == Safe || data[i] == Cut)
                        path.Add(i);
                    else
                        data[i] = Cleared;
                }

                // second pass:
                // fill area from boss into "shadowed"
                Fill(bossX, bossY, Shadowed);

                // third pass: 
                // Run through the path. If any 8-direction pixel is shadow
                // turn it to safe, otherwise cleared
                outOfBoundsAsCleared = true;
                foreach (int i in path)
                {
                    int x = i % imageWidth;
                    int y = i / imageWidth;

                    bool hasShadow = false;
                    hasShadow = hasShadow || this[x - 1, y - 1] == Shadowed;
                    hasShadow = hasShadow || this[x - 1, y + 1] == Shadowed;
                    hasShadow = hasShadow || this[x + 1, y + 1] == Shadowed;
                    hasShadow = hasShadow || this[x + 1, y - 1] == Shadowed;
                    hasShadow = hasShadow || this[x + 1, y] == Shadowed;
                    hasShadow = hasShadow || this[x - 1, y] == Shadowed;
                    hasShadow = hasShadow || this[x, y + 1] == Shadowed;
                    hasShadow = hasShadow || this[x, y - 1] == Shadowed;

                    data[i] = hasShadow ? Safe : Cleared;
                }
                outOfBoundsAsCleared = false;

                // fourth pass:
                // count pixels and find center point of difference
                clearedShadowArea = 0;
                for (int i = 0; i < imageWidth * imageHeight; i++)
                {
                    if (oldData[i] != data[i])
                    {
                        center += new Vector2(i % imageWidth, i / imageWidth);
                        diffCount++;
                    }

                    if (data[i] == Cleared)
                        clearedShadowArea += shadowImage[i];
                }
                clearedShadowArea /= 255;

                // average the position
                if (diffCount != 0)
                    center = center / diffCount;

                // raise event
                if (maskCleared != null)
                    maskCleared(new Point(center));
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