using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterIcon : Toggle
    {
        // --- Events -----------------------------------------------------------------------------------
        public event System.Action<CharacterIcon> selected;
        public event System.Action<CharacterIcon> highlighted;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Background image (used for highlighting toggled status) </summary>
        Image background = null;

        public CharacterFile.File characterFile { get { return _characterFile; } set { SetCharacterFile(value); } }
        CharacterFile.File _characterFile;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Awake()
        {
            base.Awake();

            background = GetComponent<Image>();
            onValueChanged.AddListener(OnValueChanged);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnValueChanged(bool value)
        {
            // don't change the background for the random character icon
            if (characterFile != null)
                background.enabled = value;

            if (value && selected != null)
                selected(this);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (highlighted != null)
                highlighted(this);
        }

        // -----------------------------------------------------------------------------------	
        void SetCharacterFile(CharacterFile.File file)
        {
            _characterFile = file;
            Image icon = transform.Find("Icon").GetComponent<Image>();
            icon.sprite = file.baseSheet.icon;
        }
    }
}