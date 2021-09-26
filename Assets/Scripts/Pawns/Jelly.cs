
using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class Jelly : Monster
    {
        Location[] path;

        private void Start()
        {
            //Try and path to player.
            path = GameManager.Instance.TheMap.GetPath(new Location(4, 1), new Location(7, 1));
        }

        private void Update()
        {
            Location newPos = path[(int)((Time.time / 2f) % path.Length)];
            Position = new Vector2(newPos.X, newPos.Y);
        }
    }
}
