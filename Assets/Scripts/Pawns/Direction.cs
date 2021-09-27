using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    /// <summary>
    /// Cardinal directions.
    /// </summary>
    public enum Direction
    {
        South = 0,// 0 for default
        North = 1,
        East = 2,
        West = 3
    }

    public static class DirectionExt
    {
        /// <summary>
        /// Shifts a Location by 1 in a direction.
        /// </summary>
        public static Location Shift(this Direction dir, Location location)
        {
            switch (dir)
            {
                case Direction.East: location.X++; break;
                case Direction.North: location.Y++; break;
                case Direction.South: location.Y--; break;
                case Direction.West: location.X--; break;
            }
            return location;
        }

        /// <summary>
        /// Shifts a Vector2 by 1 in a direction.
        /// </summary>
        public static Vector2 Shift(this Direction dir, Vector2 location)
        {
            switch (dir)
            {
                case Direction.East: location.x++; break;
                case Direction.North: location.y++; break;
                case Direction.South: location.y--; break;
                case Direction.West: location.x--; break;
            }
            return location;
        }
    }

}
