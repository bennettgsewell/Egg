using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class Map
    {
        // The Sprite to use for each Tile.
        Sprite[] m_tileSprites;

        // The Tile types.
        private Tile[,] m_mapTiles;

        // The root GameObject for the map.
        GameObject m_mapRootObject;

        // The GameObject for each tile.
        GameObject[,] m_mapTileGameObjects;

        private Material m_spriteMaterial;

        public Map(Location mapSize, Sprite[] tileSprites, Material spriteMaterial)
        {
            m_spriteMaterial = spriteMaterial;
            m_tileSprites = tileSprites;
             
            // The Tile types as bytes.
            m_mapTiles = new Tile[mapSize.X, mapSize.Y];

            // The GameObjects to visually represent the Sprites.
            m_mapTileGameObjects = new GameObject[mapSize.X, mapSize.Y];

            // The root GameObject for the Map.
            m_mapRootObject = new GameObject();

            for (Location tilePos = new Location(0, 0); tilePos.X < mapSize.X; tilePos.X++)
            {
                for (tilePos.Y = 0; tilePos.Y < mapSize.Y; tilePos.Y++)
                {
                    m_mapTiles[tilePos.X, tilePos.Y] = Random.Range(0, 2) == 1 ? Tile.Floor : Tile.Wall;

                    CreateTileGameObject(tilePos);
                }
            }
        }

        /// <summary>
        /// Creates a GameObject to display the Tile Sprite in world.
        /// </summary>
        private void CreateTileGameObject(Location pos)
        {
            Tile tileType = m_mapTiles[pos.X, pos.Y];

            //If this is going to be a wall
            if(tileType == Tile.Wall)
            {
                //And there's a wall just south of this one.
                if(pos.Y > 0 && m_mapTiles[pos.X, pos.Y-1] == Tile.Wall)
                {
                    tileType = Tile.Ceiling;
                }
            }

            // Get the Sprite from its array locaiton.
            Sprite sprite = m_tileSprites[(int)tileType];

            // Create the GameObject
            GameObject tileGO = new GameObject();
            m_mapTileGameObjects[pos.X, pos.Y] = tileGO;

            // Add the Sprite
            SpriteRenderer renderer = tileGO.AddComponent<SpriteRenderer>();
            //renderer.sharedMaterial = m_spriteMaterial;
            renderer.sprite = sprite;
            renderer.sortingLayerName = "Background";

            // Set the location and parent.
            tileGO.transform.SetParent(m_mapRootObject.transform);
            tileGO.transform.position = new Vector3(pos.X, pos.Y, 0);
        }
    }
}
