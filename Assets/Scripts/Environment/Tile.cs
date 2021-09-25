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
    public enum Tile : byte
    {
        //The values of these are also the locaions in the Sprite[] array.
        Floor = 0,
        Wall = 1,
        Ceiling = 2
    }
}
