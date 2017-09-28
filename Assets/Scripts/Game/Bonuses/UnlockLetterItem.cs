using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class UnlockLetterItem : BonusItem
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        // --- MonoBehaviour ----------------------------------------------------------------------------
        [SerializeField]
        UNLOCK _letter;
        public UNLOCK letter { get { return _letter; } }

        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public override float SpawnChance(float clearedRatio, int round, int totalRounds)
        {
            switch (Config.instance.difficulty)
            {
                case Config.Difficulty.Easy:
                    return 0.01f;
                case Config.Difficulty.Normal:
                    return 0.02f;
                case Config.Difficulty.Hard:
                    return 0.03f;
            }
            return 0;
        }

        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Vector3 pos = playArea.MaskPositionToWorld(x, y);
            switch(letter)
            {
                case UNLOCK.U: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_U, pos); break;
                case UNLOCK.N: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_N, pos); break;
                case UNLOCK.L: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_L, pos); break;
                case UNLOCK.O: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_O, pos); break;
                case UNLOCK.C: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_C, pos); break;
                case UNLOCK.K: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_K, pos); break;
            }
            
            Destroy(gameObject);
        }
    }
}