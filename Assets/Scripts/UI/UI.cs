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
    }
}