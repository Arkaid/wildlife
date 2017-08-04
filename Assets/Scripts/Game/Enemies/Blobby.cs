using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Slimy boss's blobby
    /// </summary>
    public class Blobby : Bouncy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Settings for the current difficulty </summary>
        JSONObject roundSettings;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            playArea.mask.maskCleared += KillIfOutsideShadow;
            killed += OnKilled;

            InitialVelocity(settings["speed"].f);
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy sender)
        {
            playArea.mask.maskCleared -= KillIfOutsideShadow;
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 5); 
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            MoveAndBounce();
        }
    }
}