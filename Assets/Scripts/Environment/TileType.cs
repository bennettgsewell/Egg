using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHC.Environment
{
    /// <summary>
    /// A tile on the map.
    /// </summary>
    public enum TileType : byte
    {
        Empty = 0,
        Door = 1,
        Blocking = 2,
    }
}
