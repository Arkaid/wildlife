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
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
        }

        public static readonly int[] RoundTime = new int[]
        {
            300, 
            180, 
            120,
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