using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField, Tooltip("Must be 1, 2, 4, 8 etc")]
        int zoom = 1;

        // --- Properties -------------------------------------------------------------------------------
        // needed to check if the screen size changed
        int lastWidth = -1;
        int lastHeight = -1;

        /// <summary> Are we tracking the player? </summary>
        bool tracking = true;

        /// <summary> Camera object to adjust </summary>
        new Camera camera;

        /// <summary> Current playarea to track </summary>
        PlayArea playArea;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            zoom = Config.instance.zoom;
            camera = GetComponent<Camera>();
        }

        // -----------------------------------------------------------------------------------	
        void OnPreRender()
        {
            if (!tracking)
                return;
            AutoSize();
            TrackPlayer();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void AutoSize()
        {
            if (Screen.width == lastWidth && lastHeight == Screen.height)
                return;
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            camera.orthographicSize = Screen.height * (0.5f / zoom);
        }
        
        // -----------------------------------------------------------------------------------	
        void TrackPlayer()
        {
            float scr_w = Screen.width / zoom;
            float scr_h = Screen.height / zoom;
            float img_w = PlayArea.imageWidth;
            float img_h = PlayArea.imageHeight;

            float w2 = 0, h2 = 0;
            if (scr_w <= img_w)
                w2 = (img_w - scr_w) * 0.5f;
            if (scr_h <= img_h)
                h2 = (img_h - scr_h) * 0.5f;

            // put the camera right on top of the player
            Vector3 position = playArea.player.transform.position;

            // do not go outside the play area if we can avoid it
            position.x = Mathf.Clamp(position.x, -w2, w2);
            position.y = Mathf.Clamp(position.y, -h2, h2);

            // maintain z
            position.z = transform.position.z;
            transform.position = position;
        }

        // -----------------------------------------------------------------------------------	
        public void StartTracking(PlayArea playArea)
        {
            this.playArea = playArea;
            tracking = true;
            lastWidth = -1;
            lastHeight = -1;
        }
        
        // -----------------------------------------------------------------------------------	
        public void StopTracking(bool zoomToImage)
        {
            tracking = false;

            if (zoomToImage)
                StartCoroutine(ZoomToImageCoroutine());
        }

        // -----------------------------------------------------------------------------------	
        IEnumerator ZoomToImageCoroutine()
        {
            const float ZoomTime = 1f;

            // start with the current orthographic size
            float size_0 = camera.orthographicSize;

            // fit to image width or height, depending on the aspect ratio of the screen
            float size_1 = 0;
            float imageAspect = PlayArea.imageWidth / (float)PlayArea.imageHeight;
            float screenAspect = Screen.width / (float)Screen.height;

            // screen is wider
            if (screenAspect > imageAspect)
                size_1 = PlayArea.imageHeight * 0.5f; // fit to height
            
            // image is wider
            else
                size_1 = (PlayArea.imageWidth / screenAspect) * 0.5f; // fit to width

            // center the camera too
            Vector3 pos_0 = transform.position;
            Vector3 pos_1 = new Vector3(0, 0, pos_0.z);

            // lerp so that the entire image fits on the screen
            float elapsed = 0;
            while(elapsed < ZoomTime && !tracking)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / ZoomTime);
                t = IllogicGate.Tweener.EaseOut(t);
                camera.orthographicSize = Mathf.Lerp(size_0, size_1, t);
                camera.transform.position = Vector3.Lerp(pos_0, pos_1, t);
                yield return null;
            }

            if (!tracking)
                camera.orthographicSize = size_1;
        }
    }
}