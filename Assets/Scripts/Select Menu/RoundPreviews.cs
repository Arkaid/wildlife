using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundPreviews : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image [] rounds;

        [SerializeField]
        Sprite notClearedIcon;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> The data file for the currently assigned character </summary>
        CharacterDataFile character;

        /// <summary> Stats for the current character </summary>
        Data.CharacterStats stats;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            for (int i = 0; i < Config.Rounds; i++)
            {
                int idx = i;
                rounds[i].GetComponent<Button>().onClick.AddListener(delegate { OnPreviewSelected(idx); });
            }
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterDataFile character)
        {
            this.character = character;
            stats = Data.SaveFile.instance.GetCharacterStats(character.guid);

            // get the "cleared" state from file
            Sprite[] roundIcons = character.characterSheet.roundIcons;

            for (int i = 0; i < Config.Rounds; i++)
                rounds[i].sprite = stats.rounds[i].cleared ? roundIcons[i] : notClearedIcon;
        }

        // -----------------------------------------------------------------------------------	
        void OnPreviewSelected(int idx)
        {
            if (stats.rounds[idx].cleared)
            {
                Overlay.instance.ShowBaseImage(idx, character);
            }

            // TODO: popup "clear the image in game to view it here"
            else
            {

            }
        }
    }
}