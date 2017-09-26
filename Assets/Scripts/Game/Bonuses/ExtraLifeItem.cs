using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class ExtraLifeItem : BonusItem
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Per game-play, how many extra lives to offer </summary>
        static readonly int[] MaxInstancesPerDifficulty = new int[] { 2, 1, 1 };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public override float maxTotalInstanceCount { get { return MaxInstancesPerDifficulty[(int)Config.instance.difficulty]; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Vector3 pos = playArea.MaskPositionToWorld(x, y);
            playArea.effects.ShowExtraLife(pos);
            Destroy(gameObject);
        }

        // -----------------------------------------------------------------------------------	
        public override float SpawnChance(float clearedRatio, int round, int totalRounds)
        {
            float progress = (round + clearedRatio) / totalRounds;

            if (progress < 0.5f)
                return progress * 2;
            else
                return 1f - (progress - 0.5f) * 2f;
        }
    }
}