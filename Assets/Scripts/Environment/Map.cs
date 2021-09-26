using PHC.Pawns;
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

        /// <summary>
        /// The Size of the map.
        /// </summary>
        public Location Size => new Location(m_mapTiles.GetLongLength(0), m_mapTiles.GetLongLength(1));

        private Material m_spriteMaterial;

        /// <summary>
        /// Creates a Map from scratch.
        /// </summary>
        /// <param name="tileSprites">The Sprites for the Tiles.</param>
        /// <param name="spriteMaterial">The Material to use when creating Tiles.</param>
        /// <param name="size">The size of the map to create.</param>
        public Map(Location size, Sprite[] tileSprites, Material spriteMaterial)
        {
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

        /// <summary>
        /// Returns an A* path between two map Locations.
        /// </summary>
        public Location[] GetPath(Location a, Location b)
        {
            // If we're already at point b.
            if(a == b)
            {
                return null;
            }

            // The steps from point A
            ushort[,] distances = new ushort[m_mapTiles.GetLongLength(0), m_mapTiles.GetLongLength(1)];

            // Make all of the distances there max values.
            for (long x = 0; x < distances.GetLongLength(0); x++)
                for (long y = 0; y < distances.GetLongLength(1); y++)
                    distances[x, y] = ushort.MaxValue;

            // Generate all the distance values.
            GenerateAStarDistances(a, 0, distances, b);

            // Return nothing if there's no path back.
            ushort distance = distances[b.X, b.Y];
            if (distance == ushort.MaxValue)
                return null;

            // Find the shortest path back.
            Location[] path = new Location[distance];
            path[path.Length - 1] = b;
            for (int i = path.Length - 2; i >= 0; i--)
            {
                distance--;
                path[i] = path[i + 1];
                if (distances[path[i].X, path[i].Y + 1] == distance)
                    path[i].Y++;
                else if (distances[path[i].X, path[i].Y - 1] == distance)
                    path[i].Y--;
                else if (distances[path[i].X + 1, path[i].Y] == distance)
                    path[i].X++;
                else if (distances[path[i].X - 1, path[i].Y] == distance)
                    path[i].X--;
            }

            return path;
        }

        private void GenerateAStarDistances(Location current, int distance, ushort[,] distances, Location target)
        {
            // The Tile must be valid.
            if (GetTile(current) != Tile.Empty)
                return;

            if (distance < distances[current.X, current.Y])
            {
                distances[current.X, current.Y] = (ushort)distance;
            }
            else
            {
                // This is a longer or equally distant path than another we've found.
                return;
            }

            // If this is the target location, return.
            if (current == target)
                return;

            // Start scanning the next tiles in a clockwise fashion.
            Location n = current;
            n.Y++;
            GenerateAStarDistances(n, distance + 1, distances, target);

            Location e = current;
            e.X++;
            GenerateAStarDistances(e, distance + 1, distances, target);

            Location s = current;
            s.Y--;
            GenerateAStarDistances(s, distance + 1, distances, target);

            Location w = current;
            w.X--;
            GenerateAStarDistances(w, distance + 1, distances, target);
        }


        /// <summary>
        /// Returns the closest Pawn Object from a location.
        /// </summary>
        /// <param name="toLocation">The location to search from.</param>
        /// <param name="path">The path from the toLocation to the closest Object of Type.</param>
        /// <returns>The Object and the path to it.</returns>
        public static T FindClosestComponent<T>(Location toLocation, out Location[] path) where T : Pawn
        {
            Map map = GameManager.Instance?.TheMap;

            T[] allObjectsOfType = GameObject.FindObjectsOfType<T>();

            path = null;

            // If no EggHoles were found, return nothing.
            if (map == null || allObjectsOfType == null || allObjectsOfType.Length == 0)
                return null;

            T outputObject = null;

            foreach (T hole in allObjectsOfType)
            {
                Location[] potentialPath = map.GetPath(toLocation, hole.GetCurrentTile());

                // If not path, skip.
                if (potentialPath == null)
                    continue;

                // If this path is closer than the previous ones.
                if (path == null || potentialPath.Length < path.Length)
                {
                    // Set this as the closest.
                    path = potentialPath;
                    outputObject = hole;
                }
            }

            return outputObject;
        }
    }
}
