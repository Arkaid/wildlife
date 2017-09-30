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
        public override int maxPerGame { get { return 2; } } // the same letter may be repeated in another round

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
                    if (round == 0)  // no letters in first round
                        return 0;
                    return 0.01f;
                case Config.Difficulty.Normal:
                    if (round == 0) // no letters in first round
                        return 0;
                    return 0.02f;
                case Config.Difficulty.Hard:
                    return 0.03f;
            }
            return 0;
        }

        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Data.UnlockState unlockState = Data.SaveFile.instance.unlockState;
            Vector3 pos = playArea.MaskPositionToWorld(x, y);

            // play effects (awards in controller)
            if (unlockState[letter])
                playArea.effects.ShowScore(Config.instance.unlockLetterScore, pos);

            else
            {
                switch (letter)
                {
                    case UNLOCK.U: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_U, pos); break;
                    case UNLOCK.N: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_N, pos); break;
                    case UNLOCK.L: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_L, pos); break;
                    case UNLOCK.O: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_O, pos); break;
                    case UNLOCK.C: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_C, pos); break;
                    case UNLOCK.K: UI.instance.PlayBonusItemEffect(BonusEffect.Type.Letter_K, pos); break;
                }
            }
            
            Destroy(gameObject);
        }
    }
}