using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class RoundIcon : Button
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        Image previewIcon { get { return transform.Find("Mask/Preview Icon").GetComponent<Image>(); } }
        GameObject lockIcon { get { return transform.Find("Mask/Lock").gameObject; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows the icon and hides the lock
        /// </summary>
        public void SetUnlocked(Sprite icon)
        {
            previewIcon.sprite = icon;
            previewIcon.gameObject.SetActive(true);
            lockIcon.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows the lock and hides the icon
        /// </summary>
        public void SetLocked()
        {
            previewIcon.gameObject.SetActive(false);
            lockIcon.SetActive(true);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Plays the animation unlocking
        /// </summary>
        public void AnimateUnlock(Sprite icon)
        {
            previewIcon.sprite = icon;
            SetLocked();
            StartCoroutine(AnimateCoroutine());
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator AnimateCoroutine()
        {
            Animator animator = GetComponent<Animator>();
            animator.enabled = true;
            while (animator.enabled)
                yield return null;
        }

        // -----------------------------------------------------------------------------------	
        void AnimationEvent_SwapImages()
        {
            previewIcon.gameObject.SetActive(true);
            lockIcon.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        void AnimationEvent_AnimationEnded()
        {
            animator.enabled = false;
        }
    }
}