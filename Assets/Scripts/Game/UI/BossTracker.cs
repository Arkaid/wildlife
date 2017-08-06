using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(BoxCollider2D))]
    public class BossTracker : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        RectTransform up = null;

        [SerializeField]
        RectTransform down = null;

        [SerializeField]
        RectTransform left = null;

        [SerializeField]
        RectTransform right = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Boss to track </summary>
        Enemy boss = null;

        /// <summary> Collider used to detect whether the boss is in or out of view </summary>
        new BoxCollider2D collider = null;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            collider = GetComponent<BoxCollider2D>();
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Reset()
        {
            boss = null;
            up.gameObject.SetActive(false);
            down.gameObject.SetActive(false);
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
        }
        
        // -----------------------------------------------------------------------------------	
        public void StartTracking(Enemy boss)
        {
            Debug.Assert(boss.isBoss);

            this.boss = boss;
            boss.killed += (Enemy e, bool killedByPlayer) => { this.boss = null; };

            SetupCollider();

            // start tracking
            gameObject.SetActive(true);
            StartCoroutine(TrackCoroutine());

        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Resizes and recenters the collider so it
        /// fits the camera frustrum
        /// </summary>
        public void SetupCollider()
        {
            Camera cam = Camera.main;
            Vector2 size = new Vector2();

            size.y = cam.orthographicSize * 2;
            size.x = size.y * cam.aspect;

            collider.size = size;
            collider.offset = cam.transform.position - transform.position;
        }

        // -----------------------------------------------------------------------------------	
        float SignedAngle(Vector2 from, Vector2 to)
        {
            return (Vector2.SignedAngle(from, to) + 360) % 360;
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator TrackCoroutine()
        {
            // calculate the angles between the
            // center of the screen and each corner
            float w = Screen.width * 0.5f;
            float h = Screen.height * 0.5f;
            Vector2 tr = new Vector2( w,  h);
            Vector2 tl = new Vector2(-w,  h);
            Vector2 br = new Vector2( w, -h);
            Vector2 bl = new Vector2(-w, -h);

            // each angle is measured from the top-right corner
            float tp = SignedAngle(tr, tl);
            float lt = SignedAngle(tr, bl);
            float bt = SignedAngle(tr, br);

            while (boss != null)
            {
                up.gameObject.SetActive(false);
                down.gameObject.SetActive(false);
                left.gameObject.SetActive(false);
                right.gameObject.SetActive(false);

                // no need to track here
                if (collider.IsTouching(boss.collider))
                {
                    yield return null;
                    continue;
                }

                // get boss position in viewport coordinates
                Vector2 bossPos = Camera.main.WorldToViewportPoint(boss.transform.position);
                bossPos.x = Mathf.Clamp01(bossPos.x);
                bossPos.y = Mathf.Clamp01(bossPos.y);

                // calculate angle respect to top-left
                float angle = SignedAngle(tr, boss.transform.position);

                // show correct arrow and place it at boss level
                RectTransform target = null;
                if (angle < tp)
                {
                    bossPos.y = 1;
                    target = up;
                }
                else if (angle < lt)
                {
                    bossPos.x = 0;
                    target = left;
                }
                else if (angle < bt)
                {
                    bossPos.y = 0;
                    target = down;
                }
                else
                {
                    bossPos.x = 1;
                    target = right;
                }

                target.gameObject.SetActive(true);
                target.anchorMin = bossPos;
                target.anchorMax = bossPos;

                yield return null;
            }
        }
    }
}