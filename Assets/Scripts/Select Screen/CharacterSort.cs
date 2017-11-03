using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class CharacterSort : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        CharacterGrid characterGrid;

        // --- Properties -------------------------------------------------------------------------------
        Dropdown options;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            options = GetComponentInChildren<Dropdown>();
            options.onValueChanged.AddListener(OnOptionChanged);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnOptionChanged(int idx)
        {
            switch (idx)
            {
                case 0: characterGrid.Paginate(CharacterGrid.SortCriteria.DateCreated, false); break;
                case 1: characterGrid.Paginate(CharacterGrid.SortCriteria.DateCreated, true); break;
                case 2: characterGrid.Paginate(CharacterGrid.SortCriteria.Unplayed, true); break;
                case 3: characterGrid.Paginate(CharacterGrid.SortCriteria.CharacterName, false); break;
                case 4: characterGrid.Paginate(CharacterGrid.SortCriteria.Artist, false); break;
            }
        }
    }
}