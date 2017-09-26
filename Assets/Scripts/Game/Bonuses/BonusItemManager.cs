using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class BonusItemManager : IllogicGate.SingletonBehaviour<BonusItemManager>
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called whenever any a item gets awarded </summary>
        public event System.Action<BonusItem> bonusAwarded;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> List of available items for the active playarea </summary>
        List<BonusItem> bonusItems;

        /// <summary> Active playarea </summary>
        PlayArea playArea;

        /// <summary> Current round </summary>
        int round;

        /// <summary> Total number of rounds </summary>
        int totalRounds;

        /// <summary> To keep track of instance by item type </summary>
        Dictionary<System.Type, int> instanceCount;

        /// <summary> To keep track of instance by item type (overall) </summary>
        Dictionary<System.Type, int> totalInstanceCount = new Dictionary<System.Type, int>();

        /// <summary> List of active instances </summary>
        List<BonusItem> activeInstances;

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
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call to get the bonus items associated with the new play area and initialize 
        /// the new round
        /// </summary>
        public void InitializeRound(PlayArea playArea, int round)
        {
            if (this.playArea != null)
                this.playArea.mask.maskCleared -= OnMaskCleared;

            this.playArea = playArea;
            this.round = round;

            playArea.mask.maskCleared += OnMaskCleared;

            instanceCount = new Dictionary<System.Type, int>();
            bonusItems = new List<BonusItem>(playArea.GetComponentsInChildren<BonusItem>());
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call after the round ends to clean up remaining items
        /// </summary>
        public void EndRound()
        {
            
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared(Point obj)
        {
            // check if we need to spawn new items
            foreach (BonusItem item in bonusItems)
            {
                // make sure we don't go over the max instance count
                System.Type type = item.GetType();
                if (!instanceCount.ContainsKey(type))
                    instanceCount[type] = 0;

                if (instanceCount[type] >= item.maxSimultaneousInstanceCount)
                    continue;

                // or the total instance count
                if (!totalInstanceCount.ContainsKey(type))
                    totalInstanceCount[type] = 0;

                if (totalInstanceCount[type] >= item.maxTotalInstanceCount)
                    continue;

                // check the chance of spawnning
                float chance = item.SpawnChance(playArea.mask.clearedRatio, round, totalRounds);
                print(chance);
                if (Random.value > chance)
                    continue;

                print("INSTANCED");

                // create instance
                instanceCount[type] = instanceCount[type] + 1;
                totalInstanceCount[type] = totalInstanceCount[type] + 1;

                BonusItem copy = Instantiate(item, item.transform.parent, true);
                activeInstances.Add(copy);
                copy.awarded += OnBonusAwarded;
                copy.Activate();
            }
        }

        // -----------------------------------------------------------------------------------	
        private void OnBonusAwarded(BonusItem item)
        {
            // decrease instance count
            item.awarded -= OnBonusAwarded;
            instanceCount[item.GetType()]--;

            activeInstances.Remove(item);

            // raise event
            if (bonusAwarded != null)
                bonusAwarded(item);
        }
    }
}