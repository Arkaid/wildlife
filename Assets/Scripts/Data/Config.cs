using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public partial class Config : IllogicGate.Singleton<Config>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Number of rounds to play </summary>
        public const int Rounds = 4;
        
        /// <summary> Levels of overall game difficulty </summary>
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard,
        }

        /*
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
        */

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> JSON object with fixed settings data. The user cannot change these </summary>
        JSONObject settings;

        /// <summary> These are the game options. The user can change these </summary>
        Data.OptionsFile options;

        /// <summary> shortcut to the JSON branch for the current difficulty </summary>
        JSONObject json { get { return settings[difficulty.ToString()]; } }

        /// <summary> Game difficulty </summary>
        public Difficulty difficulty
        {
            get { return (Difficulty)System.Enum.Parse(typeof(Difficulty), options.json["difficulty"].str); }
            set { options.json.SetField("difficulty", value.ToString()); }
        }

        /// <summary> Time for one round, adjusted for difficulty </summary>
        public int roundTime { get { return (int)json["round_time"].i; } }

        /// <summary> Needed clear ratio to win, adjusted for difficulty </summary>
        public float clearRatio { get { return json["clear_ratio"].f; } }

        /// <summary> Needed clear percentage to win, adjusted for difficulty </summary>
        public int clearPercentage { get { return Mathf.FloorToInt(clearRatio * 100); } }

        /// <summary> Speed multiplier for the speed skill, adjusted for difficulty </summary>
        public float speedSkillMultiplier { get { return json["skill_speed_multiplier"].f; } }

        /// <summary> Bonus time score, in bonus points per second remaining </summary>
        public float bonusTimeScore { get { return json["bonus_time_score"].f; } }

        /// <summary> Starting amount of lives, adjusted for difficulty </summary>
        public int startLives { get { return (int)json["start_lives"].i; } }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        protected override void OnInstanceCreated()
        {
            settings = new JSONObject(Resources.Load<TextAsset>("settings").text);

            options = new Data.OptionsFile();
            options.Load();

            difficulty = Difficulty.Normal;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Return enemy settings JSON for current difficulty
        /// </summary>
        public JSONObject GetEnemySettings(string enemyClassName)
        {
            JSONObject enemies = json["enemies"];

            if (enemies.HasField(enemyClassName))
                return enemies[enemyClassName];
            else
                return null;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Returns how much score to assign per 1%, depending on how much you cleared
        /// </summary>
        public float CalculatePerPercentage(float clearedPercentage)
        {
            List<JSONObject> list = json["score_table"].list;
            for (int i = 0; i < list.Count; i++)
            {
                // first entry in is percentage, second one is score
                List<JSONObject> item = list[i].list;
                if (item[0].f >= clearedPercentage)
                    return clearedPercentage * item[1].f;
            }
            throw new System.Exception("Unexpected error");
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Maximum skill time
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public float GetSkillTime(Game.Skill.Type skill)
        {
            return json["skills"][skill.ToString()]["max_time"].f;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Calculate how much skill to charge according to percentage cleared in one move
        /// </summary>
        public float CalculateSkillCharge(Game.Skill.Type skill, float percentage)
        {
            return json["skills"][skill.ToString()]["time_charge_per_percentage"].f * percentage;
        }

    }
}