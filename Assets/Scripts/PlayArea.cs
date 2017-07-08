using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public partial class PlayArea : MonoBehaviour
    {
        
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        public const byte Shadowed = 255;
        public const byte Cleared = 0;
        public const byte Safe = 128;
        public const byte Cut = 64;

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        PathRenderer safePath = null;

        [SerializeField]
        PathRenderer cutPath = null;

        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Width of the play area / image, in pixels </summary>
        public int width { get; private set; }

        /// <summary> Height of the play area / image, in pixels </summary>
        public int height { get; private set; }

        /// <summary> Mask object with the clear, shadowed areas and paths </summary>
        public Mask mask { get; private set; }

        /// <summary> Material assigned to the quad. </summary>
        Material material;

        /// <summary> Player running on the play area </summary>
        public Player player
        {
            get
            {
                if (_player == null)
                    _player = GetComponentInChildren<Player>();
                return _player;
            }
        }
        Player _player;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            // test values
            width = 845;
            height = 450;

            // create a new mask
            mask = new Mask(width, height);
            mask.maskCleared += OnMaskCleared;

            // material currently assigned
            material = GetComponent<MeshRenderer>().material;
            material.SetTexture("_Mask", mask.texture);

            // create a quad big enough to fit the image
            CreateQuad();

            // transform the path's local position
            // to account for quad size
            Vector3 pos = safePath.transform.localPosition;
            pos.x = -width * 0.5f;
            pos.y = -height * 0.5f;
            safePath.transform.localPosition = pos;
            cutPath.transform.localPosition = pos;

            // place the player in its initial position
            player.x = 10;
            player.y = 10;

            // create a starting zone
            CreateStartingZone(10, 10, 150, 100);

            print(Screen.width + ", " + Screen.height);
        }
        
        // -----------------------------------------------------------------------------------	
        void Update()
        {
            CenterOnPlayer();
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            safePath.RedrawPath(player.x, player.y);
            UI.instance.percentage = mask.clearedRatio * 100f;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Creates the starting zone where the player begins
        /// </summary>
        void CreateStartingZone(int x, int y, int w, int h)
        {
            for (int i = x; i < x + w; i++)
            {
                mask[i, y] = Safe;
                mask[i, y + h - 1] = Safe;
            }

            for (int i = y; i < y + h; i++)
            {
                mask[x, i] = Safe;
                mask[x + w - 1, i] = Safe;
            }
            mask.Clear(1, 1);
            mask.Apply();
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Creates the quad mesh to display the images
        /// </summary>
        void CreateQuad()
        {
            float w = width * 0.5f;
            float h = height * 0.5f;

            Vector3[] corners = new Vector3[]
            {
                new Vector3(-w, -h),
                new Vector3(-w,  h),
                new Vector3( w,  h),
                new Vector3( w, -h),
            };

            Vector2[] uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };

            int[] tris = new int[]
            {
                0, 1, 2,
                3, 0, 2
            };

            Mesh quad = new Mesh();
            quad.vertices = corners;
            quad.uv = uv;
            quad.triangles = tris;

            MeshFilter mf = GetComponent<MeshFilter>();
            mf.mesh = quad;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Centers the playarea on screen so the player is on the center
        /// as long as the image fits on the screen without gaps
        /// </summary>
        void CenterOnPlayer()
        {
            float w2 = 0, h2 = 0;
            if (Screen.width <= width)
                w2 = (width - Screen.width) * 0.5f;
            if (Screen.height <= height)
                h2 = (height - Screen.height) * 0.5f;

            Vector2 offset = player.transform.position;
            Vector2 position = (Vector2)transform.position - offset;
            position.x = Mathf.Clamp(position.x, -w2, w2);
            position.y = Mathf.Clamp(position.y, -h2, h2);
            transform.position = position;
        }
    }
}