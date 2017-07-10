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
            playArea.Setup(DEBUG_baseImage, DEBUG_shadowImage, typeof(Bouncy));
            
            Timer.instance.ResetTimer(120);
            StartCoroutine(SetStartingZone());
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator SetStartingZone()
        {
            // place the player on the center of the screen
            playArea.player.x = PlayArea.ImageWidth / 2;
            playArea.player.y = PlayArea.ImageHeight / 2;
            playArea.player.gameObject.SetActive(false);

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

            IntRect rect = new IntRect()
            {
                x = playArea.player.x - Mathf.FloorToInt(w),
                y = playArea.player.y - Mathf.FloorToInt(h),
                width = Mathf.RoundToInt(w * 2),
                height = Mathf.RoundToInt(h * 2)
            };

            // re-enable the player and put it in a corner of the square
            playArea.player.gameObject.SetActive(true);
            playArea.player.x = rect.x;
            playArea.player.y = rect.y;

            // create the square and destroy the "preview"
            playArea.CreateStartingZone(rect);
            Destroy(initialSquare);

            // now that the play area has colliders, 
            // place the boss safely in the shadow
            playArea.boss.gameObject.SetActive(true);
            playArea.boss.SetStartPosition(rect);
            playArea.boss.Run();

            // start timer
            Timer.instance.StartTimer();

            yield break;
        }
    }
}