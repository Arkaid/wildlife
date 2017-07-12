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

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Setup()
        {
            velocity = Random.insideUnitCircle.normalized * speed;
        }

        // -----------------------------------------------------------------------------------	
        RaycastHit2D[] hits = new RaycastHit2D[1];
        protected override void UpdatePosition()
        {
            // number of interpolation steps for the movement
            const int Steps = 10;

            // delta time, delta x and y
            float dt = Time.deltaTime / Steps;
            float dx = velocity.x * dt;
            float dy = velocity.y * dt;

            // starting x and y, pre-edge contact x and y
            int sx = x; int sy = y;
            int px, py;
            for (int i = 0; i < Steps; i++)
            {
                // update position
                px = x;
                py = y;
                x = Mathf.RoundToInt(sx + dx * i);
                y = Mathf.RoundToInt(sy + dy * i);

                // tell the physics engine to update colliders (thanks Unity 2017.1!)
                Physics2D.Simulate(dt);

                // did it get in contact with the edges?
                if (collider.IsTouchingLayers(PlayArea.EdgesLayerMask))
                {
                    // go back to a non-collision position
                    x = px;
                    y = py;
                    
                    // increasingly make circles until a hit point is found
                    int radius = 0;
                    while (Physics2D.CircleCastNonAlloc(
                        transform.position, radius,
                        velocity, hits, 1,
                        PlayArea.EdgesLayerMask) == 0)
                        radius++;

                    // reflect velocity with a bit of noise
                    Vector2 normal = hits[0].normal;
                    normal = Quaternion.Euler(0, 0, Random.Range(-10, 10)) * normal;
                    velocity = Vector2.Reflect(velocity, normal);

                    // set new start point and do the rest of the steps
                    sx = x;
                    sy = y;
                    dx = velocity.x * dt;
                    dy = velocity.y * dt;
                }
            }


        }
    }
}