using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
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
        public IEnumerator Show(int round)
        {
            Debug.Assert(round >= 1 && round <= 3);
            round--;
            gameObject.SetActive(true);
            for (int i = 0; i < rounds.Length; i++)
                rounds[i].gameObject.SetActive(i == round);

            animation.Play();
            while (animation.isPlaying)
                yield return null;

            Hide();
        }

        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
    }
}