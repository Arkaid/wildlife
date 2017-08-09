using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class MessagePopup : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Text titleText = null;

        [SerializeField]
        Text contentText = null;

        [SerializeField]
        Button okButton = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible { get; private set; }

        GameObject lastSelected;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Show(string content, string text= "")
        {
            gameObject.SetActive(true);
            contentText.text = content;
            titleText.text = text;
            isVisible = true;
            Overlay.instance.background.Show();
            lastSelected = EventSystem.current.currentSelectedGameObject;
            okButton.Select();
        }

        // -----------------------------------------------------------------------------------	
        public void Close()
        {
            isVisible = false;
            gameObject.SetActive(false);
            Overlay.instance.background.Hide();
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
    }
}