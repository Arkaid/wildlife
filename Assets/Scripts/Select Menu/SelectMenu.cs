using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class SelectMenu : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        CharacterIconGrid characterIconGrid = null;

        [SerializeField]
        CharacterAvatar characterAvatar = null;

        [SerializeField]
        RoundPreviews roundPreviews = null;

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
        private void OnCharacterSelected(CharacterDataFile file)
        {
            characterAvatar.SetSelected();
            StartCoroutine(LoadGame(file));
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSwitched(CharacterDataFile file)
        {
            characterAvatar.SetCharacter(file);
            roundPreviews.SetCharacter(file);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            string [] files = System.IO.Directory.GetFiles(CharacterDataFile.dataPath, "*.chr");

            foreach (string file in files)
            {
                CharacterDataFile charFile = new CharacterDataFile(file);
                characterIconGrid.Add(charFile);
                yield return null;
            }

            characterIconGrid.SelectFirst();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame(CharacterDataFile file)
        {
            Game.source = file;
            Overlay.instance.ShowTransparentBlocker();
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
    }
}