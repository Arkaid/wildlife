using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    [ExecuteInEditMode]
    public class Avatar : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Minimum amount of space to leave at the top of the preview")]
        float minimumTopSpace = 20;

        [SerializeField]
        Image characterImage = null;

        [SerializeField]
        Image nameImage= null;

        [SerializeField]
        Animation nameAnimation = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Used to auto adjust the position on screen </summary>
        int lastWidth = -1;

        /// <summary> Used to auto adjust the position on screen </summary>
        int lastHeight = -1;

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
        void Start()
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }

        // -----------------------------------------------------------------------------------	
        void Update()
        {
            if (Screen.width != lastWidth || lastHeight != Screen.height)
            {
                AdjustPosition();
                lastWidth = Screen.width;
                lastHeight = Screen.height;
            }
        }

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

        // -----------------------------------------------------------------------------------	
        void AdjustPosition()
        {
            float top = rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y;
            float maxTop = Screen.height - minimumTopSpace;
            if (top > maxTop)
            {
                Vector2 position = new Vector2();
                position.x = 0;
                position.y = maxTop - rectTransform.sizeDelta.y;
                rectTransform.anchoredPosition = position;
            }
            else
                rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}