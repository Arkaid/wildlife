using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterIcon : MonoBehaviour, ISelectHandler
    {
        // --- Events -----------------------------------------------------------------------------------
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

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            toggle.onValueChanged.AddListener(SetSelected);
        }

        // -----------------------------------------------------------------------------------	
        public void OnSelect(BaseEventData eventData)
        {
            if (!toggle.isOn)
                toggle.isOn = true;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void SetSelected(bool value)
        {
            selectedFrame.gameObject.SetActive(value);
            normalFrame.gameObject.SetActive(!value);

            if (value)
                StartCoroutine(CycleColors());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CycleColors()
        {
            while(toggle.isOn)
            {
                float t = Mathf.PingPong(Time.time, CycleTime) / CycleTime;
                selectedFrame.color = Color.Lerp(ColorA, ColorB, t);
                yield return null;
            }
        }
    }
}