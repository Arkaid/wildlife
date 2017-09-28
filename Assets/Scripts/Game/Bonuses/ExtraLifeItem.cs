using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class ExtraLifeItem : BonusItem
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        static readonly int[] MaxPerGame = new int [] { 2, 1, 1 };
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public override int maxPerGame { get { return MaxPerGame[(int)Config.instance.difficulty]; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void Award()
        {
            Vector3 pos = playArea.MaskPositionToWorld(x, y);
            UI.instance.PlayBonusItemEffect(BonusEffect.Type.ExtraLife, pos);
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
                        case 0: return 0;
                        case 1: return 0.2f * clearedRatio;
                        case 2: return 0.5f * clearedRatio;
                        case 3: return 1f;
                    }
                    break;
            }

            return 0f;
        }
    }
}