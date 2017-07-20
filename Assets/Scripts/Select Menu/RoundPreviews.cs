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
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterDataFile character)
        {
            // get the "cleared" state from file
            Data.CharacterStats stats = Data.SaveFile.instance.GetCharacterStats(character.guid);
            Sprite[] roundIcons = character.characterSheet.roundIcons;

            for (int i = 0; i < Config.Rounds; i++)
                rounds[i].sprite = stats.rounds[i].cleared ? roundIcons[i] : notClearedIcon;

        }
    }
}