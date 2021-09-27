using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public abstract class TileTypeMono : MonoBehaviour
    {
        /// <summary>
        /// The Type of Tile this GameObject is.
        /// </summary>
        public abstract Tile Tile { get; }

        /// <summary>
        /// Returns the current Tile Location that this Pawn is on.
        /// </summary>
        public Location GetCurrentTile()
        {
            // Get the current position of the Pawn.
            Vector2 pos = transform.position;

            // Get the middle of the Tile we're on.
            pos.x += 0.5f;
            pos.y += 0.5f;

            return new Location(pos);
        }
    }
}
