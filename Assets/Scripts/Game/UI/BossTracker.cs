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
        Transform up = null;

        [SerializeField]
        Transform down = null;

        [SerializeField]
        Transform left = null;

        [SerializeField]
        Transform right = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Boss to track </summary>
        Enemy boss = null;

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
            boss.killed += (Enemy e) => { this.boss = null; };

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
        IEnumerator TrackCoroutine()
        {
            while (boss != null)
            {
                yield return null;

                // no need to track here
                if (collider.IsTouching(boss.collider))
                    continue;

                // which side should we turn on?
                print(boss.transform.position);
            }
        }
    }
}