using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Collider2D))]
    public abstract class Enemy : PlayAreaObject
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Raised when the blobby dies </summary>
        public event System.Action<Enemy> killed = null;

        // --- Constants --------------------------------------------------------------------------------

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

        protected List<Enemy> subEnemies { get; private set; }

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
        public void Run()
        {
            subEnemies = new List<Enemy>();
            StartCoroutine(RunCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Stops the enemy
        /// </summary>
        public void Kill()
        {
            if (killed != null)
                killed(this);

            isAlive = false;

            // kill all sub enemies
            // (use a copy, since kill tends to remove enemies from the sublist)
            foreach (Enemy enemy in subEnemies.ToArray())
                enemy.Kill();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator RunCoroutine()
        {
            isAlive = true;
            Setup();
            while (isAlive)
            {
                UpdatePosition();
                CheckPlayerHit();
                yield return null;
            }
            Finish();
        }
        
        // -----------------------------------------------------------------------------------	
        void CheckPlayerHit()
        {
            Physics2D.Simulate(0.005f);
            if (collider.IsTouching(playArea.cutPath.collider))
                playArea.player.Hit();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks if the enemy fell outside the shadow. This doesn't happen for
        /// bosses, but it does happen for non-boss enemies that get cut out.
        /// It "kills" the enemy, if that is the case
        /// </summary>
        protected void KillIfOutsideShadow()
        {
            if (playArea.mask[x, y] != PlayArea.Shadowed)
                Kill();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Sets the position of the boss at the start of the game
        /// </summary>
        /// <param name="playerInitalSquare">Needed to know where NOT to start</param>
        public void SetBossStartPosition(IntRect playerInitalSquare)
        {
            // object must be active for this to work...
            Debug.Assert(gameObject.activeInHierarchy);

            // object must be a boss
            Debug.Assert(isBoss);

            // create a random position within the play
            // area that 
            // a) is outside the initial square and
            // b) does not overlap borders or path
            Bounds bounds = collider.bounds;
            while (true)
            {
                Vector2 test = new Vector2();
                test.x = Random.Range(bounds.extents.x, PlayArea.imageWidth - bounds.extents.x);
                test.y = Random.Range(bounds.extents.y, PlayArea.imageHeight - bounds.extents.y);
                
                if (playerInitalSquare.Contains(test))
                    continue;

                x = (int)test.x;
                y = (int)test.y;

                Physics2D.Simulate(0.005f);
                if (collider.IsTouchingLayers(PlayArea.EdgesLayerMask))
                    continue;

                break;
            }
        }
    }
}