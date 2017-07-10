using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
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
                pos.x = _x - PlayArea.ImageWidth * 0.5f;
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
                pos.y = _y - PlayArea.ImageHeight * 0.5f;
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
        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}