﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class PathRenderer : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        /// <summary> Called after the path changed shape. Argument is array of points for the new path </summary>
        public event System.Action<Vector3[]> pathRedrawn;

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

        /// <summary> List of 3D points used by the line renderer (local coordinates) </summary>
        public Vector3 [] points { get; private set; }

        /// <summary> Edge collider </summary>
        public new EdgeCollider2D collider { get; private set; }

        /// <summary> Color of the line renderer </summary>
        public Color color
        {
            get { return lineRenderer.material.color; }
            set { lineRenderer.material.color = value; }
        }
        

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Awake()
        {
            collider = gameObject.AddComponent<EdgeCollider2D>();
            collider.edgeRadius = 1;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Clear()
        {
            lineRenderer.positionCount = 0;
            lineRenderer.SetPositions(new Vector3[]{});
            collider.points = new Vector2[] { Vector2.zero, Vector2.zero };
        }

        // -----------------------------------------------------------------------------------	
        /*
        public void RedrawPath()
        {
            // find at least one point in the path
            int x = 0;
            int y = 0;
            bool found = false;
            for (; x < PlayArea.imageWidth && !found; x++)
            {
                for (; y < PlayArea.imageHeight && !found; y++)
                    found = playArea.mask[x, y] == pathType;
            }

            if (found)
                RedrawPath(x, y);
        }
        */
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
            this.points = points.ToArray();

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(this.points);

            // update line renderer
            System.Converter<Vector3, Vector2> Vec3ToVec2 = (Vector3 v3) => { return v3; };
            List<Vector2> points2 = points.ConvertAll(Vec3ToVec2);
            if (lineRenderer.loop)
                points2.Add(points2[0]);

            // update collider
            if (points.Count > 1)
                collider.points = points2.ToArray();

            // raise event
            if (pathRedrawn != null)
                pathRedrawn(this.points);
        }
        
        // -----------------------------------------------------------------------------------	
        Direction GetNextPointDirection(Direction dir, int x, int y, out int ox, out int oy)
        {
            ox = x;
            oy = y;

            PlayArea.Mask mask = playArea.mask;
            if (dir != Direction.Lt && x + 1 < PlayArea.imageWidth && mask[x + 1, y] == pathType)
            {
                ox = x + 1;
                return Direction.Rt;
            }
            if (dir != Direction.Rt && x - 1 >= 0 && mask[x - 1, y] == pathType)
            {
                ox = x - 1;
                return Direction.Lt;
            }
            if (dir != Direction.Dw && y + 1 < PlayArea.imageHeight && mask[x, y + 1] == pathType)
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