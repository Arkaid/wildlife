using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class MessagePopup : Popup
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum Type
        {
            YesNo,
            Ok
        }

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
        public PopupManager.Result result { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            yesButton.onClick.AddListener(() => { result = PopupManager.Result.YesButton; Hide(); });
            noButton.onClick.AddListener(() => { result = PopupManager.Result.NoButton; Hide(); });
            okButton.onClick.AddListener(() => { result = PopupManager.Result.OkButton; Hide(); });
        }

        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                Hide();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public override void Show()
        {
            throw new System.NotImplementedException();
        }

        // -----------------------------------------------------------------------------------	
        public void Show(string content, string title= "", Type type = Type.Ok)
        {
            result = PopupManager.Result.None;
            contentText.text = content;
            titleText.text = title;

            yesButton.gameObject.SetActive(type == Type.YesNo);
            noButton.gameObject.SetActive(type == Type.YesNo);
            okButton.gameObject.SetActive(type == Type.Ok);

            switch (type)
            {
                case Type.YesNo: yesButton.Select(); break;
                case Type.Ok: okButton.Select(); break;
            }

            base.Show();
        }
    }
}