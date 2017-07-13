using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class PathRenderer : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> 4 possible directions to search the path in</summary>
        enum Direction { Rt, Dw, Lt, Up, End }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Byte value to search in mask for the path (Cut:64, Safe:128)")]
        byte pathType = 128;

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

        /// <summary> Line renderer </summary>
        LineRenderer lineRenderer
        {
            get
            {
                if (_lineRenderer == null)
                    _lineRenderer = GetComponentInParent<LineRenderer>();
                return _lineRenderer;
            }
        }
        LineRenderer _lineRenderer;

        /// <summary> Edge collider </summary>
        EdgeCollider2D edgeCollider;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Awake()
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider.edgeRadius = 1;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void RedrawPath()
        {
            // find at least one point in the path
            int x = 0;
            int y = 0;
            bool found = false;
            for (; x < PlayArea.ImageWidth && !found; x++)
            {
                for (; y < PlayArea.ImageHeight && !found; y++)
                    found = playArea.mask[x, y] == pathType;
            }

            if (found)
                RedrawPath(x, y);
        }

        // -----------------------------------------------------------------------------------	
        public void RedrawPath(int x, int y)
        {
            // go around the path, trying to find where it changes direction
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector2(x, y));

            int sx = x; int sy = y;
            Direction dir = GetNextPointDirection(Direction.End, sx, sy, out x, out y);
            while (!(sx == x && sy == y) && dir != Direction.End)
            {
                int tx = x;
                int ty = y;
                Direction next = GetNextPointDirection(dir, tx, ty, out x, out y);
                if (next != dir)
                {
                    dir = next;
                    points.Add(new Vector2(tx, ty));
                }
            };

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());

            System.Converter<Vector3, Vector2> Vec3ToVec2 = (Vector3 v3) => { return v3; };
            List<Vector2> points2 = points.ConvertAll(Vec3ToVec2);
            if (lineRenderer.loop)
                points2.Add(points2[0]);

            if (points.Count > 1)
                edgeCollider.points = points2.ToArray();
        }
        
        // -----------------------------------------------------------------------------------	
        Direction GetNextPointDirection(Direction dir, int x, int y, out int ox, out int oy)
        {
            ox = x;
            oy = y;

            PlayArea.Mask mask = playArea.mask;
            if (dir != Direction.Lt && x + 1 < PlayArea.ImageWidth && mask[x + 1, y] == pathType)
            {
                ox = x + 1;
                return Direction.Rt;
            }
            if (dir != Direction.Rt && x - 1 >= 0 && mask[x - 1, y] == pathType)
            {
                ox = x - 1;
                return Direction.Lt;
            }
            if (dir != Direction.Dw && y + 1 < PlayArea.ImageHeight && mask[x, y + 1] == pathType)
            {
                oy = y + 1;
                return Direction.Up;
            }
            if (dir != Direction.Up && y - 1 >= 0 && mask[x, y - 1] == pathType)
            {
                oy = y - 1;
                return Direction.Dw;
            }

            return Direction.End; // path ended!
        }
    }
}