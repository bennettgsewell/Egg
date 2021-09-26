using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class Monster : Character
    {
        public enum MonsterState
        {
            Idling,
            Moving,
        }

        /// <summary>
        /// The current state of the Monster's AI.
        /// </summary>
        public MonsterState CurrentState { private set; get; }

        /// <summary>
        /// If set the monster is moving along a path.
        /// </summary>
        private Location[] m_currentPath = null;

        /// <summary>
        /// The current step along the path.
        /// </summary>
        private int m_currentPathStep = 0;

        /// <summary>
        /// Tells the Monster to move to a location if possible.
        /// </summary>
        public void SetDestination(Location target)
        {
            // Make sure there is a Map object.
            Map map = GameManager.Instance?.TheMap;
            if (map == null)
            {
                SetPath(null);
                return;
            }

            // Compare the current location to the target to make sure it is different.
            Location current = GetCurrentTile();

            // Get the path from A to B
            Location[] path = map.GetPath(current, target);

            SetPath(path);
        }

        /// <summary>
        /// Sets the current walk path.
        /// </summary>
        private void SetPath(Location[] path)
        {
            // Set the part and set the step to start from the beginning.
            m_currentPath = path;
            m_currentPathStep = 0;

            // If no path was set, don't start moving.
            CurrentState = m_currentPath == null ? MonsterState.Idling : MonsterState.Moving;
        }

        protected void Update()
        {
            switch (CurrentState)
            {
                case MonsterState.Moving:
                    Vector2 currentPos = Position;
                    Vector2 targetPos = m_currentPath[m_currentPathStep];
                    Vector2 delta = targetPos - currentPos;
                    if (delta.magnitude == 0f)
                    {
                        m_currentPathStep++;
                        if (m_currentPath.Length == m_currentPathStep)
                        {
                            SetPath(null);
                        }
                        else
                        {
                            currentPos = Position;
                            targetPos = m_currentPath[m_currentPathStep];
                            delta = targetPos - currentPos;
                            Move(delta);
                        }
                    }
                    else
                        Move(delta);
                    break;
            }
        }

        /// <summary>
        /// Sets the destination of this Monster to the closest Pawn of Type T.
        /// </summary>
        /// <typeparam name="T">The Type of Pawn to look for.</typeparam>
        /// <param name="targetObj">Outputs the object itself if one was found.</param>
        public void SetDestinationToClosestPawnOfType<T>(out T targetObj) where T : Pawn
        {
            targetObj = Map.FindClosestComponent<T>(GetCurrentTile(), out Location[] path);

            if (targetObj != null && path != null)
                SetPath(path);
            else
                SetPath(null);
        }
    }
}
