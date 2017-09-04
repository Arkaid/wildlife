using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jintori.Game
{
    using Common.UI;

    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Controls three rounds of a game.
    /// </summary>
    public class Controller : IllogicGate.SingletonBehaviour<Controller>
    {
        // --- Events -----------------------------------------------------------------------------------
        public event System.Action<bool> paused;

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
        PlayArea sourcePlayArea = null;

        [SerializeField]
        MeshFilter initialSquare = null;

#if UNITY_EDITOR
        [SerializeField]
        Texture2D DEBUG_baseImage = null;

        [SerializeField]
        Texture2D DEBUG_shadowImage = null;
#endif

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Current round (1, 2 or 3) </summary>
        public int round { get; private set; }

        /// <summary> Play area currently active </summary>
        PlayArea playArea;

        /// <summary> lives left </summary>
        public int livesLeft { get; private set; }

        /// <summary> Last cleared percentage. Used to calculate how much it changes per clear move (delta percentage) </summary>
        float lastPercentage;

        /// <summary> Current score. Internally float but display floor int </summary>
        public float score { get; private set; }

        /// <summary> Camera controller to track player and zoom to image </summary>
        CameraController cameraController;

        /// <summary> Draw one boss at the time from here for each round </summary>
        List<Enemy> roundBoss;

        /// <summary> True, if the game is paused </summary>
        public bool isPaused { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
#if UNITY_EDITOR
            if (sourceFile == null)
                sourceFile = new CharacterFile.File(DEBUG_file);
#endif
            if (SoundManager.instance == null)
                Instantiate(Resources.Load("Sound Manager"));

            cameraController = Camera.main.GetComponent<CameraController>();
            sourcePlayArea.gameObject.SetActive(false);

            // character is now set as played
            Data.SaveFile saveFile = Data.SaveFile.instance;
            Data.CharacterStats stats = saveFile.GetCharacterStats(sourceFile.guid);
            if (!stats.played)
            {
                stats.played = true;
                saveFile.Save();
            }

            // randomize bosses (we have 3, but need 4)
            roundBoss = new List<Enemy>(sourcePlayArea.GetBosses());
            for(int i = 0; i < 100; i++)
            {
                int a = Random.Range(0, roundBoss.Count);
                int b = Random.Range(0, roundBoss.Count);
                Enemy tmp = roundBoss[a];
                roundBoss[a] = roundBoss[b];
                roundBoss[b] = tmp;
            }
            roundBoss.Add(roundBoss[Random.Range(0, roundBoss.Count)]);

            // basic initialization
            round = 0;
            livesLeft = Config.instance.startLives;
            Skill.instance.Initialize();
            Timer.instance.timedOut += OnTimerTimedOut;

            // start the first round
            StartCoroutine(InitializeRound());
        }

        // -----------------------------------------------------------------------------------	
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F9))
            {
                playArea.mask.maskCleared -= OnMaskCleared;
                StartCoroutine(WinRound());
            }
