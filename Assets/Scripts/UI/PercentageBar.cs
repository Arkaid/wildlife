using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class PercentageBar : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image normalImage;

        [SerializeField]
        Image clearedImage;

        [SerializeField]
        Image fillImage;

        [SerializeField]
        Text valueText;

        // --- Properties -------------------------------------------------------------------------------
        public float percentage { set { SetPercentage(value); } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the UI to the beginning of the round
        /// </summary>
        public void Reset(int clearPercentage)
        {
            // set the sizes of "normal" and "clear"
            float t = clearPercentage / 100f;
            normalImage.rectTransform.anchorMax = new Vector2(t, 1);
            clearedImage.rectTransform.anchorMin = new Vector2(t, 0);
            percentage = 0;
        }

        // -----------------------------------------------------------------------------------	
        void SetPercentage(float value)
        {
            // set the text
            value = Mathf.Clamp(value, 0, 100);
            int i = Mathf.FloorToInt(value);
            int j = Mathf.FloorToInt((value - i) * 100);
            valueText.text = string.Format("{0:00}<size=10>.{1:00}</size>%", i, j);

            // set the fill
            float t = value / 100f;
            fillImage.rectTransform.anchorMax = new Vector2(t, 1);
        }
    }
}