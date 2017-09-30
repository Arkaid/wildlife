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
            Ok,
            UnlockYesNo,
            UnlockOk,
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

        [SerializeField]
        UnlockLetters unlock = null;

        // --- Properties -------------------------------------------------------------------------------
        public PopupManager.Button result { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            yesButton.onClick.AddListener(() => { result = PopupManager.Button.Yes; Hide(); });
            noButton.onClick.AddListener(() => { result = PopupManager.Button.No; Hide(); });
            okButton.onClick.AddListener(() => { result = PopupManager.Button.Ok; Hide(); });
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
            base.Show();

            result = PopupManager.Button.None;
            contentText.text = content;
            titleText.text = title;

            yesButton.gameObject.SetActive(type == Type.YesNo || type == Type.UnlockYesNo);
            noButton.gameObject.SetActive(type == Type.YesNo || type == Type.UnlockYesNo);
            okButton.gameObject.SetActive(type == Type.Ok || type == Type.UnlockOk);

            // show UNLOCK or not (resize if needed)
            unlock.gameObject.SetActive(type == Type.UnlockOk || type == Type.UnlockYesNo);
            unlock.SetFromSaveFile();
            RectTransform rt = GetComponent<RectTransform>();
            Vector2 size = rt.sizeDelta;
            size.y = unlock.isActiveAndEnabled ? 240 : 200;
            rt.sizeDelta = size;

            switch (type)
            {
                case Type.YesNo:
                case Type.UnlockYesNo:
                    EventSystem.current.SetSelectedGameObject(yesButton.gameObject); break;
                case Type.Ok:
                case Type.UnlockOk:
                    EventSystem.current.SetSelectedGameObject(okButton.gameObject); break;
            }
        }
    }
}