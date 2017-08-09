using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Avatar : MonoBehaviour
    {
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
            characterImage.sprite = file.baseSheet.avatarA;
            nameImage.sprite = file.baseSheet.name;

            nameAnimation.Stop();
            nameAnimation.Play();
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Switches to the second avatar image
        /// </summary>
        public void SwitchImage()
        {
            characterImage.sprite = characterFile.baseSheet.avatarB;
        }
    }
}