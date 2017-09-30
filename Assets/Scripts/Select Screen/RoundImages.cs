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

                Data.UnlockState unlockState = Data.SaveFile.instance.unlockState;
                if (unlockState.allCollected)
                {
                    // the previous round must also be unlocked
                    if (round == 0 || stats.rounds[round - 1].cleared)
                        StartCoroutine(AskToUnlock(round));
                    else
                    {
                        string msg = "Unlock the previous image first!";
                        StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "IMAGE LOCKED", MessagePopup.Type.UnlockOk));
                    }
                }
                else
                {
                    string diff = round == Config.Rounds - 1 ? "hard" : "normal";
                    string msg = string.Format("Clear round {0} in {1} difficulty,\nor collect all 6 letters to unlock", round + 1, diff);
                    StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "IMAGE LOCKED", MessagePopup.Type.UnlockOk));
                }
            }
            else
            {
                SoundManager.instance.PlaySFX("ui_cancel");
                string msg = string.Format("Character does not have round {0}", round + 1);
                StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "UNAVAILABLE"));
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Asks if you want to unlock the image
        /// </summary>
        /// <param name="round"></param>
        /// <returns></returns>
        IEnumerator AskToUnlock(int round)
        {
            Data.UnlockState unlockState = Data.SaveFile.instance.unlockState;

            string msg = string.Format("Do you want to unlock this image?");
            yield return StartCoroutine(PopupManager.instance.ShowMessagePopup(msg.ToUpper(), "UNLOCK IMAGE?", MessagePopup.Type.UnlockYesNo));

            if (PopupManager.instance.button == PopupManager.Button.Yes)
            {
                unlockState.Clear();
                stats.rounds[round].cleared = true;
                icons[round].AnimateUnlock(characterFile.baseSheet.roundIcons[round]);
                Data.SaveFile.instance.Save();
            }
        }

        // -----------------------------------------------------------------------------------	
        public void Reset()
        {
            characterFile = null;
            for (int i = 0; i < Config.Rounds; i++)
                icons[i].SetUnlocked(unavailableSprite);
        }

        // -----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterFile.File characterFile)
        {
            bool saveNeeded = false;
            stats = Data.SaveFile.instance.GetCharacterStats(characterFile.guid);
            this.characterFile = characterFile;
            for (int i = 0; i < Config.Rounds; i++)
            {
                if (stats.rounds[i].cleared)
                {
                    // only play the animation the first time you unlock an image
                    if (stats.rounds[i].lockAnimationPlayed)
                        icons[i].SetUnlocked(characterFile.baseSheet.roundIcons[i]);
                    else
                    {
                        icons[i].AnimateUnlock(characterFile.baseSheet.roundIcons[i]);
                        stats.rounds[i].lockAnimationPlayed = true;
                        saveNeeded = true;
                    }

                }
                else if (i < characterFile.availableRounds)
                    icons[i].SetLocked();
                else
                    icons[i].SetUnlocked(unavailableSprite);
            }

            if (saveNeeded)
                Data.SaveFile.instance.Save();
        }
    }
}