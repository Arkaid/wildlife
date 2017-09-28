using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class UnlockLetters : MonoBehaviour
    {
        [System.Serializable]
        struct Letter
        {
            public UNLOCK letter;
            public Image image; 
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        List<Letter> letters;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            Clear();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Clear()
        {
            foreach (Letter letter in letters)
                letter.image.enabled = false;
        }

        // -----------------------------------------------------------------------------------	
        public void ShowLetter(UNLOCK letter, bool show)
        {
            letters.Find(l => l.letter == letter).image.enabled = show;
        }
    }
}