#endif
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        IEnumerator InitializeRound()
        {
            // destroy previous area
            if (playArea != null)
                Destroy(playArea.gameObject);

            // Play a random track
            SoundManager.instance.PlayRandomRoundClip();

            // Load the images
            CharacterFile.RoundImages roundData = sourceFile.LoadRound(round);

            // create a fresh play area
            System.Type bossType = roundBoss[round].GetType();
            bossType = typeof(Wormy);
            playArea = Instantiate(sourcePlayArea, sourcePlayArea.transform.parent, true);
            playArea.gameObject.SetActive(true);
            playArea.Setup(roundData.baseImage, roundData.shadowImage, bossType);
            //currentPlay.Setup(DEBUG_baseImage, DEBUG_shadowImage, bossType);

            // reset percentage tracker to zero
            lastPercentage = 0;

            // reset score
            score = 0;

            // check when the player spawns to count lives / game overs
            playArea.player.spawned += OnPlayerSpawned;
            playArea.player.died += OnPlayerDied;

            // Let the camera know it can start tracking the player
            cameraController.StartTracking(playArea);

            // reset the UI
            UI.instance.Reset(
                livesLeft,
                Config.instance.clearPercentage,
                Config.instance.roundTime,
                Skill.instance.maxTime,
                Skill.instance.remainingTime);

            // reset the timer
            Timer.instance.ResetTimer(Config.instance.roundTime);

            // Hide the player
            playArea.player.Hide();

            // Hide the transition
#if UNITY_EDITOR
            if (Transition.instance != null)
                yield return StartCoroutine(Transition.instance.Hide());
#else
            yield return StartCoroutine(Transition.instance.Hide());
#endif
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
                while (wait >= 0 && !Input.GetButtonDown("Cut"))
                {
                    wait -= Time.deltaTime;
                    yield return null;
                }
                if (wait > 0)
                    break;
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

            // set callbacks to check game progress
            playArea.mask.maskCleared += OnMaskCleared;

            // create the square and destroy the "preview"
            playArea.CreateStartingZone(rect);
            initialSquare.gameObject.SetActive(false);

            // setup the play area ffects
            // we have to do it here since we need the initial path
            playArea.effects.Setup();

            // now that the play area has colliders, 
            // place the boss safely in the shadow
            playArea.boss.gameObject.SetActive(true);
            playArea.boss.SetBossStartPosition(rect);
            playArea.boss.minionKilled += OnMinionKilled;
            playArea.boss.Run();

            // start tracking it
            UI.instance.bossTracker.StartTracking(playArea.boss);

            // start timer
            Timer.instance.StartTimer();

            // Handle the pause from here on out
            StartCoroutine("PauseHandler");

            yield break;
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator PauseHandler()
        {
            while (true)
            {
                while (!Input.GetButtonDown("Pause"))
                    yield return null;

                isPaused = true;
                Timer.instance.StopTimer();

                if (paused != null)
                    paused(isPaused);

                MessagePopup popup = Overlay.instance.messagePopup;
                popup.ShowYesNo("END GAME?", "PAUSED");
                while (popup.isVisible)
                    yield return null;

                isPaused = false;
                Timer.instance.StartTimer();
                if (paused != null)
                    paused(isPaused);

                if (popup.isYes)
                    StartCoroutine(GameOver(true));
            }
        }
        // -----------------------------------------------------------------------------------	
        private void OnMinionKilled(Enemy minion, bool killedByPlayer)
        {
            if (killedByPlayer)
                score += minion.score;
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
            StartCoroutine(GameOver(false));
        }

        // -----------------------------------------------------------------------------------	
        private void OnTimerTimedOut()
        {
            livesLeft = 0;
            playArea.player.Hit(true);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Runs when the player loses his last life
        /// </summary>
        /// <param name="isUserExit">If true, the game over was triggered by the menu, not gameplay</param>
        IEnumerator GameOver(bool isUserExit)
        {
            // no more pause handling
            StopCoroutine("PauseHandler");

            // stop timer
            Timer.instance.StopTimer();

            // played the final result before hiding the UI
            if (!isUserExit)
                UI.instance.PlayResult(false);

            // hide the player at its current position
            playArea.player.Hide(playArea.player.x, playArea.player.y);

            // stop tracking player
            cameraController.StopTracking(false);

            bool retry = false;
            if (!isUserExit)
            {
                // wait until the player hits fire again
                yield return null;
                while (!Input.GetButtonDown("Cut"))
                    yield return null;

                // present retry dialog
                MessagePopup popup = Overlay.instance.messagePopup;
                popup.ShowYesNo("RETRY?", "GAME OVER");
                while (popup.isVisible)
                    yield return null;
                retry = popup.isYes;
            }

            // fade out BGM
            SoundManager.instance.FadeoutBGM(1f);

            // transition out and wait for the rest of the fadeout
            yield return StartCoroutine(Transition.instance.Show());
            yield return new WaitForSeconds(1f - Transition.TransitionTime);

            SceneManager.LoadScene(retry ? "Game" : "Select Menu");
        }


        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Process when the player clears the minimum needed percentage
        /// </summary>
        IEnumerator WinRound()
        {
            // no more pause handling
            StopCoroutine("PauseHandler");

            // stop timer
            Timer.instance.StopTimer();

            // kill boss and hide player
            playArea.boss.Kill(false);
            playArea.player.Hide();

            // calculate bonus score due to remaining time
            long beforeScore = (long)score;
            long bonusScore = (long)(Timer.instance.remainingTime * Config.instance.bonusTimeScore);
            score = beforeScore + bonusScore;

            // save results
            bool isBestTime, isHighScore;
            SaveResults(out isBestTime, out isHighScore);

            // played the final result before hiding the UI
            UI.instance.PlayResult(true);

            // Fit the camera to see all the image
            // (player must be on the center of the play area)
            cameraController.StopTracking(true);

            // unhide all the shadow
            yield return StartCoroutine(playArea.DiscoverShadow());
            UI.instance.HideResult();

            // wait until the player hits fire again
            yield return null;
            while (!Input.GetButtonDown("Cut"))
                yield return null;

            // show score results
            SoundManager.instance.PlaySFX("ui_sweep_in");
            UI.instance.scoreResults.Show(
                Timer.instance.elapsedTime,
                Timer.instance.remainingTime,
                beforeScore, (long)score,
                isBestTime, isHighScore
            );
            while (!UI.instance.scoreResults.isDone)
                yield return null;

            // wait until the player hits fire again
            yield return null;
            while (!Input.GetButtonDown("Cut"))
                yield return null;

            // fade out BGM
            SoundManager.instance.FadeoutBGM(1f);

            // transition out and wait for the rest of the fadeout
            yield return StartCoroutine(Transition.instance.Show());
            yield return new WaitForSeconds(1f - Transition.TransitionTime);

            // hide the scores
            UI.instance.scoreResults.Hide();

            // play next round or go back to top menu?
            round++;
            int lastRound = sourceFile.availableRounds;
            if (Config.instance.difficulty != Config.Difficulty.Hard && 
                sourceFile.availableRounds == Config.Rounds)
                lastRound = Config.Rounds - 1; // the last round can only be played in Hard mode

            if (round < lastRound)
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
        private void SaveResults(out bool isBestTime, out bool isHighScore)
        {
            Data.CharacterStats stats = Data.SaveFile.instance.GetCharacterStats(sourceFile.guid);
            Data.RoundData roundData = stats.rounds[round];
            Data.Records records = roundData.records[Config.instance.difficulty];
            roundData.cleared = true;

            float elapsed = Timer.instance.elapsedTime;
            isBestTime = records.bestTime == -1 || elapsed < records.bestTime;
            if (isBestTime)
                records.bestTime = elapsed;

            long scoreL = (long)score;
            isHighScore = scoreL > records.highScore;
            if (isHighScore)
                records.highScore = scoreL;

            Data.SaveFile.instance.Save();
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared(Point centroid)
        {
            // calculate score
            float percentage = playArea.mask.clearedRatio * 100;
            float delta = percentage - lastPercentage;

            float deltaScore = Config.instance.CalculatePerPercentage(delta);
            score += deltaScore;
            lastPercentage = percentage;
            UI.instance.scoreDisplay.score = (long)score;

            // calculate skill recharge
            Skill.instance.Recharge(delta);

            // Did we win?
            if (playArea.mask.clearedRatio >= Config.instance.clearRatio)
            {
                playArea.mask.maskCleared -= OnMaskCleared;
                StartCoroutine(WinRound());
            }
        }
    }
}