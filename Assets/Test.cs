using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using Jintori;
using Jintori.SelectScreen;
using System;



// --- Class Declaration ------------------------------------------------------------------------
public class Test : MonoBehaviour
{

    // --- Events -----------------------------------------------------------------------------------
    // --- Constants --------------------------------------------------------------------------------
    // --- Static Properties ------------------------------------------------------------------------
    // --- Static Methods ---------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------
    // --- Inspector --------------------------------------------------------------------------------
    [SerializeField]
    Collider2D target;

    // --- Properties -------------------------------------------------------------------------------
    Vector2 velocity = Vector2.one * -75;

    // --- MonoBehaviour ----------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	
    private void Start()
    {
        StartCoroutine(FixedUpdateCoroutine());
    }
    // -----------------------------------------------------------------------------------	
    IEnumerator FixedUpdateCoroutine()
    {
        YieldInstruction wffu = new WaitForFixedUpdate();
        while(true)
        {
            print("FixedUpdateCoroutine");
            yield return null;
        }
    }
    // -----------------------------------------------------------------------------------	
    private void FixedUpdate()
    {
        print("FixedUpdate");
        //transform.position = transform.position + (Vector3)velocity * Time.fixedDeltaTime;
        /*
        Collider2D col = GetComponent<Collider2D>();
        if (col.IsTouching(target))
            print("boop");
            */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);
    }
    // --- Methods ----------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	


}
