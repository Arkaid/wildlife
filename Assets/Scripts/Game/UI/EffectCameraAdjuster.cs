using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Makes sure that the camera is the same size as the canvas, so we can render at the same scale
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class EffectCameraAdjuster : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        Canvas targetCanvas = null;

        // --- Properties -------------------------------------------------------------------------------
        new Camera camera;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        // -----------------------------------------------------------------------------------	
        private void Update()
        {
            camera.orthographicSize = targetCanvas.pixelRect.height / 2;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}