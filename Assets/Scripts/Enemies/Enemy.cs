using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Collider2D))]
    public abstract class Enemy : PlayAreaObject
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public enum Difficulty
        {
            Easy_1,
            Easy_2,
            Easy_3,
            Medium_1,
            Medium_2,
            Medium_3,
            Hard_1,
            Hard_2,
            Hard_3,
        }
        


        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        bool _isBoss = false;
        public bool isBoss { get { return _isBoss; } }

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> True if the enemy is active on the play area </summary>
        protected bool isAlive;

        /// <summary> Collider for the enemy </summary>
        protected new Collider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }
        private Collider2D _collider;

        /// <summary> Difficulty set upon starting the level </summary>
        public Difficulty difficulty { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Does some basic initalization (changes per enemy type)
        /// </summary>
        protected virtual void Setup() { }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Main update routine. Steers the boss through the shadow
        /// (changes per enemy type)
        /// </summary>
        protected abstract void UpdatePosition();

        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Do some ending for the enemy (animation, whatever)
        /// </summary>
        protected virtual void Finish() { }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Runs the enemy
        /// </summary>
        public void Run(Difficulty difficulty)
        {
            this.difficulty = difficulty;
            StartCoroutine(RunCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Stops the enemy
        /// </summary>
        public void Kill()
        {
            isAlive = false;
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator RunCoroutine()
        {
            isAlive = true;
            Setup();
            while (isAlive)
            {
                UpdatePosition();
                yield return null;
            }
            Finish();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Sets the position of the boss at the start of the game
        /// </summary>
        /// <param name="playerInitalSquare">Needed to know where NOT to start</param>
        public void SetStartPosition(IntRect playerInitalSquare)
        {
            // object must be active for this to work...
            Debug.Assert(gameObject.activeInHierarchy);

            // create a random position within the play
            // area that 
            // a) is outside the initial square and
            // b) does not overlap borders or path
            Bounds bounds = collider.bounds;
            while (true)
            {
                Vector2 test = new Vector2();
                test.x = Random.Range(bounds.extents.x, PlayArea.ImageWidth - bounds.extents.x);
                test.y = Random.Range(bounds.extents.y, PlayArea.ImageHeight - bounds.extents.y);
                
                if (playerInitalSquare.Contains(test))
                    continue;

                x = (int)test.x;
                y = (int)test.y;

                Physics2D.Simulate(0.05f);
                if (collider.IsTouchingLayers(PlayArea.EdgesLayerMask))
                    continue;

                break;
            }
        }
    }
}