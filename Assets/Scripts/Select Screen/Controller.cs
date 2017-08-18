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
            Debug.Log("Starting character selection screen");

            // handle buttons
            startButton.onClick.AddListener(OnStart);
            optionsButton.onClick.AddListener(OnOptions);
            randomButton.selected += OnRandomCharacterSelected;

            Transition.instance.maskValue = 1;

            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
            StartCoroutine(CheckForExit());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            Debug.Log("Loading character files");
            string[] files = System.IO.Directory.GetFiles(CharacterFile.File.dataPath, "*.chr");

            foreach (string file in files)
            {
                Debug.Log("Loading: " + file);
                CharacterFile.File charFile = new CharacterFile.File(file);
                CharacterIcon icon = characterGrid.AddCharacter(charFile);
                icon.selected += OnCharacterSelected;
                yield return null;
            }
            characterGrid.Paginate();
            characterGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------
        private void OnCharacterSelected(CharacterIcon icon)
        {
            selected = icon.characterFile;
            avatar.SetCharacter(selected);
            roundImages.SetCharacter(selected);
        }

        // -----------------------------------------------------------------------------------	
        private void OnRandomCharacterSelected(CharacterIcon sender)
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
            yield return StartCoroutine(Transition.instance.Show(false));

            Options.instance.Show();
            yield return StartCoroutine(Transition.instance.Hide());
            while (!Options.instance.isVisible)
                yield return null;
            yield return StartCoroutine(Transition.instance.Show(false));
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

            // fade out BGM and destroy sound manager (in game has its own manager)
            IllogicGate.SoundManager2D sndMgr = IllogicGate.SoundManager2D.instance;
            sndMgr.FadeoutBGM(1f);
            yield return new WaitForSeconds(1f);
            DestroyObject(sndMgr.gameObject);

            // start game
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator CheckForExit()
        {
            while (true)
            {
                if (Overlay.instance.isVisible || Options.instance.isVisible)
                    yield return null;

                if (Input.GetButtonDown("Cancel"))
                    yield return StartCoroutine(OnExit());
            }
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator OnExit()
        {
            MessagePopup popup = Overlay.instance.messagePopup;
            popup.ShowYesNo("EXIT GAME?");
            while (popup.isVisible)
                yield return null;
            yield return null;
            if (popup.isYes)
                Application.Quit();
        }
    }
}