using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Text titleText;

        [SerializeField]
        Text contentText;

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible { get; private set; }

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
        }

        // -----------------------------------------------------------------------------------	
        public void Close()
        {
            isVisible = false;
            gameObject.SetActive(false);
            Overlay.instance.background.Hide();
        }
    }
}