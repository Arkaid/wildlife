using UnityEngine;

namespace IllogicGate
{
    // Source for the easing fuctions:
    // http://robertpenner.com/easing/

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple Tweener Interpolators
    /// </summary>
    public abstract class Tweener
    {
        // --- Constant Declaration ---------------------------------------------------------------------
        public enum Mode
        {
            Quad,
            Cubic,
            Sine,
            Expo,
            Elastic,
            Back,
            Circ,
            Bounce,
        }

        // --- Public Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Ease in interpolator
        /// </summary>
        /// <param name="t"> parameter value, between 0 and 1 </param>
        /// <param name="mode"> interpolation mode </param>
        /// <returns> an adjusted t value, depending on the interpolation </returns>
        /// <example>
        /// float x = Math.Lerp(start, end, Cave.Tweener.EaseIn(t));
        /// </example>
        public static float EaseIn(float t, Mode mode = Mode.Cubic)
        {
            switch (mode)
            {
                case Mode.Quad:
                    return t * t;
                case Mode.Cubic:
                    return t * t * t;
                case Mode.Sine:
                    return -Mathf.Cos(t * (Mathf.PI / 2)) + 1;
                case Mode.Expo:
                {
                    if (t == 0)
                        return 0;
                    t--;
                    return Mathf.Pow(2, 10 * t);
                }
                case Mode.Elastic:
                {
                    const float p = 0.3f;
                    t--;
                    return -(Mathf.Pow(2, 10 * t)) * Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p);
                }
                case Mode.Back:
                {
                    const float p = 1.70158f;
                    return t * t * ((p + 1) * t - p);
                }
                case Mode.Circ:
                    return -(Mathf.Sqrt(1 - t * t) - 1);
                case Mode.Bounce:
                    return 1 - EaseOut(1 - t, Mode.Bounce);
            }
            return 0;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Ease out interpolator
        /// </summary>
        /// <param name="t"> parameter value, between 0 and 1 </param>
        /// <param name="mode"> interpolation mode </param>
        /// <returns> an adjusted t value, depending on the interpolation </returns>
        /// <example>
        /// float x = Math.Lerp(start, end, Cave.Tweener.EaseOut(t));
        /// </example>
        public static float EaseOut(float t, Mode mode = Mode.Cubic)
        {
            switch (mode)
            {
                case Mode.Quad:
                    return t * (2 - t);
                case Mode.Cubic:
                    t--;
                    return t * t * t + 1;
                case Mode.Sine:
                    return Mathf.Sin(t * (Mathf.PI / 2));
                case Mode.Expo:
                    return -Mathf.Pow(2, -10 * t) + 1;
                case Mode.Elastic:
                    {
                        const float p = 0.3f;
                        return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - p / 4) * (2 * Mathf.PI) / p) + 1;
                    }
                case Mode.Back:
                    {
                        const float p = 1.70158f;
                        t--;
                        return (t * t * ((p + 1) * t + p) + 1);
                    }
                case Mode.Circ:
                    {
                        t--;
                        return Mathf.Sqrt(1 - t * t);
                    }
                case Mode.Bounce:
                    {
                        const float step1 = 1f / 2.75f;
                        const float step2 = 2f / 2.75f;
                        const float step3 = 2.5f / 2.75f;

                        const float p = 7.5625f;
                        const float k1 = 1.5f / 2.75f;
                        const float k2 = 2.25f / 2.75f;
                        const float k3 = 2.625f / 2.75f;
                        const float q1 = 0.75f;
                        const float q2 = 0.9375f;
                        const float q3 = 0.984375f;

                        if (t < step1)
                            return p * t * t;
                        else if (t < step2)
                        {
                            t -= k1;
                            return p * t * t + q1;
                        }
                        else if (t < step3)
                        {
                            t -= k2;
                            return p * t * t + q2;
                        }
                        else
                        {
                            t -= k3;
                            return p * t * t + q3;
                        }
                    }
            }
            return 0;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Ease in-out interpolator
        /// </summary>
        /// <param name="t"> parameter value, between 0 and 1 </param>
        /// <param name="mode"> interpolation mode </param>
        /// <returns> an adjusted t value, depending on the interpolation </returns>
        /// <example>
        /// float x = Math.Lerp(start, end, Cave.Tweener.EaseInOut(t));
        /// </example>
        public static float EaseInOut(float t, Mode mode = Mode.Cubic)
        {
            if (t <= 0.5f)
            {
                t = t * 2;
                t = EaseIn(t, mode);
                return t / 2;
            }
            else
            {
                t = (t - 0.5f) * 2;
                t = EaseOut(t, mode);
                return (t / 2) + 0.5f;
            }
        }
    }
}
