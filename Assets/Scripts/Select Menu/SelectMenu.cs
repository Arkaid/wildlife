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
        CharacterIconGrid characterIconGrid;

        [SerializeField]
        CharacterAvatar characterAvatar;

        // --- Properties -------------------------------------------------------------------------------
        string[] characterFiles;

        Dictionary<string, CharacterSheet> characterSheets;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Start()
        {
            characterIconGrid.switched += OnCharacterSwitched;
            characterIconGrid.selected += OnCharacterSelected;
            yield return StartCoroutine(LoadCharacterSheets());
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
            yield return new WaitForSeconds(2);
            Game.characterFile = file;
            SceneManager.LoadScene("Game");
        }
    }
}