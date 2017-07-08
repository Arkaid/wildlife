using UnityEngine;
using UnityEditor;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    [CustomEditor(typeof(LineRendererPhysics))]
    public class LineRendererPhysicsEditor : Editor
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------

        // --- Properties -------------------------------------------------------------------------------
        new LineRendererPhysics target;

        // properties of the target object
        SerializedProperty controlPointsProp;
        SerializedProperty linePointsProp;
        SerializedProperty jointsProp;
        SerializedProperty segmentsProp;
        SerializedProperty jointDragProp;
        SerializedProperty jointMassProp;

        /// <summary> Line renderer points. We need these to create the joints as well </summary>
        Vector3[] points;

        float lineLength;

        // --- Editor -----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        void OnEnable()
        {
            // initialize the editor
            target = base.target as LineRendererPhysics;
            controlPointsProp = serializedObject.FindProperty("controlPoints");
            linePointsProp = serializedObject.FindProperty("linePoints");
            jointsProp = serializedObject.FindProperty("joints");
            segmentsProp = serializedObject.FindProperty("segments");
            jointDragProp = serializedObject.FindProperty("jointDrag");
            jointMassProp = serializedObject.FindProperty("jointMass");

            // wrong number of control points: reset
            if (controlPointsProp.arraySize != 4)
            {
                controlPointsProp.arraySize = 4;
                controlPointsProp.GetArrayElementAtIndex(0).vector3Value = new Vector3(-6, 0, 0);
                controlPointsProp.GetArrayElementAtIndex(1).vector3Value = new Vector3(-3, 0, 0);
                controlPointsProp.GetArrayElementAtIndex(2).vector3Value = new Vector3( 3, 0, 0);
                controlPointsProp.GetArrayElementAtIndex(3).vector3Value = new Vector3( 6, 0, 0);

                serializedObject.ApplyModifiedProperties();
            }

            UpdateLineRender();
        }

        // -----------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(segmentsProp);
            EditorGUILayout.PropertyField(jointMassProp);
            EditorGUILayout.PropertyField(jointDragProp);

            if (EditorGUI.EndChangeCheck())
            {
                // something changed: update line renderer and joints
                UpdateLineRender();
                UpdateJoints();
                serializedObject.ApplyModifiedProperties();
            }
        }

        // -----------------------------------------------------------------------------------
        void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            // draw the 4 handles for each control point
            for (int i = 0; i < 4; i++)
            {
                // control points are in local coordinates
                Vector3 position = controlPointsProp.GetArrayElementAtIndex(i).vector3Value;
                position = target.transform.TransformPoint(position);
                position = Handles.PositionHandle(position, Quaternion.identity);
                position = target.transform.InverseTransformPoint(position);
                controlPointsProp.GetArrayElementAtIndex(i).vector3Value = position;
            }

            // draw the length of the line
            float size = HandleUtility.GetHandleSize(target.transform.position);
            Handles.Label(target.transform.position + Vector3.down * size, lineLength.ToString("0.000"));
            
            // if any control point changed, update 
            if (EditorGUI.EndChangeCheck())
            {
                UpdateLineRender();
                UpdateJoints();
                serializedObject.ApplyModifiedProperties();
            }
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Updates the points on the line renderer
        /// </summary>
        void UpdateLineRender()
        {
            LineRenderer line = (target as Component).GetComponent<LineRenderer>();
            line.useWorldSpace = false;

            // must at least have 1 segment
            int segments = segmentsProp.intValue;
            segments = Mathf.Max(segments, 1);

            // number of points on the line
            linePointsProp.arraySize = segments + 1;

            // update points both in the target and locally
            lineLength = 0;
            points = new Vector3[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                points[i] = CalculateBezierPoint(t,
                    controlPointsProp.GetArrayElementAtIndex(0).vector3Value,
                    controlPointsProp.GetArrayElementAtIndex(1).vector3Value,
                    controlPointsProp.GetArrayElementAtIndex(2).vector3Value,
                    controlPointsProp.GetArrayElementAtIndex(3).vector3Value);
                linePointsProp.GetArrayElementAtIndex(i).vector3Value = points[i];

                if (i > 0)
                    lineLength += Vector3.Distance(points[i - 1], points[i]);
            }

            // update line renderer
            line.SetVertexCount(segments + 1);
            line.SetPositions(points);
        }
        
        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Updates the hinge joints that make the cable physics
        /// </summary>
        void UpdateJoints()
        {
            int segments = segmentsProp.intValue;

            // set up the array for hinges
            if (jointsProp.arraySize != segments + 1)
                jointsProp.arraySize = segments + 1;

            // if there was a previous root hinge, remove it
            HingeJoint anchor = jointsProp.GetArrayElementAtIndex(0).objectReferenceValue as HingeJoint;
            if (anchor != null)
                DestroyImmediate(anchor.gameObject);

            // create a root hinge
            anchor = AddHingeJoint("Anchor", target.transform, null, 0);
            HingeJoint joint = anchor;
            joint.axis = points[1] - points[0];
            // create all child hinges
            for (int i = 1; i <= segments; i++)
            {
                Rigidbody connectedRB = joint.GetComponent<Rigidbody>();
                joint = AddHingeJoint("Joint", anchor.transform, connectedRB, i);
                joint.axis = points[i] - points[i - 1];
            }
            serializedObject.ApplyModifiedProperties();
        }

        // -----------------------------------------------------------------------------------
        /// <summary>
        /// Creates one hinge joint
        /// </summary>
        HingeJoint AddHingeJoint(string name, Transform parent, Rigidbody connectedBody, int index)
        {
            HingeJoint joint = new GameObject(name).AddComponent<HingeJoint>();
            joint.transform.SetParent(parent, true);
            joint.transform.position = points[index];
            joint.connectedBody = connectedBody;

            Rigidbody rb = joint.GetComponent<Rigidbody>();
            rb.mass = jointMassProp.floatValue;
            rb.angularDrag = jointDragProp.floatValue;

            jointsProp.GetArrayElementAtIndex(index).objectReferenceValue = joint;

            return joint;
        }
    }
}