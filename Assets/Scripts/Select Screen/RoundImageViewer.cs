using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundImageViewer : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        
        // --- Properties -------------------------------------------------------------------------------
        RawImage rawImage;

        Stats stats;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Show(CharacterFile.File characterFile, int round)
        {
            rawImage = GetComponent<RawImage>();
            stats = GetComponentInChildren<Stats>(true);
            stats.Hide();
            rawImage.enabled = false;
            Overlay.instance.background.Show(Color.clear);

            gameObject.SetActive(true);
            StartCoroutine(ShowCoroutine(characterFile, round));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ShowCoroutine(CharacterFile.File characterFile, int round)
        {
            // deselect everything while showing the image
            GameObject prevSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);

            // transition to black, load the images
            yield return StartCoroutine(Transition.instance.Show());
            CharacterFile.RoundImages images = characterFile.LoadRound(round);
            rawImage.texture = images.baseImage;
            rawImage.enabled = true;
            Overlay.instance.background.Show();
            yield return StartCoroutine(Transition.instance.Hide());

            // wait
            yield return null;
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // show stats
            stats.Show(round, characterFile.guid);

            // wait
            yield return null;
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // transition out, hide the stats and images
            yield return StartCoroutine(Transition.instance.Show());
            Overlay.instance.background.Hide();
            stats.Hide();
            rawImage.enabled = false;
            yield return StartCoroutine(Transition.instance.Hide());

            // disable the game object
            gameObject.SetActive(false);

            // restore selection
            EventSystem.current.SetSelectedGameObject(prevSelection);

        }

    }
}