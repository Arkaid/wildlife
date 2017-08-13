using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    using Common.UI;

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Controls the select screen
    /// </summary>
    public class Controller : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        CharacterGrid characterGrid = null;

        [SerializeField]
        RoundImages roundImages = null;

        [SerializeField]
        Avatar avatar = null;

        [SerializeField]
        Button startButton = null;

        [SerializeField]
        Button optionsButton = null;

        [SerializeField]
        CharacterIcon randomButton = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Character we selected </summary>
        CharacterFile.File selected;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private IEnumerator Start()
        {
            // handle buttons
            startButton.onClick.AddListener(OnStart);
            optionsButton.onClick.AddListener(OnOptions);
            randomButton.select += OnRandomCharacterSelected;

            Transition.instance.maskValue = 1;

            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            string[] files = System.IO.Directory.GetFiles(CharacterFile.File.dataPath, "*.chr");

            foreach (string file in files)
            {
                CharacterFile.File charFile = new CharacterFile.File(file);
                CharacterIcon icon = characterGrid.AddCharacter(charFile);
                icon.select += OnCharacterSelected;
                yield return null;
            }
            characterGrid.Paginate();
            characterGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------
        private void OnCharacterSelected(Selectable sender)
        {
            CharacterIcon icon = sender as CharacterIcon;
            selected = icon.characterFile;
            avatar.SetCharacter(selected);
            roundImages.SetCharacter(selected);
        }

        // -----------------------------------------------------------------------------------	
        private void OnRandomCharacterSelected(Selectable obj)
        {
            selected = null;
            avatar.SetCharacter(null);
            roundImages.Reset();
        }

        // -----------------------------------------------------------------------------------	
        void OnOptions()
        {
            StartCoroutine(LoadOptions());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadOptions()
        {
            yield return StartCoroutine(Transition.instance.Show());

            Options.instance.Show();
            yield return StartCoroutine(Transition.instance.Hide());
            while (!Options.instance.isDone)
                yield return null;
            yield return StartCoroutine(Transition.instance.Show());
            Options.instance.Hide();
            optionsButton.Select();
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // -----------------------------------------------------------------------------------	
        private void OnStart()
        {
            if (selected == null)
                selected = characterGrid.SelectRandomCharacter();
            avatar.SwitchImage();
            StartCoroutine(LoadGame());
        }
        

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame()
        {
            Game.Controller.sourceFile = selected;

            // select skill;
            Overlay.instance.skillSelectPopup.Show(Config.instance.skill);
            while (Overlay.instance.skillSelectPopup.isVisible)
                yield return null;
            if (Overlay.instance.skillSelectPopup.canceled)
            {
                avatar.SwitchImage();
                startButton.Select();
                yield break;
            }
            Config.instance.skill = Overlay.instance.skillSelectPopup.selectedSkill;
            Config.instance.SaveOptions();
            Overlay.instance.background.Show(Color.clear);


            // start game
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
    }
}