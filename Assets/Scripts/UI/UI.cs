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
        Text livesText = null;

        [SerializeField]
        TimeDisplay _timeDisplay = null;
        public TimeDisplay timeDisplay { get { return _timeDisplay; } }

        [SerializeField]
        PercentageBar _percentageBar = null;
        public PercentageBar percentageBar { get { return _percentageBar; } }

        [SerializeField]
        RoundStart _roundStart = null;
        public RoundStart roundStart { get { return _roundStart; } }

        [SerializeField]
        GameResult gameResult = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Total lives to show </summary>
        public int lives { set { livesText.text = string.Format("x{0:00}", value); } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Update()
        {
            UpdateControlAlpha();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the UI to the beginning of the round
        /// </summary>
        /// <param name="clearPercentage"> percentage needed to clear the round </param>
        /// <param name="livesLeft"> lives left in the round </param>
        public void Reset(int livesLeft, int clearPercentage, int roundTime)
        {
            gameObject.SetActive(true);

            lives = livesLeft;

            timeDisplay.Reset(roundTime);
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
                /*
                // blend the text
                Color col = timeText.color;
                col.a = Mathf.Lerp(MinAlpha, 1, t);
                timeText.color = col;

                // and the time bars
                col = timeBarLeft.color;
                col.a = Mathf.Lerp(MinAlpha, 1, t);
                timeBarLeft.color = col;
                timeBarRight.color = col;
                */
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
    }
}