using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jintori.Game
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Builds a vector path using only one stroke, reading pixel data
    /// </summary>
    public class PathBuilder
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        /// <summary> 4 possible directions to search the path in</summary>
        enum Direction { Up, Rt, Dw, Lt, End }

        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        /// <summary> Mask we're analizing </summary>
        PlayArea.Mask mask;

        /// <summary>Type of path value to process</summary> 
        byte pathType;

        /// <summary> Helper array to know which points in the mask we have already visited </summary>
        bool [,] visited;

        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
        /// <summary>
        /// Gets a list of continuous segments that can form the path
        /// </summary>
        public List<List<Point>> GetSegments(PlayArea.Mask mask, int x, int y, byte pathType)
        {
            // initialization
            this.mask = mask;
            this.pathType = pathType;
            List<List<Point>> segments = new List<List<Point>>();
            visited = new bool[PlayArea.imageWidth, PlayArea.imageHeight];
            Stack<Point> splits = new Stack<Point>();
            splits.Push(new Point(x, y));

            // do one segment at the time
            while (splits.Count > 0)
            {
                // get the start of the next segment
                Point pt = splits.Pop();
                visited[pt.x, pt.y] = false; // a split point was visited before, so we have to reset it

                // new segment
                List<Point> segment = new List<Point>();
                Direction dir = GetNextPointDirection(pt);
                Point next = pt;
                segment.Add(pt);

                // continue until we found no more valid pathType pixels
                while(dir != Direction.End)
                {
                    // hold onto the same direction
                    Direction segmentDir = dir;
                    while(segmentDir == dir)
                    {
                        bool isSplit = false;
                        pt = next;
                        segmentDir = GetNextPointDirection(dir, pt, out next, out isSplit);
                        visited[pt.x, pt.y] = true;

                        // if it's a split, push it to the stack so we can make another 
                        // segment later
                        if (isSplit)
                            splits.Push(pt);
                    }
                    // set direction of next segment
                    dir = segmentDir;
                    segment.Add(pt);
                }
                segments.Add(segment);
            } 
            
            return segments;
        }
        

        // -----------------------------------------------------------------------------------	
        Direction GetNextPointDirection(Point pt)
        {
            bool isSplit;
            Point next;
            return GetNextPointDirection(Direction.Up, pt, out next, out isSplit);
        }

        // -----------------------------------------------------------------------------------	
        Direction GetNextPointDirection(Direction dir, Point pt, out Point next, out bool isSplit)
        {
            next = pt;
            int x = pt.x;
            int y = pt.y;
            isSplit = false;

            // look around in 4 directions to possible places we can search from here
            bool[] neighbours = new bool[]
            {
                y + 1 < PlayArea.imageHeight && mask[x, y + 1] == pathType && !visited[x, y + 1],   // Up
                x + 1 < PlayArea.imageWidth && mask[x + 1, y] == pathType && !visited[x + 1, y],    // Rt
                y - 1 >= 0 && mask[x, y - 1] == pathType && !visited[x, y - 1],                     // Dw
                x - 1 >= 0 && mask[x - 1, y] == pathType && !visited[x - 1, y],                     // Lt
            };

            // count those places. If 2 or more, it's a split point (1 or less is a path)
            // NOTE: should be three, but one gets reduced since it was visited in the previous step
            int count = 0;
            foreach (bool b in neighbours)
                if (b) count++;
            isSplit = count >= 2;

            // search in clockwise direction, preferring to keep the same direction when possible
            int start = (int)dir;
            for (int i = start; i < start + 4; i++)
            {
                int j = i % 4;
                if (!neighbours[j])
                    continue;
                dir = (Direction)j;
                switch (dir)
                {
                    case Direction.Up: next.y = y + 1; break;
                    case Direction.Rt: next.x = x + 1; break;
                    case Direction.Dw: next.y = y - 1; break;
                    case Direction.Lt: next.x = x - 1; break;
                }
                return dir;
            }

            return Direction.End; // path ended!
        }
    }
}