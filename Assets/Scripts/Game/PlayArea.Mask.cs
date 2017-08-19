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

            // --- Properties -------------------------------------------------------------------------------
            /// <summary> Shorthand to access mask data </summary>
            public byte this[int x, int y]
            {
                get { return data[y * imageWidth + x]; }
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
                // copy old path, leaving everything else cleared
                for (int i = 0; i < imageWidth * imageHeight; i++)
                    data[i] = (data[i] == Safe || data[i] == Cut) ? Safe : Cleared;

                // second pass:
                // fill area from boss into "shadowed"
                Fill(bossX, bossY, Shadowed);

                // third pass:
                // find invalid paths and erase them
                clearedShadowArea = 0;
                for (int i = 1; i < imageWidth - 1; i++)
                {
                    for (int j = 1; j < imageHeight - 1; j++)
                    {
                        int idx = j * imageWidth + i;

                        if (data[idx] == Safe)
                        {
                            // a safe path is invalid if it's
                            // between two clear pixels
                            bool invalid = false;
                            invalid = invalid || this[i + 1, j] + this[i - 1, j] == Cleared;
                            invalid = invalid || this[i, j + 1] + this[i, j - 1] == Cleared;
                            invalid = invalid || this[i + 1, j + 1] + this[i - 1, j - 1] +
                                                 this[i + 1, j - 1] + this[i - 1, j + 1] == Cleared;

                            if (invalid)
                                data[idx] = Cleared;
                        }

                        // only count pixels in the
                        // shadow image that have been cleared
                        if (data[idx] == Cleared)
                            clearedShadowArea += shadowImage[idx];
                    }
                }
                clearedShadowArea /= 255;

                // fourth pass:
                // do the borders. We assume that the area
                // ouside the border is cleared. So we apply
                // the same logic. No path between cleared pixels
                // but path between shadow and cleared
                for (int i = 1; i < imageWidth - 1; i++)
                {
                    bool isSafe = false;
                    isSafe = this[i - 1, 1] == Shadowed;
                    isSafe = isSafe || this[i, 1] == Shadowed;
                    isSafe = isSafe || this[i + 1, 1] == Shadowed;
                    this[i, 0] = isSafe ? Safe : Cleared;

                    isSafe = this[i - 1, imageHeight - 2] == Shadowed;
                    isSafe = isSafe || this[i, imageHeight - 2] == Shadowed;
                    isSafe = isSafe || this[i + 1, imageHeight - 2] == Shadowed;
                    this[i, imageHeight - 1] = isSafe ? Safe : Cleared;
                }
                for (int i = 1; i < imageHeight - 1; i++)
                {
                    bool isSafe = false;
                    isSafe = this[1, i - 1] == Shadowed;
                    isSafe = isSafe || this[1, i] == Shadowed;
                    isSafe = isSafe || this[1, i + 1] == Shadowed;
                    this[0, i] = isSafe ? Safe : Cleared;

                    isSafe = this[imageWidth - 2, i - 1] == Shadowed;
                    isSafe = isSafe || this[imageWidth - 2, i] == Shadowed;
                    isSafe = isSafe || this[imageWidth - 2, i + 1] == Shadowed;
                    this[imageWidth - 1, i] = isSafe ? Safe : Cleared;
                }

                // pass 5: pass 4 doesn't take in the corners.
                // so we do them here
                this[0, 0] = this[1, 1] == Shadowed ? Safe : Cleared;
                this[imageWidth - 1, 0] = this[imageWidth - 2, 1] == Shadowed ? Safe : Cleared;
                this[0, imageHeight - 1] = this[1, imageHeight - 2] == Shadowed ? Safe : Cleared;
                this[imageWidth - 1, imageHeight - 1] = this[imageWidth - 2, imageHeight - 2] == Shadowed ? Safe : Cleared;

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