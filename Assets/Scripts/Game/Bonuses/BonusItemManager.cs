using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class BonusItemManager : IllogicGate.SingletonBehaviour<BonusItemManager>
    {
        class InstanceTracker
        {
            public int total { get; private set; }
            public int round { get; private set; }
            public BonusItem active { get; private set; }
            public void Count(BonusItem item)
            {
                total++;
                round++;
                active = item;
            }
            public void RoundReset()
            {
                round = 0;
                active = null;
            }
            public void Clear()
            {
                active = null;
            }
        }

        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called whenever any a item gets awarded </summary>
        public event System.Action<BonusItem> bonusAwarded;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> List of available items </summary>
        List<BonusItem> sourceItems;

        /// <summary> Active playarea </summary>
        PlayArea playArea;

        /// <summary> Current round </summary>
        int round;

        /// <summary> Total number of rounds </summary>
        int totalRounds;

        /// <summary> To keep track of instance by source item </summary>
        Dictionary<BonusItem, InstanceTracker> instances;

        /// <summary> Number of active instances (any kind) </summary>
        int activeInstanceCount;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call this once at the start of the game
        /// </summary>
        /// <param name="totalRounds"></param>
        public void InitializeGame(int totalRounds)
        {
            this.totalRounds = totalRounds;
            sourceItems = new List<BonusItem>(GetComponentsInChildren<BonusItem>());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call to get the bonus items associated with the new play area and initialize 
        /// the new round
        /// Some bonus items won't spawn on a retry play, so we need to know if it's a retry
        /// round or not
        /// </summary>
        public void InitializeRound(PlayArea playArea, int round, bool isRetry)
        {
            this.playArea = playArea;
            this.round = round;
            activeInstanceCount = 0;

            // initialize the counters
            if (round == 0)
            {
                instances = new Dictionary<BonusItem, InstanceTracker>();
                foreach (BonusItem item in sourceItems)
                    instances.Add(item, new InstanceTracker());
            }
            else
            {
                foreach (BonusItem item in sourceItems)
                    instances[item].RoundReset();
            }

            StartCoroutine(SpawnItems(isRetry));
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call after the round ends to clean up remaining items
        /// </summary>
        public void EndRound()
        {
            foreach(InstanceTracker instance in instances.Values)
            {
                if (instance.active != null)
                    Destroy(instance.active.gameObject);
            }
            playArea = null;
            StopAllCoroutines();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator SpawnItems(bool isRetry)
        {
            while (playArea != null)
            {
                // check if we need to spawn new items
                foreach (BonusItem item in sourceItems)
                {
                    if (!CanSpawnCheck(item, isRetry))
                        continue;
                    
                    // if it's paused, wait before spawning
                    while (Controller.instance.isPaused)
                        yield return null;

                    // create instance
                    BonusItem copy = Instantiate(item, playArea.transform, true);
                    copy.awarded += bonusAwarded;
                    copy.awarded += DecreaseInstance;
                    copy.timeout += DecreaseInstance;
                    copy.Activate();

                    instances[item].Count(copy);
                    activeInstanceCount++;

                    // only spawn 1 item per loop
                    break;
                }

                // wait a bit between item spawning
                yield return new WaitForSeconds(5);
            }
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Returns true if the conditions for spawning that type of bonus item are met
        /// </summary>
        bool CanSpawnCheck(BonusItem item, bool isRetry)
        {
            // this item is not available on a retry round
            if (isRetry && !item.canSpawnOnRetryRound)
                return false;

            // regardless of type, we can put so many items in the shadow.
            // The smaller the area, the less items we can put
            float clearedRatio = playArea.mask.clearedTotalRatio;
            // too little space
            if (clearedRatio > 0.9f)
                return false;
            // a quarter space left
            else if (clearedRatio > 0.75f && activeInstanceCount >= 2)
                return false;
            // half space left
            else if (clearedRatio > 0.50f && activeInstanceCount >= 3)
                return false;
            // three quarters space left
            else if (clearedRatio > 0.25f && activeInstanceCount >= 4)
                return false;
            // almost all covered
            else if (activeInstanceCount >= 5)
                return false;

            // we can only have one of each item type instanced at the same time
            if (instances[item].active != null)
                return false;

            // enforce per round limit
            if (instances[item].round == item.maxPerRound)
                return false;

            // enforce per play limit
            if (instances[item].total == item.maxPerGame)
                return false;
            
            // check the chance of spawning
            float chance = item.SpawnChance(playArea.mask.clearedRatio, round, totalRounds);
            if (Random.value > chance)
                return false;

            return true;
        }

        // -----------------------------------------------------------------------------------	
        private void DecreaseInstance(BonusItem item)
        {
            // decrease instance count
            item.awarded -= DecreaseInstance;
            item.timeout -= DecreaseInstance;
            activeInstanceCount--;

            foreach(InstanceTracker instance in instances.Values)
            {
                if (instance.active == item)
                    instance.Clear();
            }
        }
    }
}