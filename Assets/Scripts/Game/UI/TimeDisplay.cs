using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class TimeDisplay : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Text valueText;

        [SerializeField]
        Image clockImage;

        [SerializeField]
        Sprite[] sprites;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Total time, in seconds (0~999)  </summary>
        float totalTime = 999f;

        /// <summary> Current time, in seconds (0~999)  </summary>
        public float time { set { SetTime(value); } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Reset(float totalTime)
        {
            this.totalTime = totalTime;
            time = totalTime;
        }

        // -----------------------------------------------------------------------------------	
        void SetTime(float value)
        {
            value = Mathf.Clamp(value, 0, 999);

            int i = Mathf.FloorToInt(value);
            int j = Mathf.FloorToInt((value - i) * 100);
            valueText.text = string.Format("{0:000}<size=10>.{1:00}</size>", i, j);

            float idx = (1f - Mathf.Clamp01(value / totalTime)) * (sprites.Length - 1);
            clockImage.sprite = sprites[Mathf.RoundToInt(idx)];
        }

    }
}