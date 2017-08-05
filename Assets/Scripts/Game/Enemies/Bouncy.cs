using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
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

        /// <summary> Fixed speed at which the bouncy moves </summary>
        float speed;

        /// <summary> If true, it will always move in a 45 degree angle </summary>
        bool lock45;

        /// <summary> Position in float, as not to accumulate rounding errors from x, y </summary>
        Vector2 position;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Initializes this bouncy. Call during Setup()
        /// </summary>
        protected void Initialize(float speed, bool lock45 = true)
        {
            this.lock45 = lock45;
            position = new Vector2(x, y);
            this.speed = speed;

            if (lock45)
                velocity = Directions[Random.Range(0, 4)] * speed;
            else
                velocity = Random.insideUnitCircle.normalized * speed;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Clamps the velocity so it always bounces at a 45 degree angle
        /// </summary>
        void ClampVelocity45()
        {
            // find the direction that gives out
            // the smallest angle to a 45 degree direction
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

            velocity = Directions[idx] * speed;
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
            Vector2 delta = velocity * dt;

            // starting x and y, pre-edge contact x and y
            Vector2 start = position;
            Vector2 preContact = Vector2.zero;
            for (int i = 0; i < Steps; i++)
            {
                // update position
                preContact = position;
                position = start + delta * i;
                x = Mathf.RoundToInt(position.x);
                y = Mathf.RoundToInt(position.y);

                // tell the physics engine to update colliders (thanks Unity 2017.1!)
                Physics2D.Simulate(0.005f);

                // did it get in contact with the edges?
                int nHits = collider.GetContacts(PlayArea.EdgeContactFilter, contacts);
                if (nHits > 0)
                {
                    // go back to a non-collision position
                    position = preContact;
                    x = Mathf.RoundToInt(position.x);
                    y = Mathf.RoundToInt(position.y);

                    // obtain normal from contact points
                    Vector2 normal = Vector3.zero;
                    for (int j = 0; j < nHits; j++)
                        normal += contacts[j].normal;
                    normal.Normalize();

                    // use the first normal
                    //Vector2 normal = contacts[0].normal;

                    // reflect velocity 
                    //normal = Quaternion.Euler(0, 0, Random.Range(-2, 2) * 5) * normal;
                    velocity = Vector2.Reflect(velocity, normal);

                    // adjust velocity direction and magnitude
                    if (lock45)
                        ClampVelocity45();
                    else
                        velocity = velocity.normalized * speed;

                    // set new start point and do the rest of the steps
                    start = position;
                    delta = velocity * dt;
                }
            }
        }
    }
}