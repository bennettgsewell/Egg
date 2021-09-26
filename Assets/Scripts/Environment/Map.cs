using PHC.Utility;
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

        /// <summary>
        /// The size of the Map.
        /// </summary>
        public Location Size { private set; get; }

        /// <summary>
        /// Creates a Map from scratch.
        /// </summary>
        /// <param name="tileSprites">The Sprites for the Tiles.</param>
        /// <param name="spriteMaterial">The Material to use when creating Tiles.</param>
        /// <param name="size">The size of the map to create.</param>
        public Map(Location size, Sprite[] tileSprites, Material spriteMaterial)
        {
            Size = size;

            m_spriteMaterial = spriteMaterial;
            m_tileSprites = tileSprites;

            // The Tile types as bytes.
            m_mapTiles = new Tile[size.X, size.Y];

            // The GameObjects to visually represent the Sprites.
            m_mapTileGameObjects = new GameObject[size.X, size.Y];

            // The root GameObject for the Map.
            m_mapRootObject = new GameObject();

            for (Location tilePos = new Location(0, 0); tilePos.X < size.X; tilePos.X++)
                for (tilePos.Y = 0; tilePos.Y < size.Y; tilePos.Y++)
                    m_mapTiles[tilePos.X, tilePos.Y] = Random.Range(0, 2) == 1 ? Tile.Empty : Tile.Blocking;

            m_mapTiles[0, 0] = Tile.Empty;

            for (Location tilePos = new Location(0, 0); tilePos.X < size.X; tilePos.X++)
                for (tilePos.Y = 0; tilePos.Y < size.Y; tilePos.Y++)
                    CreateTileGameObject(tilePos);
        }

        /// <summary>
        /// Creates a Map using Tiles in the Scene
        /// </summary>
        public Map()
        {
            // Get all of the Tile GameObjects in the scene.
            TileComp[] allTileObjects = GameObject.FindObjectsOfType<TileComp>();

            // Try and find the total size of the map so we can create an array.
            Location size = new Location(0, 0);

            // Iterate through all of the Tile objects in the Scene
            foreach (TileComp eachTile in allTileObjects)
            {
                // Figure out the size of the map.
                // Round the position to the nearest grid point, just in case the level designer makes a mistake.
                Vector3 position = eachTile.transform.position.Round();

                eachTile.transform.position = position;

                if (size.X < position.x)
                    size.X = (long)position.x;

                if (size.Y < position.y)
                    size.Y = (long)position.y;
            }

            // Now we know the total size of the map in the positive axis(s)
            Size = size;

            m_mapTiles = new Tile[size.X + 1, size.Y + 1];

            // This will be used if new Tiles are added to the Scene during runtime.
            m_mapRootObject = new GameObject("The Map");

            foreach (TileComp eachTile in allTileObjects)
            {
                Location tileLoc = new Location(eachTile.transform.position);

                if (tileLoc.X < 0 || tileLoc.Y < 0)
                {
                    GameObject.Destroy(eachTile.gameObject);
                }
                else
                {
                    m_mapTiles[tileLoc.X, tileLoc.Y] = eachTile.Tile;
                }
            }
        }

        /// <summary>
        /// Creates a GameObject to display the Tile Sprite in world.
        /// </summary>
        private void CreateTileGameObject(Location pos)
        {
            Tile tileType = m_mapTiles[pos.X, pos.Y];

            // Get the Sprite from its array locaiton.
            Sprite sprite = m_tileSprites[(int)tileType];

            // Create the GameObject
            GameObject tileGO = new GameObject();
            m_mapTileGameObjects[pos.X, pos.Y] = tileGO;

            // Add the Sprite
            SpriteRenderer renderer = tileGO.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = m_spriteMaterial;
            renderer.sprite = sprite;
            renderer.sortingLayerName = "Background";

            // Set the location and parent.
            tileGO.transform.SetParent(m_mapRootObject.transform);
            tileGO.transform.position = new Vector3(pos.X, pos.Y, 0);
        }

        /// <summary>
        /// Gets the Tile in this location.
        /// </summary>
        public Tile GetTile(Location pos)
        {
            // If the position is out of bounds, return blocking.
            if (pos.X < 0 || pos.Y < 0
                || pos.X >= m_mapTiles.GetLongLength(0) || pos.Y >= m_mapTiles.GetLongLength(1))
                return Tile.Blocking;

            return m_mapTiles[pos.X, pos.Y];
        }
    }
}
