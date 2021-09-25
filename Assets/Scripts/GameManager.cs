using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    public class GameManager : MonoBehaviour
    {
        private Map m_map;

        /// <summary>
        /// The Tile sprites.
        /// </summary>
        public Sprite[] m_tiles;

        /// <summary>
        /// The Material to use on the Sprites.
        /// </summary>
        public Material m_spriteMaterial;

        private void Start()
        {
            m_map = new Map(new Location(32, 32), m_tiles, m_spriteMaterial);
        }

    }
}
