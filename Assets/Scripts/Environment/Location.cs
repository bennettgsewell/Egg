using PHC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PHC.Environment
{
    /// <summary>
    /// A tile location on the Map.
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// The X location on the map.
        /// </summary>
        public long X;

        /// <summary>
        /// The Y location on the map.
        /// </summary>
        public long Y;

        public Location(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Location(float x, float y) : this((long)x, (long)y) { }

        public Location(Vector2 position) : this(position.x, position.y) { }

        public Location RoundFrom(Vector2 position) => new Location(position.Round());

        public static bool operator ==(Location a, Location b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Location a, Location b) => a.X != b.X || a.Y != b.Y;
        public override bool Equals(object obj)
        {
            if (obj is Location)
            {
                Location other = (Location)obj;
                return other == this;
            }
            else return false;
        }

        public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();

        public static implicit operator Vector2(Location value) => new Vector2(value.X, value.Y);

        public static long GetDistance(Location a, Location b) => Math.Abs(Math.Abs(a.X) - Math.Abs(b.X)) + Math.Abs(Math.Abs(a.Y) - Math.Abs(b.Y));

        public override string ToString() => $"Location({X}, {Y})";
    }
}
