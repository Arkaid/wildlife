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
        static readonly int[] MaxInstancesPerDifficulty = new int[] { 2, 2, 2 };

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
            switch (Config.instance.difficulty)
            {
                case Config.Difficulty.Easy:
                    switch(round)
                    {
                        case 0: return 0.10f; 
                        case 1: return 0.25f;
                        case 2: return 0.25f;
                    }
                    break;
                case Config.Difficulty.Normal:
                    switch(round)
                    {
                        case 0: return 0.05f;
                        case 1: return 0.10f;
                        case 2: 
                        case 3: return 0.05f + 0.35f * clearedRatio;
                    }
                    break;
                case Config.Difficulty.Hard:
                    switch (round)
                    {
                        case 0: return 0.2f * clearedRatio;
                        case 1: return 0.4f * clearedRatio;
                        case 2: return 0.6f * clearedRatio;
                        case 3: return 0.8f * clearedRatio;
                    }
                    break;
            }

            return 0f;
        }
    }
}