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

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            killed += OnKilled;
            StartCoroutine(FixZDepth());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// The ball spawns under the turret, but when in flight, it should render above it
        /// This waits a little bit before fixing the z coordinate of the ball
        /// </summary>
        /// <returns></returns>
        IEnumerator FixZDepth()
        {
            yield return new WaitForSeconds(1f);
            Vector3 position = transform.localPosition;
            position.z = -1.5f;
            transform.localPosition = position;
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy sender)
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
        }
    }
}