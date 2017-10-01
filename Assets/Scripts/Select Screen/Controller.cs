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
            // (ie, loaded directly from editor)
            if (SoundManager.instance == null)
                Instantiate(Resources.Load("Sound Manager"));

            // when it gets back from the game, there isn't any BGM playbg
            if (!SoundManager.instance.IsPlayingBGM("Intersekt - Track 01"))
                SoundManager.instance.PlayBGM("Intersekt - Track 01", 4.8f);

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
                yield return null;
                if (Input.GetButtonDown("Cancel"))
                    yield return StartCoroutine(ExitConfirm());

                if (PopupManager.instance.isVisible || Options.instance.isVisible)
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
            state = State.StartingGame;

            // select random character if needed
            if (selected == null)
                selected = characterGrid.SelectRandomCharacter();

            avatar.SwitchImage();
            Game.Controller.sourceFile = selected;

            // select skill
            yield return StartCoroutine(PopupManager.instance.ShowSkillPopup());

            // skill select canceled?
            if (PopupManager.instance.skill == Game.Skill.Type.INVALID)
            {
                avatar.SwitchImage();
                StartCoroutine(SelectingCharacter());
                yield break;
            }

            Debug.Log("Starting game");

            // disable the graphics raycaster to avoid inputing anything while loading
            GetComponent<GraphicRaycaster>().enabled = false;

            // set and save selected skill
            Data.Options.instance.skill = PopupManager.instance.skill;
            Data.Options.instance.Save();

            // fade out BGM and destroy sound manager (in game has its own manager)
            SoundManager.instance.FadeoutBGM(1f);
            yield return new WaitForSeconds(1f);

            // start game
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ExitConfirm()
        {
            state = State.ExitConfirm;

            yield return StartCoroutine(PopupManager.instance.ShowMessagePopup("EXIT GAME?", "EXIT", MessagePopup.Type.YesNo));
            if (PopupManager.instance.button == PopupManager.Button.Yes)
            {
                Application.Quit();
                Debug.Log("Exiting");
            }
            else
                StartCoroutine(SelectingCharacter());
        }
    }
}