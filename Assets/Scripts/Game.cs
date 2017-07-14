using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Controls three rounds of a game.
    /// </summary>
    public class Game : IllogicGate.SingletonBehaviour<Game>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------

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
        /// <summary> Current round (1, 2 or 3) </summary>
        public int round { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            playArea.Setup(DEBUG_baseImage, DEBUG_shadowImage, typeof(Slimy));

            round = 1;
            Timer.instance.ResetTimer(Config.instance.roundTime);
            StartCoroutine(Initialize());
        }
        
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator Initialize()
        {
            // place the player on the center of the screen
            playArea.player.x = PlayArea.ImageWidth / 2;
            playArea.player.y = PlayArea.ImageHeight / 2;
            playArea.player.Hide();

            // Create a square that randomly changes sizes
            // until the fire button gets pressed
            const float Area = 50 * 50;
            const int MaxWidth = 100;
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
            playArea.player.Spawn(rect.x, rect.y);

            // create the square and destroy the "preview"
            playArea.CreateStartingZone(rect);
            Destroy(initialSquare);

            // now that the play area has colliders, 
            // place the boss safely in the shadow
            playArea.boss.gameObject.SetActive(true);
            playArea.boss.SetBossStartPosition(rect);
            playArea.boss.Run();

            // start timer
            Timer.instance.StartTimer();

            // set callbacks to check game progress
            playArea.mask.maskCleared += OnMaskCleared;

            yield break;
        }
        
        // -----------------------------------------------------------------------------------	
        IEnumerator WinRound()
        {
            // kill boss
            playArea.boss.Kill();

            yield break;
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            // Did we win?
            if (playArea.mask.clearedRatio >= Config.instance.clearRatio)
            {
                playArea.mask.maskCleared -= OnMaskCleared;
                StartCoroutine(WinRound());
            }
        }
    }
}