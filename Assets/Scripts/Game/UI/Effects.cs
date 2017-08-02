using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Handles effects that play on the play area, rather than the 2D canvas UI
    /// </summary>
    public class Effects : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        TextMeshEffect textMeshEffect;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Used to take the differences of paths between mask clearings </summary>
        Vector3[] prevPath;

        /// <summary> Pointer to currently active safe path instance </summary>
        PathRenderer safePath;

        float prevScore;
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the ee for the new round
        /// </summary>
        /// <param name="safePath">Instance of the safe path AFTER creating initial square</param>
        public void Setup(PathRenderer safePath)
        {
            // save the initial square path and score
            prevPath = safePath.points.Clone() as Vector3[];
            prevScore = Controller.instance.score;

            // callback to update the cleared area effects after the safe path is redrawn
            safePath.pathRedrawn += OnSafePathRedrawn;
            this.safePath = safePath;
        }

        // -----------------------------------------------------------------------------------	
        void OnSafePathRedrawn(Vector3[] newPath)
        {
            Vector3[] result;
            Vector3 centroid;

            // find the difference between the previous path and the new one
            Util.FindDifference(prevPath, newPath, out result, out centroid);

            // save the new path to check difference on the next one
            prevPath = newPath.Clone() as Vector3[];

            // Create a text with the score for the cleared section
            int diffScore = Mathf.FloorToInt(Controller.instance.score - prevScore);
            prevScore = Controller.instance.score;

            // we need the centroid in world coordinates
            centroid = safePath.transform.TransformPoint(centroid);
            /*
            List<Vector3> tmp = new List<Vector3>(result);
            tmp.Add(result[0]);
            tmp = tmp.ConvertAll((pt) => { return safePath.transform.TransformPoint(pt); });
            for (int i = 0; i < result.Length; i++)
                Debug.DrawLine(tmp[i], tmp[i+1], Color.cyan, 5);
            Debug.DrawLine(centroid, centroid + Vector3.up * 100, Color.red, 5);
            */

            TextMeshEffect tme = Instantiate(textMeshEffect, transform, true);
            tme.gameObject.SetActive(true);
            tme.ShowScore(diffScore, centroid);
        }
    }
}