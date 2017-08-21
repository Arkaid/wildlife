using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Wormy : Enemy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Radius of the sprite / collider, used for avoiding going out of bounds </summary>
        const float Radius = 16;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Transform [] parts = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Target the worm head is going to move to </summary>
        Vector2 target = Vector2.zero;

        /// <summary> Current velocity </summary>
        Vector2 velocity = Vector2.zero;

        /// <summary> Current position (play area coordinates) </summary>
        Vector2 position = Vector2.zero;

        /// <summary> If true, hard steering has been engaged to avoid hitting an edge </summary>
        bool hardSteering = false;

        /// <summary> History of positions, used to trail the segments </summary>
        List<Vector2> positionHistory;

        /// <summary> History of rotations, used to trail the segments </summary>
        List<Quaternion> rotationHistory;

        /// <summary> Used by CircleCast </summary>
        RaycastHit2D[] hits = new RaycastHit2D[8];

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            killed += OnKilled;

            position = new Vector2(x, y);
            target = position;

            // create an empty list first
            positionHistory = new List<Vector2>();
            rotationHistory = new List<Quaternion>();
            for (int i = 0; i < settings["history_length"].i; i++)
            {
                positionHistory.Add(transform.localPosition);
                rotationHistory.Add(Quaternion.identity);
            }

            // reparent the segments to share the same 
            // parent as the head
            foreach (Transform part in parts)
            {
                part.GetComponent<WormySegment>().Run();
                part.SetParent(transform.parent, true);
                part.localPosition = transform.localPosition;
            }

            target = FindValidTarget(position, Radius);
            StartCoroutine(SpawnBugsCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy obj, bool killedByPlayer)
        {
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 10);
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
#if UNITY_EDITOR
            Vector2 worldTarget = playArea.MaskPositionToWorld(target);
            Debug.DrawLine(transform.position, worldTarget, Color.red);
#endif

            // Check if we're about to hit path,
            // in which case, steer harder and set a diffent target
            const float LookAhead = Radius * 4;
            float maxSteering = settings["max_steering"].f;
            float maxSpeed = settings["max_speed"].f;
            int nHits = Physics2D.CircleCast(transform.position, Radius, velocity, PlayArea.EdgeContactFilter, hits, LookAhead);
            if (nHits > 0)
            {
                // hard steering began, switch target
                if (!hardSteering)
                    target = FindValidTarget(position, Radius);

                hardSteering = true;

                // calculate the steering force as a function of distance
                // (the closer to the edge, the harder it steers)
                float distance = Math.Max(hits[0].distance, 0.01f);
                float multiplier = Math.Max(1f / distance, 1);
                maxSteering *= multiplier;
            }
            else
                hardSteering = false;

            // calculate new velocity based on steering

            Vector2 desired = (target - position).normalized * maxSpeed;
            Vector2 steering = desired - velocity;
            steering = Vector2.ClampMagnitude(steering, maxSteering);

            velocity = velocity + steering;
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

            // rotate the sprite to face the forward movement
            transform.localRotation = Quaternion.FromToRotation(Vector2.up, velocity);

            // move the sprite
            position += velocity * Time.deltaTime;
            x = Mathf.RoundToInt(position.x);
            y = Mathf.RoundToInt(position.y);

            UpdateSegments();

            // about to reach the target, move to next
            if (Vector2.Distance(target, position) < Radius)
                target = FindValidTarget(position, Radius);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Updates the trailing "segments" of the worm
        /// </summary>
        void UpdateSegments()
        {
            // enqueue current position and rotation
            positionHistory.RemoveAt(0);
            positionHistory.Add(transform.localPosition);

            rotationHistory.RemoveAt(0);
            rotationHistory.Add(transform.rotation);

            // calculate "delay" of each part
            int idxJump = (positionHistory.Count - 1) / parts.Length;

            // apply delayed position and rotation
            for (int i = 0; i < parts.Length; i++)
            {
                int idx = idxJump * (parts.Length - 1 - i);
                parts[i].localPosition = positionHistory[idx];
                parts[i].rotation = rotationHistory[idx];
            }
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Coroutine in charge of spawning new blobs
        /// </summary>
        IEnumerator SpawnBugsCoroutine()
        {
            float spawnTime = settings["bug_spawn_time"].f;
            int bugCount = (int)settings["bug_max_count"].i;

            Buggy sourceBug = GetComponentInChildren<Buggy>(true);
            YieldInstruction wait = new WaitForSeconds(spawnTime);

            while (isAlive)
            {
                while (minionCount == bugCount)
                    yield return null;

                yield return wait;

                // do nothing while paused
                while (Controller.instance.isPaused)
                    yield return null;

                // don't spawn while frozen
                while (Skill.instance.isFreezeActive)
                    yield return null;

                if (isAlive)
                {
                    Buggy newBug = Instantiate(sourceBug);
                    AddMinion(newBug);

                    newBug.gameObject.SetActive(true);

                    newBug.transform.SetParent(transform.parent, true);
                    newBug.transform.position = sourceBug.transform.position;
                    newBug.transform.localScale = Vector3.one * ScaleBasedOnMaskSize();
                    newBug.SetXYFromLocalPosition();
                    newBug.Run();
                }
            }
        }
    }
}