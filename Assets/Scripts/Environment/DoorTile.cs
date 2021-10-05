using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class DoorTile : TileTypeMono
    {
        public override TileType Tile => TileType.Door;

        public Door m_door;
    }
}
