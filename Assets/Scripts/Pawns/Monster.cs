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
            /// <summary>
            /// Standing still for a random amount of time.
            /// </summary>
            Idling,

            /// <summary>
            /// Wandering to another WalkPoint.
            /// </summary>
            Wander,

            /// <summary>
            /// Simmilar to idling, just moving in random directions now and then.
            /// </summary>
            TinyExplore,

            /// <summary>
            /// Stops everything forever.
            /// </summary>
            STOP,
        }

        /// <summary>
        /// The current state of the Monster's AI.
        /// </summary>
        public MonsterState CurrentState { private set; get; }

        /// <summary>
        /// If set the monster is moving along a path.
        /// </summary>
        private Location[] m_currentPath = null;

        public bool IsPathing => m_currentPath != null;

        /// <summary>
        /// The current step along the path.
        /// </summary>
        private int m_currentPathStep = 0;

        /// <summary>
        /// When the idling will end.
        /// </summary>
        private float m_idlingUntil = 0;

        /// <summary>
        /// Will do a tiny explore until this time has passed.
        /// </summary>
        private float m_tinyExploreUntil = 0;

        [SerializeField]
        private AudioClip m_deathSound;

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
        }

        /// <summary>
        /// Tells the AI to stop doing everything.
        /// </summary>
        public void StopEverything()
        {
            CurrentState = MonsterState.STOP;
        }

        protected void Start()
        {
            base.Start();
            StartIdling();
        }

        /// <summary>
        /// Tells the AI to start idling for a random amount of time.
        /// </summary>
        private void StartIdling()
        {
            m_idlingUntil = Time.time + Random.Range(0f, 3f);
            CurrentState = MonsterState.Idling;
        }

        /// <summary>
        /// Starts wandering to the next WalkPoint.
        /// </summary>
        private void StartWandering()
        {
            CurrentState = MonsterState.Wander;

            // Find the closest WalkPoint.
            // This will probably be in the room with us.
            WalkPoint closestWalkPoint = Map.FindPathToClosestComponent<WalkPoint>(GetCurrentTile(), out Location[] _);

            // Get a different walk point that's connected to this one.
            WalkPoint nextWalkPoint = closestWalkPoint.GetPathToAnotherWalkPoint()?.Item1;

            // Attempt to path there.
            if (nextWalkPoint != null)
                SetDestination(nextWalkPoint.GetCurrentTile());

            // If this fails, idle instead.
            if (!IsPathing)
                StartIdling();
        }

        /// <summary>
        /// Starts moving a little bit around the same room.
        /// </summary>
        private void StartTinyExplore()
        {
            m_tinyExploreUntil = Time.time + Random.Range(4f, 15f);
            CurrentState = MonsterState.TinyExplore;
        }

        protected void Update()
        {
            switch (CurrentState)
            {
                case MonsterState.Idling:
                    if (Time.time > m_idlingUntil)
                    {
                        if (Random.Range(0, 2) == 1)
                            StartWandering();
                        else
                            StartTinyExplore();
                    }
                    break;

                case MonsterState.Wander:
                    if (!IsPathing)
                        StartIdling();
                    break;

                case MonsterState.TinyExplore:

                    if (Time.time > m_tinyExploreUntil)
                    {
                        SetPath(null);
                        StartIdling();
                    }

                    if (Time.time > m_idlingUntil)
                    {
                        // If already walking skip.
                        if (IsPathing)
                            break;

                        // Find the next tile to walk to.
                        Location nextTile = GetCurrentTile();
                        switch (GetRandomDirection())
                        {
                            case Direction.East:
                                nextTile.X++;
                                break;
                            case Direction.North:
                                nextTile.Y++;
                                break;
                            case Direction.South:
                                nextTile.Y--;
                                break;
                            case Direction.West:
                                nextTile.X--;
                                break;
                        }
                        Tile nextTileType = GameManager.Instance?.TheMap?.GetTile(nextTile) ?? Tile.Blocking;

                        // If the randomly selected Tile is empty, path there.
                        if (nextTileType == Tile.Empty)
                            SetDestination(nextTile);

                        // The amount of time to wait before choosing the next Tile to go to. This includes travel time.
                        m_idlingUntil = Time.time + Random.Range(0, 2f);
                    }
                    break;

                case MonsterState.STOP:
                    return;
            }

            if (IsPathing)
                KeepPathing();
        }

        /// <summary>
        /// Called every frame while a path is set.
        /// </summary>
        private void KeepPathing()
        {
            // Get the current position of the Monster.
            Vector2 currentPos = Position;

            // Get the target destination of the next Tile in the path.
            Vector2 targetPos = m_currentPath[m_currentPathStep];

            // Find the difference between here and there.
            Vector2 delta = targetPos - currentPos;

            // If there's no distance, move the target to the next Tile in the path.
            if (delta.magnitude == 0f)
            {
                m_currentPathStep++;

                // If we've reached the end of the path, end
                if (m_currentPath.Length == m_currentPathStep)
                {
                    SetPath(null);
                    return;
                }

                // Calculate the new delta, since the tile target changed.
                targetPos = m_currentPath[m_currentPathStep];
                delta = targetPos - currentPos;
            }

            // Tell the Pawn to move in the direction.
            Move(delta);
        }

        /// <summary>
        /// Sets the destination of this Monster to the closest Pawn of Type T.
        /// </summary>
        /// <typeparam name="T">The Type of Pawn to look for.</typeparam>
        /// <param name="targetObj">Outputs the object itself if one was found.</param>
        public void SetDestinationToClosestPawnOfType<T>(out T targetObj) where T : Pawn
        {
            targetObj = Map.FindPathToClosestComponent<T>(GetCurrentTile(), out Location[] path);

            if (targetObj != null && path != null)
                SetPath(path);
            else
                SetPath(null);
        }

        public override void Kill()
        {
            FlickerAnimation.StartFlickerOn(gameObject, true, 0.5f);
            PlaySound(m_deathSound);
            enabled = false;
        }
    }
}
