using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Wormy's bug
    /// </summary>
    public class Buggy : Bouncy
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

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            killed += OnKilled;
            Initialize(settings["speed"].f);
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy sender, bool killedByPlayer)
        {
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 5); 
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            MoveAndBounce();

            // rotate the sprite to face the forward movement
            transform.localRotation = Quaternion.FromToRotation(Vector2.up, velocity);
        }
    }
}