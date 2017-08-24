using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    public struct Point
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        public static implicit operator Vector2(Point pt) { return new Vector2(pt.x, pt.y); }

        // -----------------------------------------------------------------------------------
        public static bool operator ==(Point a, Point b) { return a.x == b.x && a.y == b.y; }

        // -----------------------------------------------------------------------------------
        public static bool operator !=(Point a, Point b) { return a.x != b.x || a.y != b.y; }

        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        public int x;
        public int y;

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        public Point(Vector2 intVector)
        {
            x = Mathf.FloorToInt(intVector.x);
            y = Mathf.FloorToInt(intVector.y);
        }

        // -----------------------------------------------------------------------------------	
        public Point(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        // -----------------------------------------------------------------------------------	
        public override bool Equals(object obj)
        {
            if (obj is Point)
                return this == (Point)obj;
            return false;
        }

        // -----------------------------------------------------------------------------------	
        public override int GetHashCode()
        {
            const int LargePrime1 = 126781;
            const int LargePrime2 = 578603;
            unchecked // Overflow is fine, just wrap
            {
                int hash = LargePrime1;
                // Suitable nullity checks etc, of course :)
                hash = hash * LargePrime2 + x.GetHashCode();
                hash = hash * LargePrime2 + y.GetHashCode();
                return hash;
            }
        }
    }
}