using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Game : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        enum State
        {
            Setup,
            SelectStartQuad,
            Playing,
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        PlayArea playArea = null;

        [SerializeField]
        MeshFilter initialSquare = null;

        [SerializeField]
        Texture2D DEBUG_baseImage = null;

        [SerializeField]
        Texture2D DEBUG_shadowImage = null;

        // --- Properties -------------------------------------------------------------------------------
        State state;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            playArea.Setup(DEBUG_baseImage, DEBUG_shadowImage);
            StartCoroutine(SetStartingZone());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator SetStartingZone()
        {
            // place the player in its initial position
            playArea.player.x = PlayArea.ImageWidth / 2;
            playArea.player.y = PlayArea.ImageHeight / 2;
            playArea.player.gameObject.SetActive(false);

            // create a starting zone
            //playArea.CreateStartingZone(10, 10, 150, 100);
            Timer.instance.ResetTimer(120);

            // Create a square that randomly changes sizes
            const float Area = 100 * 100;
            const int MaxWidth = 300;
            const int MinWidth = 20;
            initialSquare.mesh.triangles = new int[]
            {
                0, 1, 2,
                3, 0, 2
            };
            float w = 0, h = 0;
            while (!Input.GetButton("Fire1"))
            {
                w = Random.Range(MinWidth, MaxWidth);
                h = Area / w;
                w /= 2;
                h /= 2;
                Vector3[] corners = new Vector3[]
                {
                    new Vector3(-w, -h),
                    new Vector3(-w,  h),
                    new Vector3( w,  h),
                    new Vector3( w, -h),
                };

                initialSquare.mesh.vertices = corners;
                yield return new WaitForSeconds(0.05f);
            }

            // re-enable the player and put it in a corner of the square
            playArea.player.gameObject.SetActive(true);
            int x = playArea.player.x - Mathf.FloorToInt(w);
            int y = playArea.player.y - Mathf.FloorToInt(h);
            playArea.player.x = x;
            playArea.player.y = y;

            // create the square and destroy the "preview"
            playArea.CreateStartingZone(x, y, Mathf.RoundToInt(w * 2), Mathf.RoundToInt(h * 2));
            Destroy(initialSquare);

            // start timer
            Timer.instance.StartTimer();

            yield break;
        }
    }
}