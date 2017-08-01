using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    using Game;

    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillSelect : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image player;

        [SerializeField]
        Text skillName;

        // --- Properties -------------------------------------------------------------------------------
        Button button;


        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            SetSkill();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnClick()
        {
            int next = (int)(Skill.type + 1) % (int)Skill.Type.COUNT;
            Skill.type = (Skill.Type)next;
            SetSkill();
        }

        // -----------------------------------------------------------------------------------	
        void SetSkill()
        {
            player.color = Skill.playerColor[Skill.type];
            skillName.text = Skill.type.ToString().ToUpper();
        }
    }
}