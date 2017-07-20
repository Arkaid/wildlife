using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterIconGrid : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the icon is switched </summary>
        public event System.Action<CharacterDataFile> switched;

        /// <summary> Raised when the icon is selected </summary>
        public event System.Action<CharacterDataFile> selected;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        CharacterIcon sampleIcon = null;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            sampleIcon.gameObject.SetActive(false);
            Clear();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Clears the grid from all icons
        /// </summary>
        public void Clear()
        {
            while (transform.childCount > 1)
            {
                Transform child = transform.GetChild(0);
                if (child == sampleIcon.transform)
                    child = transform.GetChild(1);

                child.SetParent(null);
                DestroyObject(child.gameObject);
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Selects the first icon in the grid. You usually
        /// call this right after the menu screen opens
        /// </summary>
        public void SelectFirst()
        {
            Transform first = transform.GetChild(0);
            if (first == sampleIcon.transform)
                first = transform.GetChild(1);

            Toggle toggle = first.GetComponent<Toggle>();
            toggle.isOn = true;
        }


        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Adds an icon to the grid
        /// </summary>
        public void Add(CharacterDataFile characterFile)
        {
            CharacterIcon newIcon = Instantiate(sampleIcon);
            newIcon.gameObject.SetActive(true);
            newIcon.Setup(characterFile);

            newIcon.selected += selected;
            newIcon.switched += switched;

            newIcon.transform.SetParent(transform);
            newIcon.transform.localScale = Vector3.one;
        }
    }
}