using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Wormy's bug
    /// </summary>
    public class Bug : Bouncy
    {
        [System.Serializable]
        struct Settings
        {
            public Config.Difficulty difficulty;
            [Range(0,2)]
            public int round;
            public float speed;
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
                s.round == Controller.instance.round);

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

            // rotate the sprite to face the forward movement
            transform.localRotation = Quaternion.FromToRotation(Vector2.up, velocity);
        }
    }
}