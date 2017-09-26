using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    using IllogicGate;

    // --- Class Declaration ------------------------------------------------------------------------
    public class TextMeshEffect : MonoBehaviour
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
        /// <summary>
        /// Shows any text effect. start is in world coordinates
        /// </summary>
        public void Show(string text, Vector3 start)
        {
            TextMesh[] tms = GetComponentsInChildren<TextMesh>();
            foreach (TextMesh tm in tms)
                tm.text = text;

            StartCoroutine(RunCoroutine(start));
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator RunCoroutine(Vector3 start)
        {
            const float AnimationTime = 2f;

            // animate in local coordinates;
            start = transform.InverseTransformPoint(start);
            Vector3 end = start + Vector3.up * 75f;
            Renderer[] rends = GetComponentsInChildren<Renderer>();

            float elapsed = 0;
            while(elapsed < AnimationTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / AnimationTime);
                float p = Tweener.EaseOut(t);
                float a = 1f - Tweener.EaseIn(t);
                transform.localPosition = Vector3.Lerp(start, end, p);

                foreach(Renderer rend in rends)
                {
                    Color col = rend.material.color;
                    col.a = a;
                    rend.material.color = col;
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}