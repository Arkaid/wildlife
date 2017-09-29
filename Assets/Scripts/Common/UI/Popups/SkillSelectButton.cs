﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SkillSelectButton : Button
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Game.Skill.Type _skill;
        public Game.Skill.Type skill { get { return _skill; } }

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}