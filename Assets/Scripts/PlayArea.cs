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

        public const int ImageWidth = 960;
        public const int ImageHeight = 540;

        static readonly Color CycleColor1 = new Color(0.2f, 0.2f, 0.4f, 1);
        static readonly Color CycleColor2 = new Color(0.4f, 0.4f, 0.7f, 1);
        const float CycleTime = 3f;

        public const int EdgesLayerMask = 1 << 8;
        public static readonly ContactFilter2D EdgeContactFilter = new ContactFilter2D() { layerMask = EdgesLayerMask };

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        [SerializeField]
        PathRenderer _safePath = null;
        public PathRenderer safePath { get { return _safePath; } }

        [SerializeField]
        PathRenderer _cutPath = null;
        public PathRenderer cutPath { get { return _cutPath; } }

        // --- Properties -------------------------------------------------------------------------------
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

        /// <summary> Collider around the edges of the play area </summary>
        EdgeCollider2D edgeCollider;

        /// <summary> Current boss. Only becomes valid after setting up the play area </summary>
        public Enemy boss { get; private set; }

        /// <summary> List of all available bosses </summary>
        List<Enemy> availableBosses;

        /// <summary> Zoom used for the pixel perfect adjuster </summary>
        float cameraZoom;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Start()
        {
            cameraZoom = Camera.main.GetComponent<PixelPerfect>().zoom;
        }

        // -----------------------------------------------------------------------------------	
        void Update()
        {
            CenterOnPlayer();

            // cycles the color on the shadow
            float t = Mathf.PingPong(Time.time, CycleTime) / CycleTime;
            material.SetColor("_ShadowColor1", Color.Lerp(CycleColor1, CycleColor2, t));
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Sets up the play area for the next game
        /// </summary>
        /// <param name="baseImage">Base image to discover</param>
        /// <param name="shadowImage">Shadow image covering the base image</param>
        /// <param name="bossType">Boss to activate for this playthrough</param>
        public void Setup(Texture2D baseImage, Texture2D shadowImage, System.Type bossType)
        {
            // do some basic validity check
            if (baseImage.width != ImageWidth || baseImage.height != ImageHeight)
                throw new System.Exception("Invalid Base image size");
            if (shadowImage.width != ImageWidth || shadowImage.height != ImageHeight)
                throw new System.Exception("Invalid Shadow image size");
            if (shadowImage.format != TextureFormat.Alpha8)
                throw new System.Exception("Shadow image is not of type Alpha8");

            // Deactivate all enemies
            Enemy[] enemies = GetComponentsInChildren<Enemy>();
            availableBosses = new List<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
                if (enemy.isBoss)
                    availableBosses.Add(enemy);
            }

            // select boss
            boss = availableBosses.Find(b => b.GetType() == bossType);

            // create a new mask
            mask = new Mask(shadowImage);
            mask.maskCleared += OnMaskCleared;
            mask.Apply();

            // material currently assigned
            material = GetComponent<MeshRenderer>().material;
            material.SetTexture("_BaseImage", baseImage);
            material.SetTexture("_Shadow", shadowImage);
            material.SetTexture("_Mask", mask.texture);

            // create a quad big enough to fit the image
            CreateQuad();

            // transform the path's local position
            // to account for quad size
            Vector3 pos = safePath.transform.localPosition;
            pos.x = -ImageWidth * 0.5f;
            pos.y = -ImageHeight * 0.5f;
            safePath.transform.localPosition = pos;
            cutPath.transform.localPosition = pos;
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared()
        {
            UI.instance.percentage = mask.clearedRatio * 100f;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Creates the starting zone where the player begins
        /// </summary>
        public void CreateStartingZone(IntRect rect)
        {
            int x = rect.x;
            int y = rect.y;
            int w = rect.width;
            int h = rect.height;

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

            // boss cannot be placed until 
            // the play area has been fully set up
            // so use this to do the first fill
            int fillX = rect.x - 1;
            int fillY = rect.y - 1;
            mask.Clear(fillX, fillY); 
            mask.Apply();

            safePath.RedrawPath(x, y);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Creates the quad mesh to display the images
        /// </summary>
        void CreateQuad()
        {
            float w = ImageWidth * 0.5f;
            float h = ImageHeight * 0.5f;

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

            // add a collider to the quad and a RB to make it a trigger
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider.edgeRadius = 1;
            edgeCollider.points = new Vector2[] {
                corners[0], corners[1], corners[2], corners[3], corners[0]
            };
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Centers the playarea on screen so the player is on the center
        /// as long as the image fits on the screen without gaps
        /// </summary>
        void CenterOnPlayer()
        {
            float scr_w = Screen.width / cameraZoom;
            float scr_h = Screen.height / cameraZoom;

            float w2 = 0, h2 = 0;
            if (scr_w <= ImageWidth)
                w2 = (ImageWidth - scr_w) * 0.5f;
            if (scr_h <= ImageHeight)
                h2 = (ImageHeight - scr_h) * 0.5f;

            Vector2 offset = player.transform.position;
            Vector2 position = (Vector2)transform.position - offset;
            position.x = Mathf.Clamp(position.x, -w2, w2);
            position.y = Mathf.Clamp(position.y, -h2, h2);
            transform.position = position;
        }
    }
}