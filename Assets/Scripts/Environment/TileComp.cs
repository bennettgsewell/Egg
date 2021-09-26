using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class TileComp : MonoBehaviour
    {
        [SerializeField]
        private Tile m_tile;

        /// <summary>
        /// The Type of Tile this GameObject is.
        /// </summary>
        public Tile Tile { get => m_tile; }
    }
}
