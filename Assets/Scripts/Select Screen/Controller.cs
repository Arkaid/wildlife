using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        IconGrid characterIconGrid = null;

        [SerializeField]
        Avatar characterAvatar = null;

        [SerializeField]
        Rounds roundPreviews = null;

        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Start()
        {
            Transition.instance.maskValue = 1;

            // hide the overlay if it was showing
            Overlay.instance.Hide();

            characterIconGrid.switched += OnCharacterSwitched;
            characterIconGrid.selected += OnCharacterSelected;
            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSelected(CharacterFile.File file)
        {
            characterAvatar.SetSelected();
            StartCoroutine(LoadGame(file));
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSwitched(CharacterFile.File file)
        {
            characterAvatar.SetCharacter(file);
            roundPreviews.SetCharacter(file);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            string [] files = System.IO.Directory.GetFiles(CharacterFile.File.dataPath, "*.chr");

            foreach (string file in files)
            {
                CharacterFile.File charFile = new CharacterFile.File(file);
                characterIconGrid.Add(charFile);
                yield return null;
            }

            characterIconGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame(CharacterFile.File file)
        {
            Game.Controller.sourceFile = file;
            Overlay.instance.ShowTransparentBlocker();
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
    }
}