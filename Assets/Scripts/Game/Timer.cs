using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Timer : IllogicGate.SingletonBehaviour<Timer>
    {
        // --- Events -----------------------------------------------------------------------------------
        public event System.Action timedOut;

        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public float totalTime { get; private set; }

        public float remainingTime { get; private set; }

        public float elapsedTime { get { return totalTime - remainingTime; } }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void StartTimer()
        {
            StartCoroutine(Countdown());
        }

        // -----------------------------------------------------------------------------------	
        public void ResetTimer(int totalTime)
        {
            this.totalTime = totalTime;
            remainingTime = totalTime;
        }

        // -----------------------------------------------------------------------------------	
        public void StopTimer()
        {
            StopAllCoroutines();
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator Countdown()
        {
            while(remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UI.instance.timeDisplay.time = remainingTime;
                yield return null;
            }
            remainingTime = 0;
            UI.instance.timeDisplay.time = 0;

            if (timedOut != null)
                timedOut();
        }
    }
}