using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IllogicGate
{
    public interface IState
    {
        string next { get; }
        IEnumerator Run();
    }

    // --- Class Declaration ------------------------------------------------------------------------
    public abstract class StateMachineBehaviour : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        abstract protected List<IState> states { get; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private IEnumerator Start()
        {
            IState state = states[0];
            while (true)
            {
                yield return StartCoroutine(state.Run());
                if (string.IsNullOrEmpty(state.next))
                    break;
                string next = state.next;
                state = states.Find(s => s.GetType().Name == next);
                if (state == null)
                    throw new System.Exception("Invalid state name :" + next);
            }
        }

    }
}