using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Jintori;
using Jintori.SelectScreen;

// --- Class Declaration ------------------------------------------------------------------------
public class Test : MonoBehaviour 
{
    // --- Events -----------------------------------------------------------------------------------
    // --- Constants --------------------------------------------------------------------------------
    // --- Static Properties ------------------------------------------------------------------------
    // --- Static Methods ---------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------
    // --- Inspector --------------------------------------------------------------------------------

    // --- Properties -------------------------------------------------------------------------------

    // --- MonoBehaviour ----------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	

    // --- Methods ----------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	
    private IEnumerator Start()
    {
        IllogicGate.SoundManager2D.instance.PlayBGM("Test 1");
        yield return new WaitForSeconds(5);
        IllogicGate.SoundManager2D.instance.CrossFadeBGM("Test 2", 5);
        yield break;
    }

    private void OnGUI()
    {

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            IllogicGate.SoundManager2D.instance.PlaySFX("ui_accept");
    }

}
