using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    using ClipperLib;

    // --- Class Declaration ------------------------------------------------------------------------
    public static class Util 
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Sets the screen resolution from the config settings
        /// </summary>
        public static void SetResolutionFromConfig()
        {
            int w, h;
            if (Data.Options.instance.fullScreen)
            {
                w = Screen.currentResolution.width;
                h = Screen.currentResolution.height;
            }
            else
            {
                w = (int)Data.Options.instance.resolution.x;
                h = (int)Data.Options.instance.resolution.y;
            }
            Screen.SetResolution(w, h, Data.Options.instance.fullScreen);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Formats a number of seconds in mm:ss:mili
        /// </summary>
        public static string FormatTime(float seconds)
        {
            float time = Mathf.FloorToInt(seconds * 1000f);

            int mili = Mathf.FloorToInt(time % 1000); time -= mili; time /= 1000f;
            int secs = Mathf.FloorToInt(time % 60); time -= secs; time /= 60f;
            int mins = Mathf.FloorToInt(time);
            return string.Format("{0:00}:{1:00}:{2:000}", mins, secs, mili);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Formats the score into a string
        /// </summary>
        public static string FormatScore(long score)
        {
            return string.Format("{0:##,###,###,##0}", score);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Find the difference between the safe path before and after cutting,
        /// then returning the center of that difference plus the difference path
        /// </summary>
        static public bool FindDifference(Vector3 [] pointsBefore, Vector3 [] pointsAfter, out Vector3 [] result, out Vector3 centroid)
        {
            result = null;
            centroid = Vector3.zero;

            System.Converter<Vector3, IntPoint> converter = (pt) => { return new IntPoint(pt.x, pt.y); };
            List<IntPoint> before = new List<IntPoint>(System.Array.ConvertAll(pointsBefore, converter));
            List<IntPoint> after = new List<IntPoint>(System.Array.ConvertAll(pointsAfter, converter));
            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            Clipper clipper = new Clipper();
            clipper.AddPath(before, PolyType.ptClip, true);
            clipper.AddPath(after, PolyType.ptSubject, true);
            if (!clipper.Execute(ClipType.ctDifference, solution))
                return false;

            result = System.Array.ConvertAll(solution[0].ToArray(), (pt) => { return new Vector3 ( pt.X, pt.Y, 0); });
            centroid = FindCentroid(result);
            return true;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Finds the centroid of a polygon
        /// src: https://stackoverflow.com/questions/2792443/finding-the-centroid-of-a-polygon
        /// </summary>
        public static Vector3 FindCentroid(Vector3[] points)
        {
            int vertexCount = points.Length;

            Vector2 centroid = new Vector2();
            float signedArea = 0.0f;
            float x0 = 0.0f; // Current vertex X
            float y0 = 0.0f; // Current vertex Y
            float x1 = 0.0f; // Next vertex X
            float y1 = 0.0f; // Next vertex Y
            float a = 0.0f;  // Partial signed area

            // For all vertices except last
            int i = 0;
            for (i = 0; i < vertexCount - 1; ++i)
            {
                x0 = points[i].x;
                y0 = points[i].y;
                x1 = points[i + 1].x;
                y1 = points[i + 1].y;
                a = x0 * y1 - x1 * y0;
                signedArea += a;
                centroid.x += (x0 + x1) * a;
                centroid.y += (y0 + y1) * a;
            }

            // Do last vertex separately to avoid performing an expensive
            // modulus operation in each iteration.
            x0 = points[i].x;
            y0 = points[i].y;
            x1 = points[0].x;
            y1 = points[0].y;
            a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.x += (x0 + x1) * a;
            centroid.y += (y0 + y1) * a;

            signedArea *= 0.5f;
            centroid.x /= (6.0f * signedArea);
            centroid.y /= (6.0f * signedArea);

            return centroid;
        }
        
        // --- Properties -------------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}