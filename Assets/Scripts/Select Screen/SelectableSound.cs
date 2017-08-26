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
    public class SelectableSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void OnPointerEnter(PointerEventData eventData)
        {
            GetComponent<Selectable>().Select();
        }

        // -----------------------------------------------------------------------------------	
        public void OnSelect(BaseEventData eventData)
        {
            SoundManager.instance.PlaySFX("ui_select_notch");
        }
    }
}