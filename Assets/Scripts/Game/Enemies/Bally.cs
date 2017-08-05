using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Wormy's bug
    /// </summary>
    public class Bally : Bouncy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            Run();
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            playArea.mask.maskCleared += KillIfOutsideShadow;
            killed += OnKilled;

            //InitialVelocity(settings["speed"].f);
            Initialize(100, false);
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy sender)
        {
            playArea.mask.maskCleared -= KillIfOutsideShadow;
            //animator.SetTrigger("Die");

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