using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterIcon : Selectable
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        /// <summary> Keep track of the last icon we selected </summary>
        static CharacterIcon lastSelected = null;

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image icon = null;

        [SerializeField]
        Image background = null;

        [SerializeField]
        Image hover = null;

        [SerializeField]
        Image selected = null;

        // --- Properties -------------------------------------------------------------------------------
        bool isSelected = false;

        public CharacterFile.File characterFile { get { return _characterFile; } set { SetCharacterFile(value); } }
        CharacterFile.File _characterFile;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Start()
        {
            base.Start();

            hoverIn += OnHoverIn;
            hoverOut += OnHoverOut;
            select += OnSelect;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void SetCharacterFile(CharacterFile.File file)
        {
            _characterFile = file;
            icon.sprite = file.baseSheet.icon;
        }

        // -----------------------------------------------------------------------------------	
        void Deselect()
        {
            isSelected = false;
            selected.enabled = false;

            if (background != null)
                background.enabled = false;
        }
        // -----------------------------------------------------------------------------------	
        private void OnSelect(Selectable obj)
        {
            // can only select one icon at the time
            if (lastSelected != null)
                lastSelected.Deselect();

            lastSelected = this;

            // change selected icon
            hover.enabled = false;
            isSelected = true;
            selected.enabled = true;

            if (background != null)
            {
                background.enabled = true;
                background.color = selected.color;
            }

            IllogicGate.SoundManager2D.instance.PlaySFX("ui_accept");
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverOut(Selectable obj)
        {
            hover.enabled = false;
            if (background != null)
                background.enabled = isSelected;
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverIn(Selectable obj)
        {
            hover.enabled = true;
            if (background != null)
            {
                background.enabled = true;
                background.color = hover.color;
            }

            IllogicGate.SoundManager2D.instance.PlaySFX("ui_select_notch");
        }
    }
}