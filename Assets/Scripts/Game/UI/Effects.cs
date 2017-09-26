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

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows the "+X,XXX PTS" effect. position is in world coordinates
        /// </summary>
        public void ShowScore(int score, Vector3 position)
        {
            TextMeshEffect tme = Instantiate(textMeshEffect, transform, true);
            tme.gameObject.SetActive(true);
            tme.Show(string.Format("+{0:###,###,##0}<size=10>    </size>PTS", score), position);
        }
    }
}