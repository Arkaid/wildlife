using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Player : PlayAreaObject
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called when the player spawns into the play area </summary>
        public event System.Action spawned = null;

        /// <summary> Called when the player tried to spawn but there were no more lives left </summary>
        public event System.Action died = null;

        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Possible states for the player </summary>
        enum State
        {
            SafePath,
            StartCut,
            Cutting,
            Rewinding,
            Dying,
        }

        /// <summary> Current direction of travel </summary>
        enum Direction
        {
            None,
            Horizontal,
            Vertical
        }
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------       
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Player speed </summary>
        int speed = 120;

        /// <summary> Rewind history </summary>
        Stack<Point> rewindHistory = new Stack<Point>();

        /// <summary> Current state </summary>
        State state = State.SafePath;

        /// <summary> Current travel direction </summary>
        Direction direction = Direction.None;

        /// <summary> First point in the cut path (used for the renderer) </summary>
        Point cutPathStart;


        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            playArea.cutPath.Clear();
        }
        
        // -----------------------------------------------------------------------------------	
        void Update()
        {
            // number of moves left
            int left = Mathf.RoundToInt(speed * Time.deltaTime);

            // See if we need to change state first
            switch (state)
            {
                case State.SafePath:
                    if (Input.GetButton("Fire1"))
                        state = State.StartCut;
                    break;

                case State.StartCut:
                    if (!Input.GetButton("Fire1"))
                        state = State.SafePath;
                    break;

                case State.Cutting:
                    if (!Input.GetButton("Fire1"))
                        state = State.Rewinding;
                    break;

                case State.Rewinding:
                    if (Input.GetButton("Fire1"))
                        state = State.Cutting;
                    else
                        Rewind(left);
                    break;
            }

            // now move
            switch (state)
            {
                case State.SafePath:
                    MoveOnSafePath(left);
                    break;

                case State.StartCut:
                    // check if we can move into the shadowed area
                    int dx = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                    int dy = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                    if (playArea.mask[x + dx, y + dy] == PlayArea.Shadowed)
                    {
                        cutPathStart = new Point(x + dx, y + dy);
                        state = State.Cutting;

                        // we need to establish direction
                        // to start cutting right away
                        if (dx != 0)
                            direction = Direction.Horizontal;
                        else
                            direction = Direction.Vertical;
                        goto case State.Cutting;
                    }
                    break;

                case State.Cutting:
                    CutShadow(left);
                    break;
            }
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary> Hides and disables the player </summary>
        public void Hide(int x = PlayArea.ImageWidth / 2, int y = PlayArea.ImageHeight / 2)
        {
            this.x = x;
            this.y = y;
            gameObject.SetActive(false);
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Spawns the player at the given position 
        /// </summary>
        public void Spawn(int x, int y)
        {
            if (spawned != null)
                spawned();

            this.x = x;
            this.y = y;
            gameObject.SetActive(true);
            state = State.SafePath;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Call to hit the player
        /// </summary>
        public void Hit()
        {
            state = State.Dying;
            playArea.cutPath.Clear();
            animator.SetTrigger("Die");
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called after the "Die" animation
        /// </summary>
        void AnimationEvent_Respawn()
        {
            // store these in case it was the last life
            int nx = x; int ny = y;

            // clear up the cut path
            Rewind(rewindHistory.Count + 1);

            // can we respawn?
            if (Game.instance.livesLeft > 0)
            {
                gameObject.SetActive(false);
                Spawn(x, y);
            }

            // nope, we ded
            else
            {
                x = nx; y = ny;
                if (died != null)
                    died();
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary> 
        /// Rewinds the last cut moments in history 
        /// </summary>
        /// <param name="left"> number of pixels to move </param>
        void Rewind(int left)
        {
            while (left > 0 && rewindHistory.Count > 0)
            {
                playArea.mask[x, y] = PlayArea.Shadowed;
                Point pt = rewindHistory.Pop();
                x = pt.x;
                y = pt.y;
                left--;
            }

            if (rewindHistory.Count == 0)
                state = State.SafePath;

            playArea.cutPath.RedrawPath(cutPathStart.x, cutPathStart.y);
            playArea.mask.Apply();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Move into the cut area, cutting it
        /// </summary>
        /// <param name="left"> number of pixels to move </param>
        void CutShadow(int left)
        {
            // this allowes me to move in one direction only
            // much like the original Qix or Gals Panic
            if (Input.GetButtonDown("Horizontal"))
                direction = Direction.Horizontal;
            else if (Input.GetButtonUp("Horizontal") && Input.GetButton("Vertical"))
                direction = Direction.Vertical;

            if (Input.GetButtonDown("Vertical"))
                direction = Direction.Vertical;
            else if (Input.GetButtonUp("Vertical") && Input.GetButton("Horizontal"))
                direction = Direction.Horizontal;

            int dx = 0, dy = 0;
            if (direction == Direction.Horizontal)
            {
                dx = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                dy = 0;
            }

            if (direction == Direction.Vertical)
            {
                dx = 0;
                dy = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            }

            // no movement
            if (dx == 0 && dy == 0)
            {
                direction = Direction.None;
                return;
            }

            // next position
            int nx = x;
            int ny = y;

            bool closed = false;
            while (left > 0)
            {
                left--;

                // out of bounds
                if (nx + dx >= PlayArea.ImageWidth || nx + dx < 0 ||
                    ny + dy >= PlayArea.ImageHeight || ny + dy < 0)
                    continue;

                // look ahead to see if there are any cut paths
                // that may block the way. Leave at least 1 px
                // open in that situation
                if (!CanCutCheck(nx, ny, dx, dy))
                    break;

                // add point to history
                rewindHistory.Push(new Point(nx, ny));
                nx += dx;
                ny += dy;

                // went back to safe path? -> close
                closed = playArea.mask[nx, ny] == PlayArea.Safe;
                if (closed)
                    break;

                // keep on cutting
                else
                    playArea.mask[nx, ny] = PlayArea.Cut;
            }

            x = nx;
            y = ny;

            if (closed)
            {
                rewindHistory.Clear();
                playArea.cutPath.Clear();
                playArea.mask.Clear(playArea.boss.x, playArea.boss.y);
                playArea.mask.Apply();

                state = State.SafePath;

                // if we already won, there's nothing else to do
                if (playArea.mask.clearedRatio >= Config.instance.clearRatio)
                    return;

                // the path might have dissappeared
                // this happens when we fill an entire section all the way
                // up to the border sometimes
                // if that's the case, move the player back to where it
                // began cutting
                if (playArea.mask[x, y] != PlayArea.Safe)
                {
                    x = cutPathStart.x;
                    y = cutPathStart.y;
                }

                playArea.safePath.RedrawPath(x, y);
            }

            else
                playArea.cutPath.RedrawPath(cutPathStart.x, cutPathStart.y);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Check if the position we want to cut into is valid
        /// </summary>
        /// <param name="sx">Starting point x</param>
        /// <param name="sy">Starting point y</param>
        /// <param name="dx">Delta move x (1px max)</param>
        /// <param name="dy">Delta move y (1px max)</param>
        /// <returns></returns>
        bool CanCutCheck(int sx, int sy, int dx, int dy)
        {
            dx *= 2; // look 2 spaces ahead (if there are two spaces ahead
            dy *= 2;
            if (dx != 0)
            {
                for (int i = y - 1; i <= y + 1; i++)
                {
                    if (i < 0 || i >= PlayArea.ImageHeight)
                        continue;
                    
                    // if the look ahead goes outside the area
                    // you should be able to cut up until the border
                    int sdx = sx + dx;
                    if (sdx < 0 || sdx >= PlayArea.ImageWidth)
                        continue;

                    if (playArea.mask[sdx, i] == PlayArea.Cut)
                        return false;
                }
            }
            else if (dy != 0)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    if (i < 0 || i >= PlayArea.ImageWidth)
                        continue;
                    // if the look ahead goes outside the area
                    // you should be able to cut up until the border
                    int sdy = sy + dy;
                    if (sdy < 0 || sdy >= PlayArea.ImageHeight)
                        continue;

                    if (playArea.mask[i, sdy] == PlayArea.Cut)
                        return false;
                }
            }
            return true;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Moves the player on the safe path
        /// Basically, it checks on each direction of movement
        /// to see if there's a safe tile to move to, and moves
        /// there if it can
        /// </summary>
        void MoveOnSafePath(int left)
        {
            int dx = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            int dy = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

            int nx = x;
            int ny = y;

            // +x
            while (left > 0 && dx > 0 && nx < PlayArea.ImageWidth - 1 && 
                playArea.mask[nx + 1, ny] == PlayArea.Safe)
            {
                left--;
                nx++;
            }

            // -x
            while (left > 0 && dx < 0 && nx > 0 &&
                playArea.mask[nx - 1, ny] == PlayArea.Safe)
            {
                left--;
                nx--;
            }

            // +y 
            while (left > 0 && dy > 0 && ny < PlayArea.ImageHeight - 1 &&
                playArea.mask[nx, ny + 1] == PlayArea.Safe)
            {
                left--;
                ny++;
            }

            // -y
            while (left > 0 && dy < 0 && ny > 0 &&
                playArea.mask[nx, ny - 1] == PlayArea.Safe)
            {
                left--;
                ny--;
            }

            x = nx;
            y = ny;
        }

    }
}