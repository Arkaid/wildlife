using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Tanky : Enemy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        const float Radius = 64;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        float scale = 1;

        /// <summary> Target the worm head is going to move to </summary>
        Vector2 target = Vector2.zero;

        /// <summary> Current velocity </summary>
        Vector2 velocity = Vector2.zero;

        /// <summary> Current position (play area coordinates) </summary>
        Vector2 position = Vector2.zero;

        /// <summary> Do not move while stopped.... DUH </summary>
        bool isStopped = true;

        /// <summary> Used by CircleCast </summary>
        RaycastHit2D[] hits = new RaycastHit2D[8];

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            scale = ScaleBasedOnMaskSize();
            transform.localScale = Vector3.one * scale;
        }
        
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            playArea.mask.maskCleared += OnMaskCleared;
            killed += OnKilled;
            position = new Vector2(x, y);
            target = FindValidTarget(position, Radius * scale);
            StartCoroutine(RotateTowardsTarget());
            StartCoroutine(ShootCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            if (isStopped)
                return;

            float scaledRadius = Radius * scale;

            // Check if we're about to hit path,
            // in which case, stop and find new target
            int nHits = Physics2D.CircleCast(
                transform.position, scaledRadius, velocity, 
                PlayArea.EdgeContactFilter, hits, scaledRadius);
            if (nHits > 0)
            {
                target = FindValidTarget(position, scaledRadius);
                StartCoroutine(RotateTowardsTarget());
                return;
            }

            // move
            position += velocity * Time.deltaTime;
            x = Mathf.RoundToInt(position.x);
            y = Mathf.RoundToInt(position.y);

            // about to reach the target, move to next
            if (Vector2.Distance(target, position) < scaledRadius)
            {
                target = FindValidTarget(position, scaledRadius);
                StartCoroutine(RotateTowardsTarget());
            }
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator ShootCoroutine()
        {
            float shootTime = settings["shoot_time"].f;
            int ballCount = (int)settings["max_cannonballs"].i;
            int ballsPerShot = (int)settings["cannonballs_per_shot"].i;

            YieldInstruction wait = new WaitForSeconds(shootTime);

            while (isAlive)
            {
                while (isAlive)
                {
                    while (subEnemies.Count == ballCount)
                        yield return null;

                    yield return wait;

                    if (isAlive)
                    {
                        for (int i = 0; i < ballsPerShot && subEnemies.Count < ballCount; i++)
                        {
                            animator.SetTrigger("Shoot");
                            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                                yield return null;
                            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                                yield return null;
                        }
                        animator.ResetTrigger("Shoot");
                    }
                }

                yield return null;
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Rotates the tank to face the next target point
        /// </summary>
        /// <returns></returns>
        IEnumerator RotateTowardsTarget()
        {
            isStopped = true;
            velocity = (target - position).normalized;

            // find angle between the current direction and the new one
            Vector3 start = transform.up;
            float angle = Vector2.Angle(start, velocity);

            // calculate rotation time and final rotation
            float time = angle * settings["rotation_speed_angle"].f;
            Quaternion rot = Quaternion.FromToRotation(start, velocity);

            // rotate
            float elapsed = 0;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / time);
                // t = IllogicGate.Tweener.EaseInOut(t, IllogicGate.Tweener.Mode.Sine);
                transform.up = Quaternion.Lerp(Quaternion.identity, rot, t) * start;
                yield return null;
            }

            // apply speed
            velocity *= settings["speed"].f;
            isStopped = false;
        }

        // -----------------------------------------------------------------------------------	
        private void OnKilled(Enemy obj)
        {
            animator.SetTrigger("Die");
            animator.SetBool("Running", false);
            StartCoroutine(FadeOut());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Fades out the tank after dying
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeOut()
        {
            const float FadeStart = 6f;
            const float FadeDuration = 3f;

            yield return new WaitForSeconds(FadeStart);

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

            float elapsed = 0;
            Color color = Color.white;
            while (elapsed < FadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = 1f - Mathf.Clamp01(elapsed / FadeDuration);
                foreach (SpriteRenderer sr in renderers)
                    sr.color = color;
                yield return null;
            }

            DestroyObject(gameObject);
        }
    }
}