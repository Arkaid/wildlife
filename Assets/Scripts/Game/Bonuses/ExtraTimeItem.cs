using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class ExtraTimeItem : BonusItem
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly int[] BonusTime = new int[] { 90, 60, 45 };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public int time { get { return BonusTime[(int)Config.instance.difficulty]; } }

        public override int maxPerGame { get { return 4; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public override float SpawnChance(float clearedRatio, int round, int totalRounds)
        {
            switch (Config.instance.difficulty)
            {
                case Config.Difficulty.Easy:
                    return 0.50f;
                case Config.Difficulty.Normal:
                    return 0.25f;
                case Config.Difficulty.Hard:
                    return 0.10f;
            }

            return 0f;
        }

        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Vector3 pos = playArea.MaskPositionToWorld(x, y);
            UI.instance.PlayBonusItemEffect(BonusEffect.Type.ExtraTime, pos);
            Destroy(gameObject);
        }
    }
}