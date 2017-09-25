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

        [SerializeField]
        Text musicText = null;

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
            musicText.text = "";
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator Show(int round, string musicCredits)
        {
            Debug.Assert(round >= 0 && round < rounds.Length);

            musicText.text = "œ " + musicCredits.ToUpper() + " œ";

            gameObject.SetActive(true);
            for (int i = 0; i < rounds.Length; i++)
                rounds[i].gameObject.SetActive(i == round);

            animation.Play();
            while (animation.isPlaying)
                yield return null;
        }

        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            StartCoroutine(HideCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator HideCoroutine()
        {
            animation.Play("music_credits_out");
            while (animation.isPlaying)
                yield return null;

            gameObject.SetActive(false);
        }
    }
}