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
        Player player = null;

        [SerializeField]
        Text percentageText = null;

        [SerializeField]
        Text timeText = null;

        [SerializeField]
        Image timeBarLeft = null;

        [SerializeField]
        Image timeBarRight = null;

        [SerializeField]
        Text livesText = null;

        [SerializeField]
        PercentageBar _percentageBar = null;
        public PercentageBar percentageBar { get { return _percentageBar; } }

        [SerializeField]
        RoundStart _roundStart = null;
        public RoundStart roundStart { get { return _roundStart; } }

        [SerializeField]
        GameResult gameResult = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Total time, in seconds (0~999)  </summary>
        public float totalTime { set { _totalTime = Mathf.Clamp(value, 0, 999); } }
        float _totalTime = 999;

        /// <summary> Current time, in seconds (0~999)  </summary>
        public float time { set { SetTime(value); } }
        private float _time = 999;

        /// <summary> Used to keep track of when the width changed to update bar size </summary>
        private int lastWidth = -1;

        /// <summary> Total lives to show </summary>
        public int lives { set { livesText.text = string.Format("x{0:00}", value); } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Update()
        {
            if (Screen.width != lastWidth)
            {
                lastWidth = Screen.width;
                UpdateBarWidth();
            }
            UpdateControlAlpha();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the UI to the beginning of the round
        /// </summary>
        /// <param name="clearPercentage"> percentage needed to clear the round </param>
        /// <param name="livesLeft"> lives left in the round </param>
        public void Reset(int livesLeft, int clearPercentage)
        {
            gameObject.SetActive(true);

            lives = livesLeft;

            percentageBar.Reset(clearPercentage);
            roundStart.Reset();
            gameResult.Reset();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Hides the UI completely
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Play the game result
        /// </summary>
        public void PlayResult(bool cleared)
        {
            StartCoroutine(PlayResultCoroutine(cleared));
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator PlayResultCoroutine(bool cleared)
        {
            // hide the top / down UI parts

            if (cleared)
            {
                yield return StartCoroutine(gameResult.PlayCleared());
                Hide();
            }
            else
                yield return StartCoroutine(gameResult.PlayGameOver());
        }

        // -----------------------------------------------------------------------------------	
        void SetTime(float value)
        {
            _time = Mathf.Clamp(value, 0, 999);

            int i = Mathf.FloorToInt(_time);
            int j = Mathf.FloorToInt((_time - i) * 100);
            timeText.text = string.Format("{0:000}<size=12>.{1:00}</size>", i, j);

            UpdateBarWidth();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Controls the fading of the top and bottom UI elements when the player gets close to them
        /// </summary>
        void UpdateControlAlpha()
        {
            const float BottomFadeStart = 0.15f;
            const float BottomFadeEnd = 0.025f;
            const float TopFadeStart = 0.90f;
            const float TopFadeEnd = 0.95f;
            const float MinAlpha = 0.2f;

            Camera cam = Camera.main;
            Vector2 playerVP = cam.WorldToViewportPoint(player.transform.position);

            // fade in/out the bottom UI
            if (playerVP.y < BottomFadeStart)
            {
                // make sure t doesn't overshoot
                float t = Mathf.Max(playerVP.y, BottomFadeEnd);

                // set it between 0 and 1
                t = (t - BottomFadeEnd) / (BottomFadeStart - BottomFadeEnd);

                // blend the text
                Color col = timeText.color;
                col.a = Mathf.Lerp(MinAlpha, 1, t);
                timeText.color = col;

                // and the time bars
                col = timeBarLeft.color;
                col.a = Mathf.Lerp(MinAlpha, 1, t);
                timeBarLeft.color = col;
                timeBarRight.color = col;
            }

            // fade in/out the top UI
            else if (playerVP.y > TopFadeStart)
            {
                float t = Mathf.Min(playerVP.y, TopFadeEnd);
                t = (TopFadeEnd - t) / (TopFadeEnd - TopFadeStart);

                Color col = percentageText.color;
                col.a = Mathf.Lerp(MinAlpha, 1, t);
                percentageText.color = col;
            }

        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Updates the width of the time bar when and if the screen is resized. 
        /// Also when time changes
        /// </summary>
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