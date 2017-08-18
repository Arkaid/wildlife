using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.Common.UI
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

        [SerializeField]
        Button yesButton = null;

        [SerializeField]
        Button noButton = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible { get; private set; }

        public bool isYes { get; private set; }

        GameObject lastSelected;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            yesButton.onClick.AddListener(() => { isYes = true; Close(); });
            noButton.onClick.AddListener(() => { isYes = false; Close(); });
            okButton.onClick.AddListener(Close);
        }

        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                Close();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void ShowCommon(string content, string title)
        {
            isYes = false;

            gameObject.SetActive(true);
            contentText.text = content;
            titleText.text = title;
            isVisible = true;
            Overlay.instance.background.Show();
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }

        // -----------------------------------------------------------------------------------	
        public void Show(string content, string title= "")
        {
            ShowCommon(content, title);
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
            okButton.gameObject.SetActive(true);
            okButton.Select();
        }

        // -----------------------------------------------------------------------------------	
        public void ShowYesNo(string content, string title = "")
        {
            ShowCommon(content, title);
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            okButton.gameObject.SetActive(false);
            yesButton.Select();
        }

        // -----------------------------------------------------------------------------------	
        void Close()
        {
            isVisible = false;
            gameObject.SetActive(false);
            Overlay.instance.background.Hide();
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
    }
}