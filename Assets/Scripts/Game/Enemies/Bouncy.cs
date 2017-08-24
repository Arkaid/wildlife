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

        /// <summary> repulsion velocity used to move away from walls </summary>
        Vector2 replusion;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // obtain normal from contact points
            Vector2 normal = Vector3.zero;
            foreach (ContactPoint2D cp in collision.contacts)
                normal += cp.normal;
            normal.Normalize();

            // calculate repulsion velocity
            replusion = normal * speed;

            // reflect velocity
            velocity = Vector2.Reflect(velocity, normal).normalized * speed;

            // clamp if necessary
            if (lock45)
                ClampVelocity45();
        }

        // -----------------------------------------------------------------------------------	
        void OnCollisionStay2D(Collision2D collision)
        {
            // obtain normal from contact points
            Vector2 normal = Vector3.zero;
            foreach (ContactPoint2D cp in collision.contacts)
                normal += cp.normal;
            normal.Normalize();

            // calculate repulsion velocity
            replusion = normal * speed;
        }
        
        // -----------------------------------------------------------------------------------	
        void OnCollisionExit2D(Collision2D collision)
        {
            replusion = Vector2.zero;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Initializes bouncy with a given velocity (direction and speed)
        /// </summary>
        public void Initialize(Vector2 velocity, bool lock45 = true)
        {
            this.lock45 = lock45;
            this.velocity = velocity;
            position = new Vector2(x, y);
            speed = velocity.magnitude;

            if (lock45)
                ClampVelocity45();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Initializes this bouncy. Call during Setup()
        /// </summary>
        public void Initialize(float speed, bool lock45 = true)
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
        /// <summary>
        /// Moves the object and bounces it at exactly so it comes out at a 45
        /// degree angle. Optionally, may add a little noise to the bounce
        /// </summary>
        protected void Move()
        {
            // use repulsion velocity if available, otherwise move as planned
            Vector2 vel = replusion != Vector2.zero ? replusion : velocity;

            position += vel * Time.fixedDeltaTime;
            x = Mathf.RoundToInt(position.x);
            y = Mathf.RoundToInt(position.y);
        }
    }
}