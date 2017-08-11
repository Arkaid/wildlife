using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Avatar : MonoBehaviour
    {
        [System.Serializable]
        struct RandomCharacter
        {
            public Sprite avatar;
            public Sprite name;
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image characterImage = null;

        [SerializeField]
        Image nameImage= null;

        [SerializeField]
        Animation nameAnimation = null;

        [SerializeField]
        RandomCharacter randomCharacter;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Character we're currently showing </summary>
        CharacterFile.File characterFile;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------	
        public void SetCharacter(CharacterFile.File file)
        {
            characterFile = file;
            characterImage.sprite = file == null ? randomCharacter.avatar : file.baseSheet.avatarA;
            nameImage.sprite = file == null ? randomCharacter.name : file.baseSheet.name;

            nameAnimation.Stop();
            nameAnimation.Play();
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Switches to the second avatar image
        /// </summary>
        public void SwitchImage()
        {
            if (characterFile == null)
                return;

            if (characterImage.sprite == characterFile.baseSheet.avatarB)
                characterImage.sprite = characterFile.baseSheet.avatarA;
            else
                characterImage.sprite = characterFile.baseSheet.avatarB;
        }
    }
}