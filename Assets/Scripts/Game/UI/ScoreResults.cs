using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class ScoreResults : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Text timeValueText = null;

        [SerializeField]
        Text leftValueText = null;

        [SerializeField]
        Text scoreValueText = null;

        [SerializeField]
        Animation newBestTime = null;

        [SerializeField]
        Animation newHighScore = null;

        // --- Properties -------------------------------------------------------------------------------
        public bool isDone;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// If the user presses fire, cancel all animations and just jump to the end
        /// </summary>
        IEnumerator CheckForCancel(float time, float left, long score, bool isBestTime, bool isHighScore)
        {
            yield return null;
            while (!Input.GetButtonDown("Cut"))
                yield return null;

            StopAllCoroutines();

            timeValueText.text = Util.FormatTime(time);
            leftValueText.text = Util.FormatTime(0);
            scoreValueText.text = Util.FormatScore(score);

            if (isBestTime)
            {
                newBestTime.gameObject.SetActive(true);
                newBestTime.Play("hiscore");
                newBestTime["hiscore"].normalizedTime = 1.0f;
            }
            if (isHighScore)
            {
                newHighScore.gameObject.SetActive(true);
                newHighScore.Play("hiscore");
                newHighScore["hiscore"].normalizedTime = 1.0f;
            }

            isDone = true;
        }
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Shows the results panel
        /// </summary>
        public void Show(float time, float left, long scoreBefore, long scoreAfter, bool isBestTime, bool isHighScore)
        {
            isDone = false;
            gameObject.SetActive(true);
            StartCoroutine(PlayAnimations(time, left, scoreBefore, scoreAfter, isBestTime, isHighScore));
            StartCoroutine(CheckForCancel(time, left, scoreAfter, isBestTime, isHighScore));
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator PlayAnimations(float time, float left, long scoreBefore, long scoreAfter, bool isBestTime, bool isHighScore)
        {
            newHighScore.gameObject.SetActive(false);
            newBestTime.gameObject.SetActive(false);

            timeValueText.text = "";
            leftValueText.text = "";
            scoreValueText.text = "";

            Animation anim = GetComponent<Animation>();
            anim.Play();
            while (anim.isPlaying)
                yield return null;

            yield return new WaitForSeconds(0.75f);
            timeValueText.text = Util.FormatTime(time);

            yield return new WaitForSeconds(0.75f);
            leftValueText.text = Util.FormatTime(left);

            yield return new WaitForSeconds(0.75f);
            scoreValueText.text = Util.FormatScore(scoreBefore);

            // animate the seconds and score
            const float MaxAnimationTime = 5f;  // don't take more than this long
            const float LeftDecreaseSpeed = 10; // in "left" time over "animation" seconds
            float speed = LeftDecreaseSpeed;
            if (left / LeftDecreaseSpeed > MaxAnimationTime)
                speed = left / MaxAnimationTime;

            float scorePerLeftTime = (scoreAfter - scoreBefore) / left;
            long score = scoreBefore;
            while (left > 0)
            {
                float deltaLeft = speed * Time.deltaTime;
                left -= deltaLeft;
                score += Mathf.FloorToInt(scorePerLeftTime * deltaLeft);
                scoreValueText.text = Util.FormatScore(score);
                leftValueText.text = Util.FormatTime(left);
                yield return null;
            }
            scoreValueText.text = Util.FormatScore(scoreAfter);
            leftValueText.text = Util.FormatTime(0);

            // show "best time" animation
            if (isBestTime)
            {
                newBestTime.gameObject.SetActive(true);
                newBestTime.Play();
                while (newBestTime.isPlaying)
                    yield return null;
            }

            // show "high score" animation
            if (isHighScore)
            {
                newHighScore.gameObject.SetActive(true);
                newHighScore.Play();
                while (newHighScore.isPlaying)
                    yield return null;
            }

            // stop the "cancel" coroutine
            StopAllCoroutines();
            isDone = true;
        }

    }
}