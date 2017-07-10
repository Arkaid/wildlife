using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Bouncy : Enemy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        float speed;

        // --- Properties -------------------------------------------------------------------------------
        Vector2 velocity;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        RaycastHit2D[] hits = new RaycastHit2D[8];
        /*void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Safe") && !other.CompareTag("Play Area"))
                return;

            float radius = collider.bounds.extents.magnitude * 1.1f;
            int nHits = Physics2D.CircleCastNonAlloc(transform.position, radius, velocity, hits, radius);
            Vector2 normal = Vector3.zero;
            for (int i = 0; i < nHits; i++)
            {
                if (hits[i].collider != collider)
                    normal += hits[i].normal;
            }
            normal = Quaternion.Euler(0, 0, Random.Range(-5, 5f)) * normal.normalized;
            velocity = Vector2.Reflect(velocity, normal);
        }
        */
        // -----------------------------------------------------------------------------------	


        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            velocity = Random.insideUnitCircle.normalized * speed;
        }

        // -----------------------------------------------------------------------------------	
        protected override void UpdatePosition()
        {
            // first, check if we're going to bounce
            float dt = Time.deltaTime;
            float radius = collider.bounds.extents.magnitude;
            Vector2 from = (Vector2)transform.position + velocity.normalized * radius;
            float distance = speed * dt;

            // cast forward, see if hits any edges
            int nHits = Physics2D.CircleCastNonAlloc(from, radius, velocity, hits, distance, PlayArea.EdgesLayerMask);
            
            // if it does, calculate the dt needed to reach the edge without 
            // going past it
            if (nHits > 0)
                dt = hits[0].distance / speed;

            // update position
            x += Mathf.RoundToInt(velocity.x * dt);
            y += Mathf.RoundToInt(velocity.y * dt);

            // bounce velocity (with a little random deviation)
            if (nHits > 0)
            {
                Vector2 normal = hits[0].normal;
                normal = Quaternion.Euler(0, 0, Random.Range(-10, 10)) * normal;
                velocity = Vector2.Reflect(velocity, normal);
            }
        }
    }
}