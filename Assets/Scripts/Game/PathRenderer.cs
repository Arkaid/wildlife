using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public class PathRenderer : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> 4 possible directions to search the path in</summary>
        enum Direction { Rt, Dw, Lt, Up, End }

        /// <summary> Type of path to render </summary>
        public enum Type
        {
            Safe,
            Cut
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Type type;

        [SerializeField]
        Material material = null;

        [SerializeField, Tooltip("Material to use when the Shield skill is used. Only valid for Cut type path")]
        Material shieldedMaterial = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Byte value to search on the mask </summary>
        byte pathValue { get { return type == Type.Safe ? PlayArea.Safe : PlayArea.Cut; } }
        
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

        /// <summary> Edge collider </summary>
        public List<EdgeCollider2D> colliders { get; private set; }

        // active line renderers
        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        /// <summary>Set to true to protect. Only works with the cut path </summary>
        [HideInInspector]
        public bool isShielded;

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
            lineRend.material = isShielded ? shieldedMaterial : material;
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
            List<List<Point>> segments = pb.GetSegments(playArea.mask, x, y, pathValue);

            // add play area edges, if needed
            if (type == Type.Safe)
                AddPlayAreaEdges(segments);

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
                    col.isTrigger = type == Type.Cut && !isShielded;
                    colliders.Add(col);
                }
            }
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks if we need to add the edges of the play area
        /// This is needed until the player connects the inner square with the
        /// outer edges
        /// </summary>
        /// <param name="segments"></param>
        void AddPlayAreaEdges(List<List<Point>> segments)
        {
            // check each corner. If it is "safe" but doesn't have a segment that passes
            // through that corner, we need to add them
            List<Point> corners = new List<Point>()
            {
                new Point(0, 0),
                new Point(0, PlayArea.imageHeight - 1),
                new Point(PlayArea.imageWidth - 1, PlayArea.imageHeight - 1),
                new Point(PlayArea.imageWidth - 1, 0),
                new Point(0, 0), // to loop back only
            };

            for (int i = 0; i < 4; i++)
            {
                Point pt = corners[i];

                // corner is not Safe -> can't draw safe path here
                if (playArea.mask[pt.x, pt.y] != PlayArea.Safe)
                    return;

                // all four corners need to be without a match for this to work
                // so if any corner is already covered by a segment, return
                if (segments.Find(seg => seg.Contains(pt)) != null)
                    return; 
            }

            // if we got this far, it means that all 4 corners are safe
            // AND neither has a segment going through them. Draw an extra path
            segments.Add(corners);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks every edge on the path to see if intersects the bounds, in world coordinates
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool Intersects(Bounds bounds)
        {
            foreach(LineRenderer rend in lineRenderers)
            {
                Vector3 pt_a = transform.TransformPoint(rend.GetPosition(0));
                pt_a.z = bounds.center.z;
                for (int i = 1; i < rend.positionCount; i++)
                {
                    Vector3 pt_b = transform.TransformPoint(rend.GetPosition(i));
                    pt_b.z = pt_a.z;

                    Vector3 line = pt_b - pt_a;
                    Ray ray = new Ray(pt_a, line);
                    float distance;
                    if (bounds.IntersectRay(ray, out distance) && distance <= line.magnitude)
                        return true;

                    pt_a = pt_b;
                }

            }
            return false;
        }
    }
}