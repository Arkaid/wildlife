using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class Player : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> Possible states for the player </summary>
        enum State
        {
            SafePath,
            StartCut,
            Cutting,
            Rewinding,
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
        [SerializeField]
        PathRenderer cutPath = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Current play area </summary>
        PlayArea playArea
        {
            get
            {
                if (_playArea == null)
                    _playArea = GetComponentInParent<PlayArea>();
                return _playArea;
            }
        }
        PlayArea _playArea;

        /// <summary> X position, in pixels. Automatically adjusts position in world space </summary>
        public int x
        {
            get { return _x; }
            set
            {
                _x = value;
                Vector3 pos = transform.localPosition;
                pos.x = _x - playArea.width * 0.5f;
                transform.localPosition = pos;
            }
        }
        int _x;

        /// <summary> T position, in pixels. Automatically adjusts position in world space </summary>
        public int y
        {
            get { return _y; }
            set
            {
                _y = value;
                Vector3 pos = transform.localPosition;
                pos.y = _y - playArea.height * 0.5f;
                transform.localPosition = pos;
            }
        }
        int _y;

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
            cutPath.gameObject.SetActive(false);
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
                        cutPath.gameObject.SetActive(true);
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
        /*
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Bressenham line algorithm
        /// src: https://stackoverflow.com/questions/11678693/all-cases-covered-bresenhams-line-algorithm
        /// </summary>
        IEnumerable<Point> IntermediatePoints(int dx, int dy)
        {
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (dx < 0) dx1 = dx2 = -1; else if (dx > 0) dx1 = dx2 = 1;
            if (dy < 0) dy1 = -1; else if (dy > 0) dy1 = 1;

            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);
            if (shortest > longest)
            {
                int tmp = shortest;
                shortest = longest;
                longest = tmp;
                if (dy < 0) dy2 = -1; else if (dy > 0) dy2 = 1;
                dx2 = 0;
            }

            int ptx = 0, pty = 0;
            int num = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                num += shortest;
                if (num >= longest)
                {
                    num -= longest;
                    ptx += dx1;
                    yield return new Point(ptx, pty);
                    pty += dy1;
                }
                else
                {
                    ptx += dx2;
                    pty += dy2;
                }
                yield return new Point(ptx, pty);
            }
        }
        */

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

            cutPath.RedrawPath(cutPathStart.x, cutPathStart.y);
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
                if (nx + dx >= playArea.width || nx + dx < 0 ||
                    ny + dy >= playArea.height || ny + dy < 0)
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
                cutPath.gameObject.SetActive(false);
                rewindHistory.Clear();
                playArea.mask.Clear(1, 1);
                state = State.SafePath;
            }

            cutPath.RedrawPath(cutPathStart.x, cutPathStart.y);
            playArea.mask.Apply();
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
                    if (i < 0 || i >= playArea.height)
                        continue;
                    
                    // if the look ahead goes outside the area
                    // you should be able to cut up until the border
                    int sdx = sx + dx;
                    if (sdx < 0 || sdx >= playArea.width)
                        continue;

                    if (playArea.mask[sdx, i] == PlayArea.Cut)
                        return false;
                }
            }
            else if (dy != 0)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    if (i < 0 || i >= playArea.width)
                        continue;
                    // if the look ahead goes outside the area
                    // you should be able to cut up until the border
                    int sdy = sy + dy;
                    if (sdy < 0 || sdy >= playArea.height)
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
            while (left > 0 && dx > 0 && nx < playArea.width - 1 && 
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
            while (left > 0 && dy > 0 && ny < playArea.height - 1 &&
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