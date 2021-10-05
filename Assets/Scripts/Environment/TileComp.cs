using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class TileComp : TileTypeMono
    {
        [SerializeField]
        private TileType m_tile;

        /// <summary>
        /// The Type of Tile this GameObject is.
        /// </summary>
        public override TileType Tile => m_tile;
    }
}
