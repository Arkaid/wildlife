using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Wormy : Enemy
    {
        [Serializable]
        struct Settings
        {
            public Config.Difficulty difficulty;
            [Range(0, 2)]
            public int round;
            public float maxSpeed;
            public float maxSteering;
            [Tooltip("Used for the segments, how much history positions to keep. The faster it moves, the shorter it needs to be")]
            public int historyLength;
            public float timeBetweenBugs;
            public int bugCount;
        }

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

        [SerializeField]
        Settings[] settings = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Current settings for this difficulty and round </summary>
        Settings currentSettings;

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

            currentSettings = Array.Find(
                settings,
                s => s.difficulty == Config.instance.difficulty &&
                s.round == Controller.instance.round);

            position = new Vector2(x, y);
            target = position;

            // create an empty list first
            positionHistory = new List<Vector2>();
            rotationHistory = new List<Quaternion>();
            for (int i = 0; i < currentSettings.historyLength; i++)
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
        private void OnKilled(Enemy obj)
        {
            animator.SetTrigger("Die");

            // wait a few seconds to destroy the object
            // to give the animation time to finish
            DestroyObject(gameObject, 10);
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            // Check if we're about to hit path,
            // in which case, steer harder and set a diffent target
            const float LookAhead = Radius * 4;
            float maxSteering = currentSettings.maxSteering;
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
            Vector2 desired = (target - position).normalized * currentSettings.maxSpeed;
            Vector2 steering = desired - velocity;
            steering = Vector2.ClampMagnitude(steering, maxSteering);

            velocity = velocity + steering;
            velocity = Vector2.ClampMagnitude(velocity, currentSettings.maxSpeed);

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
            Bug sourceBug = GetComponentInChildren<Bug>(true);
            YieldInstruction wait = new WaitForSeconds(currentSettings.timeBetweenBugs);

            while (isAlive)
            {
                yield return wait;
                while (subEnemies.Count == currentSettings.bugCount)
                    yield return null;
                if (isAlive)
                {
                    Bug newBug = Instantiate(sourceBug);
                    subEnemies.Add(newBug);

                    newBug.gameObject.SetActive(true);
                    newBug.killed += (Enemy e) => { subEnemies.Remove(e); };

                    newBug.transform.SetParent(transform.parent, true);
                    newBug.transform.position = sourceBug.transform.position;
                    newBug.transform.localScale = Vector3.one;
                    newBug.SetXYFromLocalPosition();
                    newBug.Run();
                }
            }
        }
    }
}