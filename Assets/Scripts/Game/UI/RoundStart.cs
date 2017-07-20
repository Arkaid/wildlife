using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundStart : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        RectTransform [] rounds = null;

        [SerializeField]
        new Animation animation = null;

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
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator Show(int round)
        {
            Debug.Assert(round >= 0 && round <= 2);

            gameObject.SetActive(true);
            for (int i = 0; i < rounds.Length; i++)
                rounds[i].gameObject.SetActive(i == round);

            animation.Play();
            while (animation.isPlaying)
                yield return null;

            gameObject.SetActive(false);
        }
    }
}