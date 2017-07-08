using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple constraints for rigidbody movement
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyConstraints : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        [System.Flags]
        public enum Axis
        {
            X = 0x01,
            Y = 0x02,
            Z = 0x04
        }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, EnumFlag]
        Axis lockPosition;

        [SerializeField, EnumFlag]
        Axis lockRotation;

        // --- Properties -------------------------------------------------------------------------------
        Vector3 startingPosition;
        Vector3 startingRotation;

        new Rigidbody rigidbody;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        IEnumerator Start()
        {
            // give the physics engine a few moments to settle
            yield return new WaitForSeconds(2f);

            rigidbody = GetComponent<Rigidbody>();
            startingPosition = transform.position;
            startingRotation = transform.eulerAngles;
        }

        // -----------------------------------------------------------------------------------
        void LateUpdate()
        {
            if (rigidbody == null)
                return;

            if (lockPosition != 0)
            {
                Vector3 position = transform.position;
                if ((lockPosition & Axis.X) != 0) position.x = startingPosition.x;
                if ((lockPosition & Axis.Y) != 0) position.y = startingPosition.y;
                if ((lockPosition & Axis.Z) != 0) position.z = startingPosition.z;
                transform.position = position;
            }

            if (lockRotation != 0)
            {
                Vector3 rotation = transform.eulerAngles;
                if ((lockRotation & Axis.X) != 0) rotation.x = startingRotation.x;
                if ((lockRotation & Axis.Y) != 0) rotation.y = startingRotation.y;
                if ((lockRotation & Axis.Z) != 0) rotation.z = startingRotation.z;
                transform.eulerAngles = rotation;
            }
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
    }
}