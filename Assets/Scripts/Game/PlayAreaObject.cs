using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public abstract class PlayAreaObject : MonoBehaviour
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> X position, in pixels. Automatically adjusts position in world space </summary>
        public int x
        {
            get { return _x; }
            set
            {
                _x = value;
                Vector3 pos = transform.localPosition;
                pos.x = _x - PlayArea.imageWidth * 0.5f;
                transform.localPosition = pos;
            }
        }
        int _x;

        /// <summary> Y position, in pixels. Automatically adjusts position in world space </summary>
        public int y
        {
            get { return _y; }
            set
            {
                _y = value;
                Vector3 pos = transform.localPosition;
                pos.y = _y - PlayArea.imageHeight * 0.5f;
                transform.localPosition = pos;
            }
        }
        int _y;

        /// <summary> Current play area </summary>
        public PlayArea playArea
        {
            get
            {
                if (_playArea == null)
                    _playArea = GetComponentInParent<PlayArea>();
                return _playArea;
            }
        }
        PlayArea _playArea;

        /// <summary> Animator, if available. </summary>
        public Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();
                return _animator;
            }
        }
        Animator _animator;

        /// <summary> Collider, if available </summary>
        public new Collider2D collider
        {
            get
            {
                if (_collider == null)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }
        Collider2D _collider;

    // --- MonoBehaviour ----------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------	
    virtual protected void Start()
        {
            Controller.instance.paused += OnPaused;
        }

        // -----------------------------------------------------------------------------------	
        virtual protected void OnDestroy()
        {
            if (Controller.instance != null)
                Controller.instance.paused -= OnPaused;
        }

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        void OnPaused(bool pause)
        {
            if (animator != null)
                animator.speed = pause ? 0 : 1;
        }

        // -----------------------------------------------------------------------------------	
        void AnimationEvent_PlaySound(string clip)
        {
            SoundManager.instance.PlaySFX(clip);
        }
        
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Updates the values of x and y from the local position
        /// You should call this if you move the object by its transform
        /// and not by the x and y properties
        /// </summary>
        public void SetXYFromLocalPosition()
        {
            Vector2 pos = transform.localPosition;

            _x = Mathf.RoundToInt(pos.x + PlayArea.imageWidth * 0.5f);
            _x = Mathf.Clamp(_x, 0, PlayArea.imageWidth - 1);

            _y = Mathf.RoundToInt(pos.y + PlayArea.imageHeight * 0.5f);
            _y = Mathf.Clamp(_y, 0, PlayArea.imageHeight - 1);
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Checks whether this object is in the shadow or not. Requires a collider
        /// If object does not have a collider it will throw an exception
        /// </summary>
        /// <returns></returns>
        public bool IsInShadow()
        {
            if (collider == null)
                throw new System.Exception("Cannot determine size without a collider");

            // first check: is the position within the shadow?
            if (playArea.mask[x, y] != PlayArea.Shadowed)
                return false;

            // second check. Are the bounds intersecting with the safe path in any way?
            return !playArea.safePath.Intersects(collider.bounds);
        }

        // -----------------------------------------------------------------------------------	
        private Collider2D[] overlaps = new Collider2D[16];
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Returns true if there is another play area object in the same position (requires collider)
        /// </summary>
        /// <returns></returns>
        public bool IsOverlapping()
        {
            if (collider == null)
                throw new System.Exception("Cannot determine size without a collider");

            Bounds bounds = collider.bounds;
            int hits = Physics2D.OverlapAreaNonAlloc(bounds.min, bounds.max, overlaps, PlayArea.EnemiesLayerMask | PlayArea.BonusesLayerMask);

            bool result = false;
            for (int i = 0; i < hits && !result; i++)
            {
                // it only overlaps if it's not the same object AND if that object is not a child
                result = overlaps[i].gameObject != gameObject && 
                        !overlaps[i].transform.IsChildOf(transform);
            }

            return result;
        }

        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Places this object randomly on the play area. It requires a collider to estimate size
        /// </summary>
        public void PlaceRandomly()
        {
            // find a valid position in the shadow
            do
            {
                x = Random.Range(0, PlayArea.imageWidth);
                y = Random.Range(0, PlayArea.imageHeight);
            } while (!IsInShadow() || IsOverlapping());
        }
    }
}