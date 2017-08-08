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
        Image background = null;

        [SerializeField]
        Image hover = null;

        [SerializeField]
        Image selected = null;

        // --- Properties -------------------------------------------------------------------------------
        bool isSelected = false;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Start()
        {
            hoverIn += OnHoverIn;
            hoverOut += OnHoverHour;
            select += OnSelect;
        }

        // --- Methods ----------------------------------------------------------------------------------
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
        }

        // -----------------------------------------------------------------------------------	
        private void OnHoverHour(Selectable obj)
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
        }

#if OLD
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the icon is switched </summary>
        public event System.Action<CharacterFile.File> selected;

        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Cycle time for the colors </summary>
        const float CycleTime = 1.5f;

        static readonly Color ColorA = new Color32(255, 90, 0, 255);
        static readonly Color ColorB = new Color32(255, 197, 165, 255);

        // --- Static Properties ------------------------------------------------------------------------
        /// <summary> Using to de-highlight icons when hovering / changing to a new one </summary>
        static GameObject lastHighlighted = null;

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image selectedFrame = null;

        [SerializeField]
        Image iconImage = null;

        // --- Properties -------------------------------------------------------------------------------
        public Toggle toggle
        {
            get
            {
                if (_toggle == null)
                    _toggle = GetComponent<Toggle>();
                return _toggle;
            }
        }
        Toggle _toggle = null;

        /// <summary> Source file associated to this icon </summary>
        CharacterFile.File characterFile;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            selectedFrame.gameObject.SetActive(false);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Setup(CharacterFile.File file)
        {
            iconImage.sprite = file.baseSheet.icon;
            characterFile = file;
        }

        // -----------------------------------------------------------------------------------	
        void OnToggleValueChanged(bool value)
        {
            selectedFrame.gameObject.SetActive(value);

            if (value)
            {
                StartCoroutine(CycleColors());
                if (selected != null)
                    selected(characterFile);
            }
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CycleColors()
        {
            while (toggle.isOn)
            {
                float t = Mathf.PingPong(Time.time, CycleTime) / CycleTime;
                selectedFrame.color = Color.Lerp(ColorA, ColorB, t);
                yield return null;
            }
        }
#endif
    }
}