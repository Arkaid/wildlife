using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Timer : IllogicGate.SingletonBehaviour<Timer>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public float totalTime { get; private set; }

        public float remainingTime { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            StartTimer(120);
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void StartTimer(int totalTime)
        {
            this.totalTime = totalTime;
            remainingTime = totalTime;
            UI.instance.totalTime = totalTime;
            StartCoroutine(Countdown());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator Countdown()
        {
            while(remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UI.instance.time = remainingTime;
                yield return null;
            }
            remainingTime = 0;
            UI.instance.time = 0;
        }
    }
}