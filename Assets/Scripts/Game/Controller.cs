using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Controls three rounds of a game.
    /// </summary>
    public class Controller : IllogicGate.SingletonBehaviour<Controller>
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
#if UNITY_EDITOR
        const string DEBUG_file = "Assets/Characters/arkaid01.chr";
#endif

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        /// <summary> Character file containing the images we want to play </summary>
        public static CharacterFile.File sourceFile;

        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        PlayArea playArea = null;

        [SerializeField]
        MeshFilter initialSquare = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Current round (1, 2 or 3) </summary>
        public int round { get; private set; }

        /// <summary> Play area currently active </summary>
        PlayArea currentPlay;

        /// <summary> lives left </summary>
        public int livesLeft { get; private set; }

        /// <summary> Last cleared percentage. Used to calculate how much it changes per clear move (delta percentage) </summary>
        float lastPercentage;

        /// <summary> Current score. Internally float but display floor int </summary>
        float score;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
#if UNITY_EDITOR
            if (sourceFile == null)
                sourceFile = new CharacterFile.File(DEBUG_file);
#endif
            round = 0;
            Timer.instance.timedOut += OnTimerTimedOut;
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

            // Load the images
            CharacterFile.RoundImages roundData = sourceFile.LoadRound(round);

            // create a fresh play area
            currentPlay = Instantiate(playArea, playArea.transform.parent, true);
            currentPlay.gameObject.SetActive(true);
            currentPlay.Setup(roundData.baseImage, roundData.shadowImage, typeof(Slimy));

            // reset percentage tracker to zero
            lastPercentage = 0;

            // reset score
            score = 0;

            // check when the player spawns to count lives / game overs
            currentPlay.player.spawned += OnPlayerSpawned;
            currentPlay.player.died += OnPlayerDied;

            // set the camera to auto adjust
            CameraAdjuster camAdjuster = Camera.main.GetComponent<CameraAdjuster>();
            camAdjuster.autoAdjust = true;

            // reset the UI
            UI.instance.Reset(livesLeft, Config.instance.clearPercentage, Config.instance.roundTime);

            // reset the timer
            Timer.instance.ResetTimer(Config.instance.roundTime);

            // Hide the player
            currentPlay.player.Hide();

            // Hide the transition
            yield return StartCoroutine(Transition.instance.Hide());

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

            // set callbacks to check game progress
            currentPlay.mask.maskCleared += OnMaskCleared;

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
        private void OnTimerTimedOut()
        {
            livesLeft = 0;
            currentPlay.player.Hit();
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

            // hide the player at its current position
            currentPlay.player.Hide(currentPlay.player.x, currentPlay.player.y);

            // wait until the player hits fire again
            yield return null;
            while (!Input.GetButtonDown("Fire1"))
                yield return null;

            // transition out
            yield return StartCoroutine(Transition.instance.Show());

            SceneManager.LoadScene("Select Menu");
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

            // save results
            SaveResults();

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

            // transition out
            yield return StartCoroutine(Transition.instance.Show());

            // play next round or go back to top menu?
            round++;
            if (round < Config.Rounds)
            {
                // add a life since you respawn on the next round
                livesLeft++;

                // start next round
                StartCoroutine(InitializeRound());
            }
            else
            {
                SceneManager.LoadScene("Select Menu");
            }

            yield break;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Saves game results into the save file
        /// </summary>
        private void SaveResults()
        {
            Data.CharacterStats stats = Data.SaveFile.instance.GetCharacterStats(sourceFile.guid);
            Data.RoundData roundData = stats.rounds[round];
            Data.Records records = roundData.records[Config.instance.difficulty];
            roundData.cleared = true;

            float elapsed = Timer.instance.totalTime - Timer.instance.remainingTime;
            if (elapsed < records.bestTime)
                records.bestTime = elapsed;

            long scoreL = (long)score;
            if (scoreL > records.highScore)
                records.highScore = scoreL;

            Data.SaveFile.instance.Save();
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            // calculate score
            float percentage = currentPlay.mask.clearedRatio * 100;
            float delta = percentage - lastPercentage;

            float deltaScore = Config.Score.CalculatePerPercentage(delta);
            score += deltaScore;
            lastPercentage = percentage;
            UI.instance.scoreDisplay.score = (long)score;

            // Did we win?
            if (currentPlay.mask.clearedRatio >= Config.instance.clearRatio)
            {
                currentPlay.mask.maskCleared -= OnMaskCleared;
                StartCoroutine(WinRound());
            }
        }
    }
}