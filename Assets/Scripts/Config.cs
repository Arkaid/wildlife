using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Config : IllogicGate.Singleton<Config>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public const int Rounds = 3;

        /// <summary> Levels of overall game difficulty </summary>
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
        }

        /// <summary> Time needed to clear rounds in each difficulty </summary>
        public static readonly int[] RoundTime = new int[]
        {
            300, 
            180, 
            120,
        };

        /// <summary> Amount of percentage (ratio) needed to clear in order to finish the round, per difficulty </summary>
        public static readonly float[] ClearRatio = new float[]
        {
            0.10f,
            0.90f,
            0.95f
        };

        /// <summary> Starting amount of lives for each difficulty </summary>
        public static readonly int[] Lives = new int[]
        {
            4,
            3,
            3
        };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Game difficulty </summary>
        public Difficulty difficulty { get; private set; }

        /// <summary> Time for one round, adjusted for difficulty </summary>
        public int roundTime { get { return RoundTime[(int)difficulty]; } }

        /// <summary> Needed clear ratio to win, adjusted for difficulty </summary>
        public float clearRatio { get { return ClearRatio[(int)difficulty]; } }

        /// <summary> Needed clear percentage to win, adjusted for difficulty </summary>
        public int clearPercentage { get { return Mathf.FloorToInt(clearRatio * 100); } }

        /// <summary> Starting amount of lives, adjusted for difficulty </summary>
        public int lives { get { return Lives[(int)difficulty]; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void OnInstanceCreated()
        {
            difficulty = Difficulty.Easy;
        }
    }
}