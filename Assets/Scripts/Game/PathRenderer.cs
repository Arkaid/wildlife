using System.Collections;
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

        [SerializeField]
        Material material = null;

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

        /// <summary> List of 3D points used by the line renderer (local coordinates) </summary>
        public Vector3 [] points { get; private set; }

        /// <summary> Edge collider </summary>
        public List<EdgeCollider2D> colliders { get; private set; }

        /// <summary> Color of the line renderer </summary>
        public Color color
        {
            get { return material.color; }
            set { material.color = value; }
        }

        // active line renderers
        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Awake()
        {
            colliders = new List<EdgeCollider2D>();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void Clear()
        {
            foreach (LineRenderer seg in lineRenderers)
                Destroy(seg.gameObject);
            lineRenderers = new List<LineRenderer>();

            foreach (EdgeCollider2D col in colliders)
                Destroy(col);
            colliders = new List<EdgeCollider2D>();
        }
        
        // -----------------------------------------------------------------------------------	
        LineRenderer CreateLineRenderer()
        {

            LineRenderer lineRend = new GameObject("Line Segment").AddComponent<LineRenderer>();
            lineRend.material = material;
            lineRend.startWidth = 2;
            lineRend.endWidth = 2;
            lineRend.receiveShadows = false;
            lineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRend.useWorldSpace = false;

            lineRend.transform.SetParent(transform);
            lineRend.transform.localScale = Vector3.one;
            lineRend.transform.localPosition = Vector3.zero;

            return lineRend;
        }

        // -----------------------------------------------------------------------------------	
        public void RedrawPath(int x, int y)
        {
            Clear();

            PathBuilder pb = new PathBuilder();
            List<List<Point>> segments = pb.GetSegments(playArea.mask, x, y, pathType);

            System.Converter<Point, Vector3> Point2Vec3 = (Point pt) => { return (Vector2)pt; };
            System.Converter<Point, Vector2> Point2Vec2 = (Point pt) => { return pt; };

            foreach (List<Point> segment in segments)
            {
                // create a new line renderer for the segment
                LineRenderer newLine = CreateLineRenderer();
                lineRenderers.Add(newLine);

                newLine.positionCount = segment.Count;
                newLine.SetPositions(segment.ConvertAll(Point2Vec3).ToArray());

                // check if the segment is looping (first and last position are 1 px apart)
                if (Vector2.Distance(segment[0], segment[segment.Count - 1]) == 1)
                    newLine.loop = true;

                // add a collider for the segment
                if (segment.Count >= 2)
                {
                    EdgeCollider2D col = gameObject.AddComponent<EdgeCollider2D>();
                    col.edgeRadius = 1;
                    col.points = segment.ConvertAll(Point2Vec2).ToArray();
                    colliders.Add(col);
                }
            }
        }
        
    }
}