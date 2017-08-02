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
                {
                    float time = Mathf.FloorToInt(records.bestTime * 1000f);

                    int mili = Mathf.FloorToInt(time % 1000); time -= mili; time /= 1000f;
                    int secs = Mathf.FloorToInt(time % 60); time -= secs; time /= 60f;
                    int mins = Mathf.FloorToInt(time);
                    bestTime.text = string.Format("{0:00}:{1:00}:{2:000}", mins, secs, mili);
                }

                if (records.highScore == -1)
                    highScore.text = "N/A";
                else
                    highScore.text = string.Format("{0:##,###,###,##0}", records.highScore);
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