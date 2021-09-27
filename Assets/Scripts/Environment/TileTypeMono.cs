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
    }
}
