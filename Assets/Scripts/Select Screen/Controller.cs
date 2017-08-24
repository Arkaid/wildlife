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
        enum State
        {
            Initializing,
            SelectingCharacter,
            OptionsScreen,
            StartingGame,
            ExitConfirm,
        }


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
        Button exitButton = null;

        [SerializeField]
        CharacterIcon randomButton = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Character we selected </summary>
        CharacterFile.File selected;

        /// <summary> State of the UI </summary>
        State state;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            StartCoroutine(Initalize());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Initializes the UI
        /// </summary>
        IEnumerator Initalize()
        {
            state = State.Initializing;
            Debug.Log("Initializing character selection screen");

            // load the sound manager if we didn't come in from the title screen
            // (ie, game ended)
            if (SoundManager.instance == null)
            {
                Instantiate(Resources.Load("Outgame Sound Manager"));
                SoundManager.instance.PlayBGM("outgame", 4.8f);
            }

            // handle buttons
            startButton.onClick.AddListener(() => { StartCoroutine(StartGame()); });
            optionsButton.onClick.AddListener(() => { StartCoroutine(ShowOptions()); });
            exitButton.onClick.AddListener(() => { StartCoroutine(ExitConfirm()); });
            randomButton.selected += OnCharacterSelected;

            Transition.instance.maskValue = 1;

            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
            StartCoroutine(SelectingCharacter());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Loads all character sheets and sets up icons
        /// </summary>
        /// <returns></returns>
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

            // wait until the charcter grid finishes initializing
            while (!characterGrid.isReady)
                yield return null;
                     
            characterGrid.Paginate();
            characterGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// This is the main loop, basically. Just waiting around for a character to be selected
        /// </summary>
        IEnumerator SelectingCharacter()
        {
            state = State.SelectingCharacter;
            while (state == State.SelectingCharacter)
            {
                if (Input.GetButtonDown("Cancel"))
                    yield return StartCoroutine(ExitConfirm());

                if (Overlay.instance.isVisible || Options.instance.isVisible)
                    yield return null;

                yield return null;
            }
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Callback from the character icon, when a character gets selected.
        /// </summary>
        private void OnCharacterSelected(CharacterIcon icon)
        {
            if (icon == randomButton)
            {
                selected = null;
                avatar.SetCharacter(null);
                roundImages.Reset();
            }
            else
            {
                selected = icon.characterFile;
                avatar.SetCharacter(selected);
                roundImages.SetCharacter(selected);
            }
        }


        // -----------------------------------------------------------------------------------	
        IEnumerator ShowOptions()
        {
            Debug.Log("Showing options screen");
            state = State.OptionsScreen;

            // show transition, show options screen.
            yield return StartCoroutine(Transition.instance.Show(false));
            Options.instance.Show();
            yield return StartCoroutine(Transition.instance.Hide());

            // wait until the option screen closes
            while (Options.instance.isVisible)
                yield return null;

            // show transition, hide options screen
            yield return StartCoroutine(Transition.instance.Show(false));
            Options.instance.Hide();
            optionsButton.Select();
            yield return StartCoroutine(Transition.instance.Hide());

            // go back to selecting stuffs
            StartCoroutine(SelectingCharacter());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator StartGame()
        {
            Debug.Log("Starting game");
            state = State.StartingGame;

            // select random character if needed
            if (selected == null)
                selected = characterGrid.SelectRandomCharacter();

            avatar.SwitchImage();
            Game.Controller.sourceFile = selected;

            // select skill
            Overlay.instance.skillSelectPopup.Show(Config.instance.skill);
            while (Overlay.instance.skillSelectPopup.isVisible)
                yield return null;

            // skill select canceled?
            if (Overlay.instance.skillSelectPopup.canceled)
            {
                avatar.SwitchImage();
                startButton.Select();
                StartCoroutine(SelectingCharacter());
                yield break;
            }

            // set and save selected skill
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
        IEnumerator ExitConfirm()
        {
            state = State.ExitConfirm;

            MessagePopup popup = Overlay.instance.messagePopup;
            popup.ShowYesNo("EXIT GAME?");
            while (popup.isVisible)
                yield return null;
            if (popup.isYes)
            {
                Application.Quit();
                Debug.Log("Exiting");
            }
            else
                StartCoroutine(SelectingCharacter());
        }
    }
}