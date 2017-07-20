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
            [Range(0, 2)]
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
        /// <summary> Settings for the current difficulty level and round </summary>
        Settings currentSettings;

        /// <summary> Blobby object we use to copy and spawn new instances </summary>
        Blobby sourceBlobby;

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

            sourceBlobby = GetComponentInChildren<Blobby>(true);

            playArea.mask.maskCleared += OnMaskCleared;
            killed += OnKilled;

            InitialVelocity(currentSettings.speed);

            StartCoroutine(SpawnBlobsCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
                MoveAndBounce();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Coroutine in charge of spawning new blobs
        /// </summary>
        IEnumerator SpawnBlobsCoroutine()
        {
            YieldInstruction wait = new WaitForSeconds(currentSettings.timeBetweenBlobs);
            while(isAlive)
            {
                yield return wait;
                while (subEnemies.Count == currentSettings.blobCount)
                    yield return null;
                if (isAlive)
                    animator.SetTrigger("Spawn Blob");
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called from the animation as an event, at the moment the blob must be spawned
        /// </summary>
        void AnimationCallback_SpawnBlob()
        {
            Blobby newBlobby = Instantiate(sourceBlobby);
            subEnemies.Add(newBlobby);

            newBlobby.gameObject.SetActive(true);
            newBlobby.killed += (Enemy e) => { subEnemies.Remove(e); };

            newBlobby.transform.SetParent(transform.parent, true);
            newBlobby.transform.position = sourceBlobby.transform.position;
            newBlobby.transform.localScale = Vector3.one;
            newBlobby.SetXYFromLocalPosition();
            newBlobby.Run();
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy obj)
        {
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 10);
        }
        
    }
}