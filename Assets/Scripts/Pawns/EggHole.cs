using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class EggHole : Pawn
    {
        /// <summary>
        /// Returns the closest EggHole to a location.
        /// </summary>
        /// <param name="toLocation">The location to search from.</param>
        /// <param name="path">The path from the toLocation to the EggHole.</param>
        /// <returns>The EggHole and the path to it.</returns>
        public static EggHole FindClosestHole(Location toLocation, out Location[] path)
        {
            Map map = GameManager.Instance?.TheMap;

            EggHole[] allHoles = FindObjectsOfType<EggHole>();

            path = null;

            // If no EggHoles were found, return nothing.
            if (map == null || allHoles == null || allHoles.Length == 0)
                return null;

            EggHole outputHole = null;

            foreach (EggHole hole in allHoles)
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
                    outputHole = hole;
                }
            }

            return outputHole;
        }
    }
}