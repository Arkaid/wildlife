using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundIcon : Selectable
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly Color HoverColor = new Color32(224, 192, 131, 255);

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image _previewIcon = null;
        public Image previewIcon { get { return _previewIcon; } }

        // --- Properties -------------------------------------------------------------------------------
        Image frame;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Start()
        {
            base.Start();

            frame = GetComponent<Image>();
            hoverIn += OnHoverIn;
            hoverOut += OnHoverOut;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnHoverIn(Selectable obj)
        {
            IllogicGate.SoundManager2D.instance.PlaySFX("ui_select_notch");
            frame.color = HoverColor;
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverOut(Selectable obj)
        {
            frame.color = Color.white;
        }
    }
}