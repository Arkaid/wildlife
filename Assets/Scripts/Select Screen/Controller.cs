using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Jintori.SelectScreen
{
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

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Character we selected </summary>
        CharacterFile.File selected;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private IEnumerator Start()
        {
            // handle buttons
            startButton.onClick.AddListener(OnStart);

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
        private void OnStart()
        {
            avatar.SwitchImage();
            StartCoroutine(LoadGame());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame()
        {
            Game.Controller.sourceFile = selected;

            // select skill;
            Overlay.instance.skillSelectPopup.Show(Game.Skill.type);
            while (Overlay.instance.skillSelectPopup.isVisible)
                yield return null;
            Game.Skill.type = Overlay.instance.skillSelectPopup.selectedSkill;
            Overlay.instance.background.Show(Color.clear);

            // start game
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
#if OLD
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        [SerializeField]
        IconGrid iconGrid = null;

        [SerializeField]
        Avatar avatar = null;

        [SerializeField]
        Rounds rounds = null;

        [SerializeField]
        Button startButton = null;

        [SerializeField]
        Button optionsButton = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Character we selected </summary>
        CharacterFile.File selected;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Start()
        {
            Transition.instance.maskValue = 1;

            // hide the overlay if it was showing
            Overlay.instance.Hide();

            // handle buttons
            startButton.onClick.AddListener(OnStart);

            iconGrid.selected += OnCharacterSelected;
            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
        }
        
        // -----------------------------------------------------------------------------------	
        private void OnStart()
        {
            avatar.SetSelected();
            StartCoroutine(LoadGame());
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSelected(CharacterFile.File file)
        {
            selected = file;
            avatar.SetCharacter(file);
            rounds.SetCharacter(file);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            string [] files = System.IO.Directory.GetFiles(CharacterFile.File.dataPath, "*.chr");

            foreach (string file in files)
            {
                CharacterFile.File charFile = new CharacterFile.File(file);
                iconGrid.Add(charFile);
                yield return null;
            }

            iconGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame()
        {
            Game.Controller.sourceFile = selected;
            Overlay.instance.ShowTransparentBlocker();
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
#endif
    }
}