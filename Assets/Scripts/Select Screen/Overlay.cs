using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple overlay for the menu screens with popups and 
    /// other functions
    /// </summary>
    public class Overlay : IllogicGate.SingletonBehaviour<Overlay>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Image background = null;

        [SerializeField]
        RawImage baseImageViewer = null;

        [SerializeField]
        Stats statsViewer = null;

        // --- Properties -------------------------------------------------------------------------------
        Color defaultBackgroundColor;

        GameObject selectedObject;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            defaultBackgroundColor = background.color;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            statsViewer.Hide();

            EventSystem.current.SetSelectedGameObject(selectedObject);
            gameObject.SetActive(false);
            baseImageViewer.gameObject.SetActive(false);
            background.color = defaultBackgroundColor;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Displays a transparent overlay to block all 
        /// mouse input
        /// </summary>
        public void ShowTransparentBlocker()
        {
            selectedObject = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);

            background.color = Color.clear;
            gameObject.SetActive(true);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows the base image for a given character and round
        /// </summary>
        public void ShowBaseImage(int round, CharacterFile.File file)
        {
            ShowTransparentBlocker();
            StartCoroutine(ShowBaseImageCoroutine(round, file));
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator ShowBaseImageCoroutine(int round, CharacterFile.File file)
        {
            // fade and load the image (this might take 2 or 3 secs in slow systems)
            yield return StartCoroutine(Transition.instance.Show());
            baseImageViewer.texture = file.LoadRound(round).baseImage;
            baseImageViewer.gameObject.SetActive(true);
            yield return StartCoroutine(Transition.instance.Hide());

            // wait until the user presses fire
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // show stats
            statsViewer.Show(round, file.guid);
            yield return null;

            // wait until the user presses fire
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // fade and go back to menu
            yield return StartCoroutine(Transition.instance.Show());
            baseImageViewer.gameObject.SetActive(false);
            statsViewer.Hide();
            yield return StartCoroutine(Transition.instance.Hide());
            Hide();
        }
    }
}