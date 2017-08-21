using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Slimy : Bouncy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Blobby object we use to copy and spawn new instances </summary>
        Blobby sourceBlobby;

        /// <summary> Used to wait until the spawn of one blobby is finished, before starting the next </summary>
        bool spawnFinished = true;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared(Point center)
        {
            transform.localScale = Vector3.one * ScaleBasedOnMaskSize();
        }

        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            sourceBlobby = GetComponentInChildren<Blobby>(true);

            playArea.mask.maskCleared += OnMaskCleared;
            killed += OnKilled;

            Initialize(settings["speed"].f);

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
            float spawnTime = settings["blob_spawn_time"].f;
            int blobCount = (int)settings["blob_max_count"].i;

            YieldInstruction wait = new WaitForSeconds(spawnTime);
            while(isAlive)
            {
                while(!spawnFinished)
                    yield return null;

                while (minionCount == blobCount)
                    yield return null;

                yield return wait;

                // don't spawn while frozen
                while (Skill.instance.isFreezeActive)
                    yield return null;

                if (isAlive)
                {
                    spawnFinished = false;
                    animator.SetTrigger("Spawn Blob");
                }
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called from the animation as an event, at the moment the blob must be spawned
        /// </summary>
        void AnimationCallback_SpawnBlob()
        {
            Blobby newBlobby = Instantiate(sourceBlobby);
            AddMinion(newBlobby);

            newBlobby.gameObject.SetActive(true);

            newBlobby.transform.SetParent(transform.parent, true);
            newBlobby.transform.position = sourceBlobby.transform.position;
            newBlobby.transform.localScale = Vector3.one;
            newBlobby.SetXYFromLocalPosition();
            newBlobby.Run();

            spawnFinished = true;
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy obj, bool killedByPlayer)
        {
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 10);
        }
        
    }
}