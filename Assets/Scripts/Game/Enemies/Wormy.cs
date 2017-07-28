using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Wormy : Enemy
    {
        [System.Serializable]
        struct Settings
        {
            public Config.Difficulty difficulty;
            [Range(0, 2)]
            public int round;
            public float maxSpeed;
            public float maxSteering;
            [Tooltip("Used for the segments, how much history positions to keep. The faster it moves, the shorter it needs to be")]
            public int historyLength;
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
                positionHistory.Add(transform.position);
                rotationHistory.Add(Quaternion.identity);
            }

            SetNextTarget();
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
                    SetNextTarget();
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
                SetNextTarget();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Updates the trailing "segments" of the worm
        /// </summary>
        void UpdateSegments()
        {
            // enqueue current position and rotation
            positionHistory.RemoveAt(0);
            positionHistory.Add(transform.position);

            rotationHistory.RemoveAt(0);
            rotationHistory.Add(transform.rotation);

            // calculate "delay" of each part
            int idxJump = (positionHistory.Count - 1) / parts.Length;

            // apply delayed position and rotation
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].position = positionHistory[idxJump * i];
                parts[i].rotation = rotationHistory[idxJump * i];
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Finds the next target to move to
        /// </summary>
        void SetNextTarget()
        {
            Vector2 next = Vector2.zero;
            int nHits = 1;
            int retries = 200;

            // search until a valid zone is found or it fails
            // TODO: when it fails, there is no recovery
            while (nHits > 0 && retries > 0)
            {
                // find a random position in the play area
                next = new Vector2()
                {
                    x = UnityEngine.Random.Range(Radius, PlayArea.imageWidth - Radius),
                    y = UnityEngine.Random.Range(Radius, PlayArea.imageHeight - Radius)
                };

                retries--;

                // target is not in the shadow (cannot move there)
                if (playArea.mask[(int)next.x, (int)next.y] != PlayArea.Shadowed)
                    continue;

                // cast a circle to see if it collides with something
                Vector2 direction = next - position;
                nHits = Physics2D.CircleCast(
                    transform.position, Radius, direction, 
                    PlayArea.EdgeContactFilter, hits, direction.magnitude);
                //Debug.DrawRay(transform.position, direction, Color.red, 5);
            }

            if (retries == 0)
            {
                print("failed");
                print(transform.position);
                print(string.Format("{0}, {1}", x, y));
                Debug.Break();
            }

            target = next;
        }
    }
}