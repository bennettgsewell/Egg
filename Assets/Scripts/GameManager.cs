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

        private void Start()
        {
            m_map = new Map(new Location(32, 32), m_tiles);
        }

    }
}
