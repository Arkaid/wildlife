using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Camera))]
    public class PixelPerfect : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Must be 1, 2, 4, 8 etc")]
        int _zoom = 1;
        public int zoom { get { return _zoom; } }

        // --- Properties -------------------------------------------------------------------------------
        int lastWidth = -1;
        int lastHeight = -1;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnPreRender()
        {
            if (Screen.width == lastWidth && lastHeight == Screen.height)
                return;
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            GetComponent<Camera>().orthographicSize = Screen.height * (0.5f / zoom);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}