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

        /// <summary> Play area we play the effects on </summary>
        PlayArea playArea;


        float prevScore;
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the effects for the new round
        /// You MUST call this after creating the initial square
        /// </summary>
        public void Setup()
        {
            playArea = GetComponentInParent<PlayArea>();

            // assert that the safe path has been initialzied
            Debug.Assert(playArea.safePath.points.Length > 0);

            // save the initial square path and score
            prevPath = playArea.safePath.points.Clone() as Vector3[];
            prevScore = Controller.instance.score;

            // callback to update the cleared area effects after the safe path is redrawn
            playArea.safePath.pathRedrawn += OnSafePathRedrawn;

            // callback to display score effects on minions' deaths
            playArea.boss.minionKilled += OnMinionKilled;
        }

        // -----------------------------------------------------------------------------------	
        private void OnMinionKilled(Enemy enemy, bool killedByPlayer)
        {
            if (!killedByPlayer)
                return;

            TextMeshEffect tme = Instantiate(textMeshEffect, transform, true);
            tme.gameObject.SetActive(true);
            tme.ShowScore(enemy.score, enemy.transform.position);
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
            centroid = playArea.safePath.transform.TransformPoint(centroid);
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