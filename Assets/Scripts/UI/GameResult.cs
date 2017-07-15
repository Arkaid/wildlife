using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class GameResult : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Animation gameOver;

        [SerializeField]
        Animation cleared;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resets the UI to the beginning of the round
        /// </summary>
        public void Reset()
        {
            gameOver.gameObject.SetActive(false);
            cleared.gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator PlayGameOver()
        {
            gameOver.gameObject.SetActive(true);
            gameOver.Play();

            while (gameOver.isPlaying)
                yield return null;
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator PlayCleared()
        {
            cleared.gameObject.SetActive(true);
            cleared.Play();

            while (cleared.isPlaying)
                yield return null;
        }

    }
}