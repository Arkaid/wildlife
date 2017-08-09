using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillSelectButton : Selectable
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image hover = null;

        [SerializeField]
        Image background = null;

        [SerializeField]
        Game.Skill.Type _skill;
        public Game.Skill.Type skill { get { return _skill; } }

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Initialize()
        {
            hoverIn += OnHoverIn;
            hoverOut += OnHoverOut;
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverOut(Selectable obj)
        {
            hover.enabled = false;
            background.enabled = false;
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverIn(Selectable obj)
        {
            hover.enabled = true;
            background.enabled = true;
            background.color = hover.color;
        }

    }
}