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
        public long X { set; get; }

        /// <summary>
        /// The Y location on the map.
        /// </summary>
        public long Y { set; get; }

        public Location(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Location(float x, float y) : this((long)x, (long)y) { }

        public Location(Vector2 position) : this(position.x, position.y) { }
    }
}
