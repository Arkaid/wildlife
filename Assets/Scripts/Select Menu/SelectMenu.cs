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

        // --- Properties -------------------------------------------------------------------------------
        string[] characterFiles;

        Dictionary<string, CharacterSheet> characterSheets;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Start()
        {
            Transition.instance.maskValue = 1;

            characterIconGrid.switched += OnCharacterSwitched;
            characterIconGrid.selected += OnCharacterSelected;
            yield return StartCoroutine(LoadCharacterSheets());
            yield return StartCoroutine(Transition.instance.Hide());
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSelected(string file)
        {
            characterAvatar.SetSelected();
            StartCoroutine(LoadGame(file));
        }

        // -----------------------------------------------------------------------------------	
        private void OnCharacterSwitched(string file)
        {
            characterAvatar.SetCharacter(characterSheets[file]);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadCharacterSheets()
        {
            characterFiles = System.IO.Directory.GetFiles(CharacterDataFile.dataPath, "*.chr");
            characterSheets = new Dictionary<string, CharacterSheet>();

            foreach (string file in characterFiles)
            {
                CharacterSheet sheet = CharacterDataFile.LoadCharacterSheet(file);
                characterIconGrid.Add(sheet.icon, file);
                characterSheets.Add(file, sheet);
                yield return null;
            }
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator LoadGame(string file)
        {
            Game.characterFile = file;
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(Transition.instance.Show());
            SceneManager.LoadScene("Game");
        }
    }
}