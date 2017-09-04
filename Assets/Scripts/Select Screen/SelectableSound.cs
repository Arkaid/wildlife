using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Selectable))]
    public class SelectableSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerDownHandler
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        bool enableHover = true;

        [SerializeField]
        string _hover = null;
        string hover { get { return string.IsNullOrEmpty(_hover) ? "ui_hover" : _hover; } }

        [SerializeField]
        bool enablePressed = true;

        [SerializeField]
        string _pressed = null;
        string pressed { get { return string.IsNullOrEmpty(_pressed) ? "ui_accept" : _pressed; } }

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            if (Input.GetButtonDown("Submit") && EventSystem.current.currentSelectedGameObject == gameObject)
                OnPointerDown(null);
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void OnPointerDown(PointerEventData eventData)
        {
            if (enablePressed)
                SoundManager.instance.PlaySFX(pressed);
        }

        // -----------------------------------------------------------------------------------	
        public void OnPointerEnter(PointerEventData eventData)
        {
            GetComponent<Selectable>().Select();
        }

        // -----------------------------------------------------------------------------------	
        public void OnSelect(BaseEventData eventData)
        {
            if (enableHover)
                SoundManager.instance.PlaySFX(hover);
        }

    }
}