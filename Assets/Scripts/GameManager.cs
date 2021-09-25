using PHC.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        /// <summary>
        /// The current Map.
        /// </summary>
        public Map TheMap { private set; get; }

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
            if (Instance != null)
                throw new Exception("More than one GameManager!");

            Instance = this;
            TheMap = new Map(new Location(32, 32), m_tiles, m_spriteMaterial);
        }

    }
}
