using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Slimy : Bouncy
    {
        [System.Serializable]
        struct Settings
        {
            public Config.Difficulty difficulty;
            [Range(1, 3)]
            public int round;
            public float speed;
            public int blobCount;
            public float timeBetweenBlobs;
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
        Settings currentSettings;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            // resizes the boss to a smaller 
            // size as you take away parts of the shadow
            const float MinSize = 0.4f;
            const float MaxSize = 1.0f;
            const float MinRatio = 0.25f; // start getting small here
            const float MaxRatio = 0.75f; // stop getting small here

            float t = Mathf.Clamp(playArea.mask.clearedRatio, MinRatio, MaxRatio);
            t = (t - MinRatio) / (MaxRatio - MinRatio);
            float size = Mathf.Lerp(MaxSize, MinSize, t);
            transform.localScale = Vector3.one * size;
        }

        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            currentSettings = System.Array.Find(
                settings, 
                s => s.difficulty == Config.instance.difficulty && 
                s.round == Game.instance.round);

            playArea.mask.maskCleared += OnMaskCleared;
            InitialVelocity(currentSettings.speed);
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            MoveAndBounce();
        }
    }
}