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
            foreach(RoundIcon icon in icons)
                icon.select += OnRoundSelected;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnRoundSelected(Selectable sender)
        {
            if (characterFile == null)
            {
                IllogicGate.SoundManager2D.instance.PlaySFX("ui_cancel");
                string msg = string.Format("Select a non-random character");
                Overlay.instance.messagePopup.Show(msg.ToUpper(), "UNAVAILABLE");
                return;
            }

            int idx = System.Array.IndexOf(icons, sender);
            if (stats.rounds[idx].cleared)
            {
                IllogicGate.SoundManager2D.instance.PlaySFX("ui_accept");
                Overlay.instance.roundImageViewer.Show(characterFile, idx);
            }
            else if (idx < characterFile.availableRounds)
            {
                IllogicGate.SoundManager2D.instance.PlaySFX("ui_cancel");
                string diff = idx == 3 ? "hard" : "normal";
                string msg = string.Format("Clear round {0} in {1} difficulty to unlock this image", idx + 1, diff);
                Overlay.instance.messagePopup.Show(msg.ToUpper(), "IMAGE LOCKED");
            }
            else
            {
                IllogicGate.SoundManager2D.instance.PlaySFX("ui_cancel");
                string msg = string.Format("Character does not have round {0}", idx + 1);
                Overlay.instance.messagePopup.Show(msg.ToUpper(), "UNAVAILABLE");
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