using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori
{
    // --- Class Declaration ------------------------------------------------------------------------
    public struct IntRect
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        Rect baseRect;

        public int x { get { return (int)baseRect.x; } set { baseRect.x = value; } }
        public int y { get { return (int)baseRect.y; } set { baseRect.y = value; } }
        public int width { get { return (int)baseRect.width; } set { baseRect.width = value; } }
        public int height { get { return (int)baseRect.height; } set { baseRect.height = value; } }
        public int xMax { get { return (int)baseRect.xMax; } set { baseRect.xMax = value; } }
        public int xMin { get { return (int)baseRect.xMin; } set { baseRect.xMin = value; } }
        public int yMax { get { return (int)baseRect.yMax; } set { baseRect.yMax = value; } }
        public int yMin { get { return (int)baseRect.yMin; } set { baseRect.yMin = value; } }

        public bool Contains(Vector2 pt) { return baseRect.Contains(pt); }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}