using PHC.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class WalkPoint : Pawn
    {
        private List<Tuple<WalkPoint, Location[]>> otherWalkPoints;

        private void Awake()
        {
            // Use Floor to find the closest Tile.
            // This fixes a bug at the start.
            Position = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
        }

        private void Start()
        {
            base.Start();

            otherWalkPoints = Map.FindAllPathsToComponents<WalkPoint>(this, GetCurrentTile(), 10);
        }

        public Tuple<WalkPoint, Location[]> GetPathToAnotherWalkPoint()
        {
            if (otherWalkPoints == null || otherWalkPoints.Count == 0)
                return null;

            return otherWalkPoints[UnityEngine.Random.Range(0, otherWalkPoints.Count)];
        }
    }
}
