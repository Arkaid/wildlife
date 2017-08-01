using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillBar : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image fillImage;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Maximum available time you can possible use the skill for </summary>
        float maxTime;

        /// <summary> Amount of remaining time you can use the skill </summary>
        public float remainingTime { set { SetRemainingTime(value); } }
        
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the UI to the beginning of the round
        /// </summary>
        public void Reset(float maxTime, float remainingTime)
        {
            this.maxTime = maxTime;
            this.remainingTime = remainingTime;
        }

        // -----------------------------------------------------------------------------------	
        void SetRemainingTime(float remainingTime)
        {
            float t = Mathf.Clamp01(remainingTime / maxTime);
            fillImage.rectTransform.anchorMax = new Vector2(t, 1);
        }
    }
}