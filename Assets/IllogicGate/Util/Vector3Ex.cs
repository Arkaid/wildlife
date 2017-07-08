using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Unity Vector3 extensions
    /// </summary>
    public static class Vector3Ex
    {
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns the signed angle between two vectors, using an axis as a rotation reference
        /// </summary>
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            float angle = Vector3.Angle(from, to);
            angle *= Mathf.Sign(Vector3.Dot(Vector3.Cross(from, to), axis));
            return angle;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns a vector in which each component is the absolute value of the
        /// argument's components
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Abs(Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }
        
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Per-component version of Math.Approximately
        /// </summary>
        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return 
                Mathf.Approximately(a.x, b.x) && 
                Mathf.Approximately(a.y, b.y) &&
                Mathf.Approximately(a.z, b.z);
        }

        // -----------------------------------------------------------------------------------
        public static JSONObject ToJSON(this Vector3 vector)
        {
            return new JSONObject(string.Format("[{0},{1},{2}]", vector.x, vector.y, vector.z));
        }

        // -----------------------------------------------------------------------------------
        public static Vector3 GetVector(this JSONObject json)
        {
            return new Vector3(json[0].f, json[1].f, json[2].f);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random vector of length 1 that is orthogonal to this one
        /// </summary>
        public static Vector3 RandomOrthogonal(this Vector3 vector)
        {
            Vector3 cross = Vector3.zero;

            do
            {
                // get any random vector
                Vector3 random = Random.insideUnitSphere;

                // the cross product between the two is orthogonal (perpendicular)
                cross = Vector3.Cross(random, vector);

            } while (Mathf.Approximately(cross.sqrMagnitude, 0)); // oops, random was parallel to vector

            return cross.normalized;
        }
    }

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Unity's Quaternion extesions
    /// </summary>
    public static class QuaternionEx
    {
        // -----------------------------------------------------------------------------------
        public static JSONObject ToJSON(this Quaternion q)
        {
            return new JSONObject(string.Format("[{0},{1},{2},{3}]", q.x, q.y, q.z, q.w));
        }

        // -----------------------------------------------------------------------------------
        public static Quaternion GetQuaternion(this JSONObject json)
        {
            return new Quaternion(json[0].f, json[1].f, json[2].f, json[3].f);
        }
    }
}