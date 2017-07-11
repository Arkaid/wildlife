using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
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
            /// <summary> Shorthand to access mask data </summary>
            public byte this[int x, int y]
            {
                get { return data[y * ImageWidth + x]; }
                set { data[y * ImageWidth + x] = value; }
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
                texture = new Texture2D(ImageWidth, ImageHeight, TextureFormat.Alpha8, false);
                texture.filterMode = FilterMode.Point;
                this.shadowImage = shadowImage.GetRawTextureData();

                data = new byte[ImageWidth * ImageHeight];
                for (int i = 0; i < ImageWidth * ImageHeight; i++)
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
                UnityEngine.Profiling.Profiler.BeginSample("Clear_FC");

                // first pass:
                // copy old path, leaving everything else cleared
                for (int i = 0; i < ImageWidth * ImageHeight; i++)
                    data[i] = (data[i] == Safe || data[i] == Cut) ? Safe : Cleared;

                // second pass:
                // fill area from boss into "shadowed"
                UnityEngine.Profiling.Profiler.BeginSample("Fill");
                Fill(bossX, bossY, Shadowed);
                UnityEngine.Profiling.Profiler.EndSample();

                // third pass:
                // find invalid paths and erase them
                clearedShadowArea = 0;
                for (int i = 1; i < ImageWidth - 1; i++)
                {
                    for (int j = 1; j < ImageHeight - 1; j++)
                    {
                        int idx = j * ImageWidth + i;

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
                // do the borders. If they're cleared, you
                // can use them as safe paths
                for (int i = 0; i < ImageWidth; i++)
                {
                    if (this[i, 0] == Cleared)
                        this[i, 0] = Safe;
                    if (this[i, ImageHeight - 1] == Cleared)
                        this[i, ImageHeight - 1] = Safe;
                }
                for (int i = 0; i < ImageHeight; i++)
                {
                    if (this[0, i] == Cleared)
                        this[0, i] = Safe;
                    if (this[ImageWidth - 1, i] == Cleared)
                        this[ImageWidth - 1, i] = Safe;
                }

                UnityEngine.Profiling.Profiler.EndSample();

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
                    while (xx < ImageWidth && this[xx, y] == refColor)
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

                        if (y < ImageHeight - 1)
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