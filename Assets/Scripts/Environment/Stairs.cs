using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class Stairs : TileTypeMono
    {
        public enum StairDirection
        {
            Ascending,
            Descending,
        }

        public override TileType Tile => TileType.Empty;

        public StairDirection m_direction;
    }
}
