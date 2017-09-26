using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
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

        static readonly Color CycleColor1 = new Color(0.2f, 0.2f, 0.4f, 1);
        static readonly Color CycleColor2 = new Color(0.4f, 0.4f, 0.7f, 1);
        const float CycleTime = 3f;

        public const int EdgesLayerMask = 1 << 8;
        public static readonly ContactFilter2D EdgeContactFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = EdgesLayerMask
        };
        public const int EnemiesLayerMask = 1 << 9;

        public const int BonusesLayerMask = 1 << 10;

        /// <summary> Width of the main game image in landscape mode, in pixels </summary>
        public const int LandscapeWidth = 960;

        /// <summary> Height of the main game image in landscape mode, in pixels </summary>
        public const int LandscapeHeight = 540;

        // --- Static Properties ------------------------------------------------------------------------
        /// <summary> Image width. Changes if it's portrait mode or not </summary>
        public static int imageWidth { get; private set; }

        /// <summary> Image height . Changes if it's portrait mode or not </summary>
        public static int imageHeight { get; private set; }

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

        /// <summary> Effects available on the play area </summary>
        public Effects effects
        {
            get
            {
                if (_effects == null)
                    _effects = GetComponentInChildren<Effects>();
                return _effects;
            }
        }
        Effects _effects;

        /// <summary> Current boss. Only becomes valid after setting up the play area </summary>
        public Enemy boss { get; private set; }

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void Update()
        {
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
            bool isLandscape = baseImage.width == LandscapeWidth && baseImage.height == LandscapeHeight;
            imageWidth = isLandscape ? LandscapeWidth : LandscapeHeight;
            imageHeight = isLandscape ? LandscapeHeight: LandscapeWidth;

            // Deactivate all enemies
            Enemy[] enemies = GetComponentsInChildren<Enemy>();
            foreach (Enemy enemy in enemies)
                enemy.gameObject.SetActive(false);

            // select boss
            Enemy[] bosses = GetBosses();
            boss = System.Array.Find(bosses, b => b.GetType() == bossType);

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
            pos.x = -imageWidth * 0.5f;
            pos.y = -imageHeight * 0.5f;
            safePath.transform.localPosition = pos;
            cutPath.transform.localPosition = pos;
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary> Retreive a list of bosses for the play area </summary>
        public Enemy [] GetBosses()
        {
            Enemy[] enemies = GetComponentsInChildren<Enemy>(true);
            return System.Array.FindAll(enemies, e => e.isBoss);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Called after winning the game to discover the remainings of the shadow
        /// </summary>
        public IEnumerator DiscoverShadow()
        {
            safePath.Clear();
            cutPath.Clear();

            bool cancelWait = false;
            for (int y = imageHeight - 1; y >= 0; y--)
            {
                for (int x = 0; x < imageWidth; x++)
                    mask[x, y] = Cleared;

                // cancel the slow "discover" and just clear the whole image
                // if the player hits the fire button
                if (Input.GetButtonDown("Cut"))
                    cancelWait = true;

                if (!cancelWait)
                {
                    yield return null;
                    mask.Apply();
                }
            }

            if (cancelWait)
                mask.Apply();
        }

        // -----------------------------------------------------------------------------------	
        private void OnMaskCleared(Point center)
        {
            UI.instance.percentageBar.percentage = mask.clearedRatio * 100f;
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Simple utility to turn a mask coordinate into a world coordinate
        /// </summary>
        public Vector2 MaskPositionToWorld(int x, int y)
        {
            return MaskPositionToWorld(new Vector2(x, y));
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Simple utility to turn a mask coordinate into a world coordinate
        /// </summary>
        public Vector2 MaskPositionToWorld(Vector2 pos)
        {
            pos = pos - new Vector2(imageWidth, imageHeight) * 0.5f;
            return transform.TransformPoint(pos);
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
            float w = imageWidth * 0.5f;
            float h = imageHeight * 0.5f;

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
    }
}