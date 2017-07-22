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

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
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
    }
}