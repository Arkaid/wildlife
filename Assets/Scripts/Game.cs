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

        /// <summary> Play area currently active </summary>
        PlayArea currentPlay;

        /// <summary> lives left </summary>
        public int livesLeft { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            round = 1;
            livesLeft = Config.instance.lives;
            playArea.gameObject.SetActive(false);
            StartCoroutine(InitializeRound());
        }
        
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator InitializeRound()
        {
            // destroy previous area
            if (currentPlay != null)
                Destroy(currentPlay.gameObject);

            // create a fresh play area
            currentPlay = Instantiate(playArea, playArea.transform.parent, true);
            currentPlay.gameObject.SetActive(true);
            currentPlay.Setup(DEBUG_baseImage, DEBUG_shadowImage, typeof(Slimy));

            // check when the player spawns to count lives / game overs
            currentPlay.player.spawned += OnPlayerSpawned;
            currentPlay.player.died += OnPlayerDied;

            // set the camera to auto adjust
            CameraAdjuster camAdjuster = Camera.main.GetComponent<CameraAdjuster>();
            camAdjuster.autoAdjust = true;

            // reset the UI
            UI.instance.Reset(livesLeft);

            // reset the timer
            Timer.instance.ResetTimer(Config.instance.roundTime);

            // Hide the player
            currentPlay.player.Hide();

            // play the intro animation for the round
            yield return StartCoroutine(UI.instance.roundStart.Show(round));

            // Create a square that randomly changes sizes
            // until the fire button gets pressed
            const float Area = 50 * 50;
            const int MaxWidth = 100;
            const int MinWidth = 20;
            const float FlickDelay = 0.075f;
            initialSquare.gameObject.SetActive(true);
            initialSquare.mesh.triangles = new int[]
            {
                0, 1, 2,
                3, 0, 2
            };
            float w = 0, h = 0;
            while (true)
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

                // wait until the next random square
                // or cancel wait if user presses button
                float wait = FlickDelay;
                while (wait >= 0 && !Input.GetButtonDown("Fire1"))
                {
                    wait -= Time.deltaTime;
                    yield return null;
                }
                if (wait > 0)
                    break;
            }

            IntRect rect = new IntRect()
            {
                x = currentPlay.player.x - Mathf.FloorToInt(w),
                y = currentPlay.player.y - Mathf.FloorToInt(h),
                width = Mathf.RoundToInt(w * 2),
                height = Mathf.RoundToInt(h * 2)
            };

            // re-enable the player and put it in a corner of the square
            currentPlay.player.Spawn(rect.x, rect.y);

            // create the square and destroy the "preview"
            currentPlay.CreateStartingZone(rect);
            initialSquare.gameObject.SetActive(false);

            // now that the play area has colliders, 
            // place the boss safely in the shadow
            currentPlay.boss.gameObject.SetActive(true);
            currentPlay.boss.SetBossStartPosition(rect);
            currentPlay.boss.Run();

            // start timer
            Timer.instance.StartTimer();

            // set callbacks to check game progress
            currentPlay.mask.maskCleared += OnMaskCleared;

            yield break;
        }

        // -----------------------------------------------------------------------------------	
        private void OnPlayerSpawned()
        {
            livesLeft--;
            UI.instance.lives = livesLeft;
        }

        // -----------------------------------------------------------------------------------	
        private void OnPlayerDied()
        {
            StartCoroutine(GameOver());
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Runs when the player loses his last life
        /// </summary>
        IEnumerator GameOver()
        {
            // stop timer
            Timer.instance.StopTimer();

            // played the final result before hiding the UI
            UI.instance.PlayResult(false);

            // hide the player
            playArea.player.Hide();

            // wait until the player hits fire again
            yield return null;
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            print("Go back to top");
        }


        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Process when the player clears the minimum needed percentage
        /// </summary>
        IEnumerator WinRound()
        {
            // stop timer
            Timer.instance.StopTimer();

            // kill boss and hide player
            currentPlay.boss.Kill();
            currentPlay.player.Hide();

            // played the final result before hiding the UI
            UI.instance.PlayResult(true);

            // Fit the camera to see all the image
            // (player must be on the center of the play area)
            CameraAdjuster camAdjuster = Camera.main.GetComponent<CameraAdjuster>();
            camAdjuster.ZoomToImage();

            // unhide all the shadow
            yield return StartCoroutine(currentPlay.DiscoverShadow());

            // wait until the player hits fire again
            yield return null;
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // play next round or go back to top menu?
            if (round < 3)
            {
                // add a life since you respawn on the next round
                livesLeft++;

                // start next round
                round++;
                StartCoroutine(InitializeRound());
            }
            else
            {
                print("Go back to top");
            }

            yield break;
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            // Did we win?
            if (currentPlay.mask.clearedRatio >= Config.instance.clearRatio)
            {
                currentPlay.mask.maskCleared -= OnMaskCleared;
                StartCoroutine(WinRound());
            }
        }
    }
}