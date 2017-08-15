using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    public static class Util
    {
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the MD5 Checksum of a bytes array. Useful for comparing files
        /// </summary>
        static public string Md5Checksum(byte [] bytes)
        {
            // calculate hash
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++)
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');

            return hashString.PadLeft(32, '0');
        }

        // -----------------------------------------------------------------------------------
        // src: http://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/
        /// <summary>
        /// Generates a random number with a gaussian distribution, mean = 0 and stddev = 1
        /// </summary>
        public static float RandomGaussian()
        {
            float v1, v2, s;
            do
            {
                v1 = 2f * Random.Range(0f, 1f) - 1f;
                v2 = 2f * Random.Range(0f, 1f) - 1f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1f || s == 0);

            s = Mathf.Sqrt((-2f * Mathf.Log(s)) / s);
            return v1 * s;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Generates a random number with a gaussian distribution, with given mean and stddev
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stddev"></param>
        /// <returns></returns>
        public static float RandomGaussian(float mean, float stddev)
        {
            return mean + RandomGaussian() * stddev;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Finds a component in the scene. It works similar to GetComponent, but
        /// throught the whole scene. It's quite process intensive, so use lightly
        /// </summary>
        public static T GetComponentInScene<T>(bool includeInactive = false) where T : Component
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                foreach(GameObject root in scene.GetRootGameObjects())
                {
                    T component = root.GetComponentInChildren<T>(includeInactive);
                    if (component != null)
                        return component;
                }
            }
            return null;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Finds a component in the scene. It works similar to GetComponents, but
        /// throught the whole scene. It's quite process intensive, so use lightly
        /// </summary>
        public static T [] GetComponentsInScene<T>(bool includeInactive = false) where T : Component
        {
            List<T> components = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    T component = root.GetComponentInChildren<T>(includeInactive);
                    if (component != null)
                        components.Add(component);
                }
            }
            return components.ToArray();
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Copies a component by reflection (as found in Unity Anwsers)
        /// http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        /// </summary>
        /// <returns></returns>
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            System.Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        // -----------------------------------------------------------------------------------
        public static void SetLayerRecursively(GameObject go, string layer)
        {
            SetLayerRecursively(go, LayerMask.NameToLayer(layer));
        }
        
        // -----------------------------------------------------------------------------------
        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursively(child.gameObject, layer);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the object's bounds in world space from mesh renderers
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Bounds CalculateObjectBounds(GameObject go, bool includeInactive = false)
        {
            Renderer[] rends = go.GetComponentsInChildren<MeshRenderer>(includeInactive);
            if (rends.Length == 0)
                return new Bounds();
            Bounds bounds = rends[0].bounds;
            foreach (Renderer rend in rends)
                bounds.Encapsulate(rend.bounds);
            return bounds;
        }
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Finds the submesh for a given triangle. Useful when you get the triangle index on a hitpoint
        /// </summary>
        public static int FindSubmesh(MeshRenderer renderer, int triangle)
        {
            Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;

            int[] meshTriangles = mesh.triangles;
            int[] searchTri = new int[]
            {
                meshTriangles[triangle * 3],
                meshTriangles[triangle * 3 + 1],
                meshTriangles[triangle * 3 + 2],
            };

            // parse each submesh
            for (int sm = 0; sm < mesh.subMeshCount; sm++)
            {
                // check each triangle
                int[] tris = mesh.GetTriangles(sm);
                for (int i = 0; i < tris.Length; i+=3)
                {
                    if( tris[i    ] == searchTri[0] &&
                        tris[i + 1] == searchTri[1] &&
                        tris[i + 2] == searchTri[2])
                        return sm;
                }
            }
            return -1;
        }

        // -----------------------------------------------------------------------------------
        public static int FindClosestTriangle(MeshRenderer[] renderers, Vector3 point, out int submesh, out MeshRenderer renderer)
        {
            float dst;
            return FindClosestTriangle(renderers, point, out submesh, out renderer, out dst);
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Finds the triangle index from a mesh in space, to a given point in world coordinates
        /// Useful when RaycastHit is not against a mesh collider and we need to know which triangle it hits
        /// (Multiple renderers version)
        /// </summary>
        public static int FindClosestTriangle(MeshRenderer [] renderers, Vector3 point, out int submesh, out MeshRenderer renderer, out float sqrDistance)
        {
            // from all the renderers, find the closest one

            List<MeshRenderer> done = new List<MeshRenderer>();
            sqrDistance = float.MaxValue;
            submesh = -1;
            int closest = -1;
            renderer = null;
            foreach (MeshRenderer rend in renderers)
            {
                if (done.Contains(rend))
                    continue;
                done.Add(rend);

                float sqrdist;
                int sm;
                int tri = FindClosestTriangle(rend, point, out sm, out sqrdist);
                if (sqrdist < sqrDistance)
                {
                    sqrDistance = sqrdist;
                    submesh = sm;
                    closest = tri;
                    renderer = rend;
                }
            }

            return closest;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Finds the triangle index from a mesh in space, to a given point in world coordinates
        /// Useful when RaycastHit is not against a mesh collider and we need to know which triangle it hits
        /// </summary>
        public static int FindClosestTriangle(MeshRenderer renderer, Vector3 point, out int submesh, out float sqrDistance)
        {
            // transform the point to local coordinates
            point = renderer.transform.InverseTransformPoint(point);

            Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;
            Vector3[] verts = mesh.vertices;
            sqrDistance = float.MaxValue;
            int closest = -1;
            submesh = -1;

            // parse each submesh
            for (int sm = 0; sm < mesh.subMeshCount; sm++)
            {
                // check each triangle
                int[] tris = mesh.GetTriangles(sm);
                for (int i = 0; i < tris.Length;)
                {
                    Vector3 p1 = verts[tris[i++]];
                    Vector3 p2 = verts[tris[i++]];
                    Vector3 p3 = verts[tris[i++]];

                    // now check distance to each edge. Shortest wins
                    float sqrdst = PointToSegmentSqrDistance(point, p2, p1);
                    if (sqrdst < sqrDistance)
                    {
                        sqrDistance = sqrdst;
                        closest = i;
                        submesh = sm;
                        continue;
                    }
                    sqrdst = PointToSegmentSqrDistance(point, p3, p1);
                    if (sqrdst < sqrDistance)
                    {
                        sqrDistance = sqrdst;
                        closest = i;
                        submesh = sm;
                        continue;
                    }
                    sqrdst = PointToSegmentSqrDistance(point, p3, p2);
                    if (sqrdst < sqrDistance)
                    {
                        sqrDistance = sqrdst;
                        closest = i;
                        submesh = sm;
                    }
                }
            }

            return closest;
        }

        // -----------------------------------------------------------------------------------
        // src: http://geomalgorithms.com/a02-_lines.html
        //      https://web.archive.org/web/20161021055120/http://geomalgorithms.com/a02-_lines.html
        static float PointToSegmentSqrDistance(Vector3 point, Vector3 segment_a, Vector3 segment_b)
        {
            Vector3 v = segment_b - segment_a;
            Vector3 w = point - segment_a;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return w.sqrMagnitude;

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
                return (point - segment_b).sqrMagnitude;

            float b = c1 / c2;
            Vector3 pt = segment_a + v * b;
            return (point - pt).sqrMagnitude;
        }
    }
}