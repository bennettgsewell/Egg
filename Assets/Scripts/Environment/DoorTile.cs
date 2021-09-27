using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class DoorTile : TileTypeMono
    {
        public override Tile Tile => Tile.Door;

        public Door m_door;
    }
}
