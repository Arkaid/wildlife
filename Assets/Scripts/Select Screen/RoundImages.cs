using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.SelectScreen
{
    using Common.UI;

    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundImages : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        RoundIcon[] icons = null;

        [SerializeField]
        Sprite lockedSprite = null;

        [SerializeField]
        Sprite unavailableSprite = null;

        // --- Properties -------------------------------------------------------------------------------
        CharacterFile.File characterFile;
        Data.CharacterStats stats;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            for (int i = 0; i < icons.Length; i++)
            {
                RoundIcon icon = icons[i];
                int round = i;
                icon.onClick.AddListener(() => { OnRoundSelected(round); });
            }
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnRoundSelected(int round)
        {
            if (characterFile == null)
            {
                SoundManager.instance.PlaySFX("ui_cancel");
                string msg = string.Format("Select a non-random character");
                StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "UNAVAILABLE"));
                return;
            }

            if (stats.rounds[round].cleared)
            {
                RoundImageViewer.instance.Show(characterFile, round);
            }
            else if (round < characterFile.availableRounds)
            {
                SoundManager.instance.PlaySFX("ui_cancel");
                string diff = round == Config.Rounds - 1 ? "hard" : "normal";
                string msg = string.Format("Clear round {0} in {1} difficulty to unlock this image", round + 1, diff);
                StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "IMAGE LOCKED"));
            }
            else
            {
                SoundManager.instance.PlaySFX("ui_cancel");
                string msg = string.Format("Character does not have round {0}", round + 1);
                StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "UNAVAILABLE"));
            }
        }

        // -----------------------------------------------------------------------------------	
        public void Reset()
        {
            characterFile = null;
            for (int i = 0; i < Config.Rounds; i++)
                icons[i].previewIcon.sprite = unavailableSprite;
        }

        // -----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterFile.File characterFile)
        {
            stats = Data.SaveFile.instance.GetCharacterStats(characterFile.guid);
            this.characterFile = characterFile;
            for (int i = 0; i < Config.Rounds; i++)
            {
                if (stats.rounds[i].cleared)
                    icons[i].previewIcon.sprite = characterFile.baseSheet.roundIcons[i];
                else if (i < characterFile.availableRounds)
                    icons[i].previewIcon.sprite = lockedSprite;
                else
                    icons[i].previewIcon.sprite = unavailableSprite;
            }
        }
    }
}