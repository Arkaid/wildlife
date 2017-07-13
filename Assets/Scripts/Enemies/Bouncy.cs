using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public abstract class Bouncy : Enemy
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly Vector2[] Directions = new Vector2[]
        {
            (new Vector2(-1, -1)).normalized,
            (new Vector2( 1, -1)).normalized,
            (new Vector2( 1,  1)).normalized,
            (new Vector2(-1,  1)).normalized,
        };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Velocity vector </summary>
        protected Vector2 velocity { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected void InitialVelocity(float speed)
        {
            // start with a 45 degree angle, no matter what
            velocity = Directions[Random.Range(0, 4)] * speed;
        }

        // -----------------------------------------------------------------------------------	
        ContactPoint2D[] contacts = new ContactPoint2D[8];
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Moves the object and bounces it at exactly so it comes out at a 45
        /// degree angle. Optionally, may add a little noise to the bounce
        /// </summary>
        protected void MoveAndBounce()
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
                Physics2D.Simulate(0.005f);

                // did it get in contact with the edges?
                int nHits = collider.GetContacts(PlayArea.EdgeContactFilter, contacts);
                if (nHits > 0)
                {
                     // go back to a non-collision position
                    x = px;
                    y = py;

                    // obtain normal from contact points
                    Vector2 normal = Vector3.zero;
                    for (int j = 0; j < nHits; j++)
                        normal += contacts[j].normal;
                    normal.Normalize();

                    // reflect velocity 
                    //normal = Quaternion.Euler(0, 0, Random.Range(-2, 2) * 5) * normal;
                    velocity = Vector2.Reflect(velocity, normal);

                    // make sure it's always at a 45 degree angle
                    float smallest = Vector2.Angle(velocity, Directions[0]);
                    int idx = 0;
                    for (int dir = 1; dir < 4; dir++)
                    {
                        float angle = Vector2.Angle(velocity, Directions[dir]);
                        if (smallest > angle)
                        {
                            idx = dir;
                            smallest = angle;
                        }
                    }
                    velocity = Directions[idx] * velocity.magnitude;


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