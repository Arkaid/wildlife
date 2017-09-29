using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Jintori.Common.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundImageViewer : IllogicGate.SingletonBehaviour<RoundImageViewer>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        RawImage landscape;

        [SerializeField]
        RawImage portrait;

        [SerializeField]
        GameObject localBackground;

        [SerializeField]
        Text artist;

        // --- Properties -------------------------------------------------------------------------------
        Stats stats;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Show(CharacterFile.File characterFile, int round)
        {
            stats = GetComponentInChildren<Stats>(true);
            stats.Hide();
            landscape.enabled = false;
            portrait.enabled = false;
            localBackground.SetActive(false);
            artist.text = "";

            gameObject.SetActive(true);
            StartCoroutine(ShowCoroutine(characterFile, round));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ShowCoroutine(CharacterFile.File characterFile, int round)
        {
            // deselect everything while showing the image
            GameObject prevSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);

            RawImage target = characterFile.IsPortrait(round) ? portrait : landscape;

            // transition to black, load the images
            yield return StartCoroutine(Transition.instance.Show());
            CharacterFile.RoundImages images = characterFile.LoadRound(round);
            target.texture = images.baseImage;
            target.enabled = true;
            artist.text = "ARTIST:" + characterFile.artist.ToUpper();
            localBackground.SetActive(true);
            yield return StartCoroutine(Transition.instance.Hide());

            // wait
            yield return null;
            while (!Input.GetButtonDown("Cut"))
                yield return null;

            // show stats
            SoundManager.instance.PlaySFX("ui_sweep_in");
            stats.Show(round, characterFile.guid);

            // wait
            yield return null;
            while (!Input.GetButtonDown("Cut"))
                yield return null;

            // transition out, hide the stats and images
            yield return StartCoroutine(Transition.instance.Show());
            stats.Hide();
            localBackground.SetActive(false);
            artist.text = "";
            target.enabled = false;
            yield return StartCoroutine(Transition.instance.Hide());

            // disable the game object
            gameObject.SetActive(false);

            // restore selection
            EventSystem.current.SetSelectedGameObject(prevSelection);

        }

    }
}