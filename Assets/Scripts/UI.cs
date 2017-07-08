using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class UI : IllogicGate.SingletonBehaviour<UI>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Text percentageText = null;

        [SerializeField]
        Text timeText = null;

        [SerializeField]
        Image timeBarLeft = null;

        [SerializeField]
        Image timeBarRight = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Percentage to display (0 ~ 100) </summary>
        public float percentage
        {
            set
            {
                float clamped = Mathf.Clamp(value, 0, 100);
                int i = Mathf.FloorToInt(clamped);
                int j = Mathf.FloorToInt((clamped - i) * 100);
                percentageText.text = string.Format("{0:00}<size=12>.{1:00}</size>%", i, j);
            }
        }

        /// <summary> Total time, in seconds (0~999)  </summary>
        public float totalTime
        {
            set { _totalTime = Mathf.Clamp(value, 0, 999); }
        }
        float _totalTime = 999;


        /// <summary> Current time, in seconds (0~999)  </summary>
        public float time
        {
            set
            {
                _time = Mathf.Clamp(value, 0, 999);

                int i = Mathf.FloorToInt(_time);
                int j = Mathf.FloorToInt((_time - i) * 100);
                timeText.text = string.Format("{0:000}<size=12>.{1:00}</size>", i, j);

                UpdateBarWidth();
            }
        }
        private float _time = 999;

        private int lastWidth = -1;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Update()
        {
            if (Screen.width != lastWidth)
            {
                lastWidth = Screen.width;
                UpdateBarWidth();
            }
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void UpdateBarWidth()
        {
            float t = _time / _totalTime;

            float x = timeBarRight.rectTransform.anchoredPosition.x;
            Vector2 size = timeBarRight.rectTransform.sizeDelta;
            size.x = (Screen.width * 0.5f - x - 10) * t;
            timeBarRight.rectTransform.sizeDelta = size;
            timeBarLeft.rectTransform.sizeDelta = size;
        }
    }
}