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

            // save the initial score
            prevScore = Controller.instance.score;

            // callback to update the cleared area effects after the safe path is redrawn
            playArea.mask.maskCleared += OnMaskCleared;

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
        void OnMaskCleared(Point center)
        {
            // Create a text with the score for the cleared section
            int diffScore = Mathf.FloorToInt(Controller.instance.score - prevScore);
            prevScore = Controller.instance.score;

            // we need the centroid in world coordinates
            Vector3 worldCenter = playArea.MaskPositionToWorld(center);

            // play effect
            TextMeshEffect tme = Instantiate(textMeshEffect, transform, true);
            tme.gameObject.SetActive(true);
            tme.ShowScore(diffScore, worldCenter);
        }
    }
}