using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    [ExecuteInEditMode]
    public class CharacterPreview : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Minimum amount of space to leave at the top of the preview")]
        float minimumTopSpace = 20;

        // --- Properties -------------------------------------------------------------------------------
        int lastWidth = -1;
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