using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Displays the UNLOCK letters
    /// </summary>
    public class UnlockLetters : MonoBehaviour
    {
        [System.Serializable]
        struct Letter
        {
            public UNLOCK letter;
            public Image image;
            public Sprite sprite;
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        List<Letter> letters = null;

        [SerializeField]
        Sprite emptyLetter = null;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads the state in the save file and reflects it on the UI
        /// </summary>
        public void SetFromSaveFile()
        {
            foreach (UNLOCK letter in System.Enum.GetValues(typeof(UNLOCK)))
                ShowLetter(letter, Data.SaveFile.instance.unlockState[letter]);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows or hide a given letter
        /// </summary>
        public void ShowLetter(UNLOCK letter, bool show)
        {
            Letter let = letters.Find(l => l.letter == letter);
            let.image.sprite = show ? let.sprite : emptyLetter;
        }
    }
}