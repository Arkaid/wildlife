using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Avatar : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image characterImage = null;

        [SerializeField]
        Image nameImage= null;

        [SerializeField]
        Animation nameAnimation = null;

        // --- Properties -------------------------------------------------------------------------------
        RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        RectTransform _rectTransform;

        /// <summary> Character we're currently showing </summary>
        CharacterFile.File characterFile;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterFile.File file)
        {
            characterFile = file;
            characterImage.sprite = file.baseSheet.avatarA;
            nameImage.sprite = file.baseSheet.name;

            nameAnimation.Stop();
            nameAnimation.Play();
        }
        
        // -----------------------------------------------------------------------------------	
        public void SetSelected()
        {
            characterImage.sprite = characterFile.baseSheet.avatarB;
        }
    }
}