using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple overlay for with popups and other useful overlays
    /// </summary>
    public class PopupManager : IllogicGate.SingletonBehaviour<PopupManager>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum Result
        {
            None,
            YesButton,
            NoButton,
            OkButton,
        }


        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        GameObject background;

        [SerializeField]
        MessagePopup messagePopup = null;

        [SerializeField]
        SkillSelectPopup _skillSelectPopup = null;
        public SkillSelectPopup skillSelectPopup { get { return _skillSelectPopup; } }

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible
        {
            get
            {
                return
                    messagePopup.isVisible ||
                    skillSelectPopup.isVisible;
            }
        }

        public Result result { get; private set; }
       
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();
            background.SetActive(false);
            messagePopup.Hide();
            skillSelectPopup.Hide();
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public IEnumerator ShowMessagePopup(string content, string title = "", MessagePopup.Type type = MessagePopup.Type.Ok)
        {
            background.SetActive(true);
            GameObject lastSelected = EventSystem.current.currentSelectedGameObject;

            messagePopup.Show(content, title, type);
            while (messagePopup.isActiveAndEnabled)
                yield return null;

            result = messagePopup.result;

            EventSystem.current.SetSelectedGameObject(lastSelected);
            background.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
    }
}