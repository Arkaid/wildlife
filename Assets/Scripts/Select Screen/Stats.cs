using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.SelectScreen
{
    using Data;

    // --- Class Declaration ------------------------------------------------------------------------
    public class Stats : MonoBehaviour
    {
        [System.Serializable]
        class  Records
        {
            [SerializeField]
            Text bestTime = null;
            [SerializeField]
            Text highScore = null;

            public void SetRecords(Data.Records records)
            {
                if (records.bestTime == -1)
                    bestTime.text = "N/A";
                else
                    bestTime.text = Util.FormatTime(records.bestTime);

                if (records.highScore == -1)
                    highScore.text = "N/A";
                else
                    highScore.text = Util.FormatScore(records.highScore);
            }
        }

        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Records easy = null;

        [SerializeField]
        Records normal = null;

        [SerializeField]
        Records hard = null;

        // --- Properties -------------------------------------------------------------------------------
        new Animation animation;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            animation = GetComponent<Animation>();
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------
        public void Show(int round, string guid)
        {
            gameObject.SetActive(true);
            animation.Play();

            CharacterStats stats = SaveFile.instance.GetCharacterStats(guid);

            easy.SetRecords(stats.rounds[round].records[Config.Difficulty.Easy]);
            normal.SetRecords(stats.rounds[round].records[Config.Difficulty.Normal]);
            hard.SetRecords(stats.rounds[round].records[Config.Difficulty.Hard]);
        }
    }
}