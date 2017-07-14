using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Slimy boss's blobby
    /// </summary>
    public class Blobby : Bouncy
    {
        [System.Serializable]
        struct Settings
        {
            public Config.Difficulty difficulty;
            [Range(1,3)]
            public int round;
            public float speed;
            public int maxBounces;
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Settings [] settings;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Settings for the current difficulty </summary>
        Settings currentSettings;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            currentSettings = System.Array.Find(
                settings,
                s => s.difficulty == Config.instance.difficulty &&
                s.round == Game.instance.round);

            playArea.mask.maskCleared += KillIfOutsideShadow;
            killed += OnKilled;

            InitialVelocity(currentSettings.speed);
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