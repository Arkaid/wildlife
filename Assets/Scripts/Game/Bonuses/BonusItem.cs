using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public abstract class BonusItem : PlayAreaObject
    {
        // --- Events -----------------------------------------------------------------------------------
        public event System.Action<BonusItem> awarded;

        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Time the item stays active on the play area (easy, normal, hard) </summary>
        static readonly float[] DefaultActiveTime = new float[] { 60, 45, 30 };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Time the item stays active on the play area, adjusted for difficulty </summary>
        virtual protected float activeTime { get { return DefaultActiveTime[(int)Config.instance.difficulty]; } }

        /// <summary> Maximum number of instances that may be in the play area simultaneously </summary>
        virtual public float maxSimultaneousInstanceCount { get { return 1; } }

        /// <summary> Maximum number of instances that may be allowed per character / game-set </summary>
        virtual public float maxTotalInstanceCount { get { return 1; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
                Activate();
            if (Input.GetKeyDown(KeyCode.F6))
                Award();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call to activate the item into the play area
        /// </summary>
        public void Activate()
        {
            PlaceRandomly();
            playArea.mask.maskCleared += OnMaskCleared;
            StartCoroutine(Countdown());
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Destroys the object after a given time
        /// </summary>
        IEnumerator Countdown()
        {
            float time = activeTime;

            // flash 10 seconds (at the most)
            float flashTime = Mathf.Min(time, 10);

            // wait until flashing begins
            float wait = time - flashTime;
            if (wait > 0)
                yield return new WaitForSeconds(wait);

            // flash, then destroy object
            IEnumerator flashCoroutine = Flash();
            StartCoroutine(flashCoroutine);
            yield return new WaitForSeconds(flashTime);
            StopCoroutine(flashCoroutine);

            playArea.mask.maskCleared -= OnMaskCleared;

            Destroy(gameObject);
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator Flash()
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            YieldInstruction wait = new WaitForSeconds(0.25f);
            while (true)
            {
                foreach (SpriteRenderer rend in renderers)
                    rend.enabled = !rend.enabled;
                yield return wait;
            }
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared(Point obj)
        {
            if (IsInShadow())
                return;
            playArea.mask.maskCleared -= OnMaskCleared;
            Award();

            if (awarded != null)
                awarded(this);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called when the player gets the item
        /// </summary>
        protected abstract void Award();

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called to check the chance of spawning in game. Depends on how much you've cleared
        /// and which round is it. return a value between 0 and 1
        /// </summary>
        public abstract float SpawnChance(float clearedRatio, int round, int totalRounds);

    }
}