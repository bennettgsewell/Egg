
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
            base.Start();
            SetDestination(new Location(7, 1));
        }
    }
}
