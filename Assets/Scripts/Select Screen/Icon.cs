using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Icon : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the icon is switched </summary>
        public event System.Action<CharacterFile.File> switched;

        /// <summary> Raised when the icon is selected </summary>
        public event System.Action<CharacterFile.File> selected;

        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Cycle time for the colors </summary>
        const float CycleTime = 1.5f;

        static readonly Color ColorA = new Color32(255, 90, 0, 255);
        static readonly Color ColorB = new Color32(255, 197, 165, 255);

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image normalFrame = null;

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
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        // -----------------------------------------------------------------------------------	
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!toggle.isOn)
            {
                toggle.isOn = true;
                if (switched != null)
                    switched(characterFile);
            }
        }

        // -----------------------------------------------------------------------------------	
        public void OnSelect(BaseEventData eventData)
        {
            if (selected != null)
                selected(characterFile);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Setup(CharacterFile.File file)
        {
            iconImage.sprite = file.baseSheet.icon;
            this.characterFile = file;
        }

        // -----------------------------------------------------------------------------------	
        void OnToggleValueChanged(bool value)
        {
            selectedFrame.gameObject.SetActive(value);
            normalFrame.gameObject.SetActive(!value);

            if (value)
                StartCoroutine(CycleColors());
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
    }
}