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
            Button_Yes,
            Button_No,
            Button_Ok,
            Skill_Freeze,
            Skill_Shield,
            Skill_Speed,
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
        SkillSelectPopup skillSelectPopup = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool isVisible { get; private set; }

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
        /// <summary>
        /// Shows a standard popup message. Run as coroutine
        /// </summary>
        public IEnumerator ShowMessagePopup(string content, string title = "", MessagePopup.Type type = MessagePopup.Type.Ok)
        {
            isVisible = true;
            background.SetActive(true);
            GameObject lastSelected = EventSystem.current.currentSelectedGameObject;

            messagePopup.Show(content, title, type);
            while (messagePopup.isActiveAndEnabled)
                yield return null;

            result = messagePopup.result;

            EventSystem.current.SetSelectedGameObject(lastSelected);
            background.SetActive(false);
            isVisible = false;
        }

        // -----------------------------------------------------------------------------------	
        public IEnumerator ShowSkillPopup()
        {
            isVisible = true;
            background.SetActive(true);
            GameObject lastSelected = EventSystem.current.currentSelectedGameObject;

            skillSelectPopup.Show();
            while (messagePopup.isActiveAndEnabled)
                yield return null;

            EventSystem.current.SetSelectedGameObject(lastSelected);
            background.SetActive(false);
            isVisible = false;
        }

        // -----------------------------------------------------------------------------------	
    }
}