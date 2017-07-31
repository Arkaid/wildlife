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
        /// <summary> Number of rounds to play </summary>
        public const int Rounds = 3;

        /// <summary> Levels of overall game difficulty </summary>
        public enum Difficulty
        {
            Easy,
            Normal,
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
            0.80f,
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

        /// <summary> Speed multiplier for the speed skill, per difficulty </summary>
        public static readonly float[] SpeedSkillMultiplier = new float[]
        {
            2f,
            1.75f,
            1.5f,
        };

        /// <summary> Calculates score </summary>
        public static class Score
        {
            /// <summary> Tuple to match percentage to pointage </summary>
            struct Tuple
            {
                public Tuple(float percentage, float points)
                {
                    this.percentage = percentage;
                    this.points = points;
                }

                public float percentage;
                public float points;
            }

            /// <summary>
            /// How much points to assign per 
            /// cleared percentage in one move
            /// { up to this %, assign these many points per 1% }
            /// </summary>
            static readonly Dictionary<Difficulty, List<Tuple>> ScoreChart = new Dictionary<Difficulty, List<Tuple>>
            {
                { Difficulty.Easy, new List<Tuple>()
                {
                    new Tuple( 2.5f, 100f),
                    new Tuple( 5.0f, 200f),
                    new Tuple( 7.5f, 500f), 
                    new Tuple( 10f, 1000f), 
                    new Tuple( 20f, 2000f), 
                    new Tuple( 50f, 10000f), 
                    new Tuple( 75f, 20000f), 
                    new Tuple( 85f, 30000f), 
                    new Tuple( 95f, 40000f), 
                    new Tuple(100f, 50000f), 
                }},

                { Difficulty.Normal, new List<Tuple>()
                {
                    new Tuple( 2.5f, 200f),
                    new Tuple( 5.0f, 400f),
                    new Tuple( 7.5f, 600f),
                    new Tuple( 10f, 1500f),
                    new Tuple( 20f, 3000f),
                    new Tuple( 50f, 20000f),
                    new Tuple( 75f, 40000f),
                    new Tuple( 85f, 50000f),
                    new Tuple( 95f, 60000f),
                    new Tuple(100f, 70000f),
                }},

                { Difficulty.Hard, new List<Tuple>()
                {
                    new Tuple( 2.5f, 400f),
                    new Tuple( 5.0f, 800f),
                    new Tuple( 7.5f, 1500f),
                    new Tuple( 10f, 3000f),
                    new Tuple( 20f, 5000f),
                    new Tuple( 50f, 50000f),
                    new Tuple( 75f, 75000f),
                    new Tuple( 85f, 85000f),
                    new Tuple( 95f, 95000f),
                    new Tuple(100f, 100000f),
                }}
            };

            /// <summary>
            /// Calculates how many points a move is awarded
            /// based on percentage cleared
            /// </summary>
            public static float CalculatePerPercentage(float percentage)
            {
                List<Tuple> list = ScoreChart[instance.difficulty];
                for(int i = 0; i < list.Count; i++)
                {
                    if (list[i].percentage >= percentage)
                        return percentage * list[i].points;
                }
                throw new System.Exception("Unexpected error");
            }
        }


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

        /// <summary> Speed multiplier for the speed skill, adjusted for difficulty </summary>
        public float speedSkillMultiplier { get { return SpeedSkillMultiplier[(int)difficulty]; } }

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