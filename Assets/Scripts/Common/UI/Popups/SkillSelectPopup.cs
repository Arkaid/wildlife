using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillSelectPopup : Popup
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        SkillSelectButton[] skillButtons;

        /// <summary> Selected skill </summary>
        public Game.Skill.Type result { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public override void Show()
        {
            throw new System.NotImplementedException();
        }
        
        // -----------------------------------------------------------------------------------	
        public void Show(Game.Skill.Type select)
        {
            if (skillButtons == null)
                Initialize();

            result = Game.Skill.Type.INVALID;
            EventSystem.current.SetSelectedGameObject(skillButtons[(int)select].gameObject);
            base.Show();
        }

        // -----------------------------------------------------------------------------------	
        void Initialize()
        {
            skillButtons = GetComponentsInChildren<SkillSelectButton>();
            for (int i = 0; i < skillButtons.Length; i++)
            {
                SkillSelectButton btn = skillButtons[i];
                skillButtons[i].onClick.AddListener(() => { OnSkillSelected(btn); });
            }
        }
        // -----------------------------------------------------------------------------------	
        private void OnSkillSelected(SkillSelectButton sender)
        {
            SkillSelectButton skillButton = sender as SkillSelectButton;
            result = skillButton.skill;
            Hide();
        }
    }
}