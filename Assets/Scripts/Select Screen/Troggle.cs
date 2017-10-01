using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// A troggle is a toogle with three values
    /// </summary>
    public class Troggle : Button
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum Value
        {
            Yes,
            No,
            None,
        }

        static readonly Color NoColor = new Color32(212, 50, 50, 255);
        static readonly Color YesColor = new Color32(45, 154, 50, 255);

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        Image target { get { return transform.Find("Background/Checkmark").GetComponent<Image>(); } }

        public Value value { get { return _value; } set { SetValue(value); } }
        Value _value;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Start()
        {
            base.Start();
            onClick.AddListener(OnClick);
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void SetValue(Value value)
        {
            _value = value;
            switch (value)
            {
                case Value.Yes: target.color = YesColor; break;
                case Value.No: target.color = NoColor; break;
                case Value.None: target.color = Color.clear; break;
            }
        }

        // -----------------------------------------------------------------------------------	
        void OnClick()
        {
            value = (Value)(((int)value + 1) % 3);
        }
    }
}