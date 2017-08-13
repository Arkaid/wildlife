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
    public class Overlay : IllogicGate.SingletonBehaviour<Overlay>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Background _background = null;
        public Background background { get { return _background; } }

        [SerializeField]
        MessagePopup _messagePopup = null;
        public MessagePopup messagePopup { get { return _messagePopup; } }

        [SerializeField]
        SkillSelectPopup _skillSelectPopup = null;
        public SkillSelectPopup skillSelectPopup { get { return _skillSelectPopup; } }

        [SerializeField]
        RoundImageViewer _roundImageViewer = null;
        public RoundImageViewer roundImageViewer { get { return _roundImageViewer; } }

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}