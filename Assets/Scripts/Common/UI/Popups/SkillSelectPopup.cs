﻿using System.Collections;
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

        /// <summary> True if the user canceled the skill selection </summary>
        public bool canceled { get; private set; }

        public Game.Skill.Type selectedSkill { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                Close(true);
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
        public void Show(Game.Skill.Type setDefault)
        {
            if (skillButtons == null)
                Initialize();

            selectedSkill = setDefault;

            canceled = false;

            EventSystem.current.SetSelectedGameObject(skillButtons[(int)setDefault].gameObject);

            base.Show();
        }

        // -----------------------------------------------------------------------------------	
        private void OnSkillSelected(SkillSelectButton sender)
        {
            SkillSelectButton skillButton = sender as SkillSelectButton;
            selectedSkill = skillButton.skill;
            Close(false);
        }
        
        // -----------------------------------------------------------------------------------	
        void Close(bool canceled)
        {
            this.canceled = canceled;
            base.Hide();
        }
    }
}