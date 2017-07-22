using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Camera))]
    public class CameraAdjuster : MonoBehaviour
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

        /// <summary> Whether auto adujust should run or not </summary>
        public bool autoAdjust
        {
            get { return _autoAdjust; }
            set
            {
                _autoAdjust = value;
                lastWidth = -1;
                lastHeight = -1;
            }
        }
        bool _autoAdjust = true;


        /// <summary> Camera object to adjust </summary>
        new Camera camera;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            camera = GetComponent<Camera>();
        }

        // -----------------------------------------------------------------------------------	
        void OnPreRender()
        {
            if (!autoAdjust)
                return;

            if (Screen.width == lastWidth && lastHeight == Screen.height)
                return;
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            camera.orthographicSize = Screen.height * (0.5f / zoom);
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public void ZoomToImage()
        {
            StartCoroutine(ZoomToImageCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ZoomToImageCoroutine()
        {
            const float ZoomTime = 1f;

            // fit to image width or height, depending on the aspect ratio
            // of the screen
            float end = PlayArea.imageHeight * 0.5f;
            float imageAspect = PlayArea.imageWidth / (float)PlayArea.imageHeight;
            float screenAspect = Screen.width / (float)Screen.height;
            if (imageAspect > screenAspect)
                end = PlayArea.imageHeight * screenAspect * 0.5f;

            // turn off auto adjusting
            autoAdjust = false;

            // lerp so that the entire image fits on the screen
            float start = Screen.height * (0.5f / zoom);
            float elapsed = 0;
            while(elapsed < ZoomTime && !autoAdjust)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ZoomTime);
                camera.orthographicSize = Mathf.Lerp(start, end, t);
                yield return null;
            }

            if (!autoAdjust)
                camera.orthographicSize = end;
        }
    }
}