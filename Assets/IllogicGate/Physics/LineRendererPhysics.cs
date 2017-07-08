using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererPhysics : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        int segments;

        [SerializeField]
        Vector3 [] controlPoints;

        [SerializeField]
        Vector3[] linePoints; // to update line renderer when the joints move

        [SerializeField]
        HingeJoint [] joints; // to keep track of each joint in lateupdate

        [SerializeField]
        float jointDrag;

        [SerializeField]
        float jointMass;

        // --- Properties -------------------------------------------------------------------------------
        LineRenderer line;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        void Awake()
        {
            line = GetComponent<LineRenderer>();
        }
        // -----------------------------------------------------------------------------------
        void LateUpdate()
        {
            for (int i = 0; i < linePoints.Length; i++)
                linePoints[i] = joints[i].transform.position;
            line.SetPositions(linePoints);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
    }
}