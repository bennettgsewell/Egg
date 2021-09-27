using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class Monster : Character
    {
        /// <summary>
        /// The distance that the mob will see the player and start attacking.
        /// </summary>
        private const long ENEMY_SEE_DISTANCE = 5;

        /// <summary>
        /// The distance that the mob will see the player and start attacking.
        /// </summary>
        private const long EGG_SEE_DISTANCE = 10;

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
            /// Chasing the player.
            /// </summary>
            Attack,

            /// <summary>
            /// After a succesful attack, the monster will move to pickup the egg, if not found they will pause.
            /// </summary>
            SuccessOnAttackSearchForEgg,

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

        /// <summary>
        /// This is used to keep track of when the Kobold moves.
        /// </summary>
        private Location m_lastKoboldLocation;

        /// <summary>
        /// The last Kobold we were attacking.
        /// </summary>
        private Kobold m_lastKobold;

        [SerializeField]
        private AudioClip m_deathSound;

        /// <summary>
        /// The egg we're currently chasing.
        /// </summary>
        private Egg m_targetEgg;

        /// <summary>
        /// The EggHole we're moving towards.
        /// </summary>
        private EggHole m_targetEggHole;

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
        /// Tells the Monster to move to a location if possible.
        /// </summary>
        public void SetDestination(Pawn pawn) => SetDestination(pawn.GetCurrentTile());

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

        /// <summary>
        /// Start attack the closest player.
        /// </summary>
        private void StartAttacking()
        {
            CurrentState = MonsterState.Attack;
        }

        private void StartSuccessfullyAttackedFindEgg()
        {
            CurrentState = MonsterState.SuccessOnAttackSearchForEgg;

            // Start chasing the closest egg if there is one.
            SetDestinationToClosestPawnOfType(out m_targetEgg);

            // If the pathing failed, it could be because the Egg is in the same Tile.
            if (m_currentPath == null)
            {
                // Try and pick up EVERY EGG
                Egg[] eggs = FindObjectsOfType<Egg>();
                foreach (Egg egg in eggs)
                    if (PickupLargeItem(egg, false))
                        return;
            }

            // If pathing failed or egg is out of range.
            if (m_currentPath == null || m_currentPath.Length > EGG_SEE_DISTANCE || m_targetEgg.IsBeingHeld)
            {
                // Cancel chasing egg.
                SetPath(null);
                m_targetEgg = null;
            }

            m_idlingUntil = Time.time + 3f;
        }

        protected void Update()
        {
            Location current = GetCurrentTile();

            // If we're holding an item and it's an Egg, navigate to EggHole, otherwise drop.
            if (IsHoldingItem)
            {
                if (HeldItem is Egg)
                {
                    // If not pathing, move towards nearest EggHole.
                    if (!IsPathing || m_targetEggHole == null)
                    {
                        SetDestinationToClosestPawnOfType<EggHole>(out m_targetEggHole);
                        if (!IsPathing)
                        {
                            // Couldn't find an EggHole.
                            // This should never happen but in case it does.
                            Kill();
                        }
                    }

                    // If we have reached the EggHole.
                    if (Vector2.Distance(Position, m_targetEggHole.Position) < m_pickupDistance)
                    {
                        LargeItem egg = HeldItem;
                        DropLargeItem();
                        Destroy(egg.gameObject);

                        m_targetEgg = null;
                        m_targetEggHole = null;
                    }
                }
                else
                {
                    DropLargeItem();
                }
            }
            else
            {
                // Check to see if an egg is nearby.
                if (CurrentState != MonsterState.Attack && CurrentState != MonsterState.SuccessOnAttackSearchForEgg)
                {
                    Egg egg = Map.FindPathToClosestComponent<Egg>(current, out Location[] eggPath);
                    if (egg != null && !egg.IsBeingHeld && Vector2.Distance(Position, egg.Position) < EGG_SEE_DISTANCE)
                        StartSuccessfullyAttackedFindEgg();
                }

                // Check to see if a player is nearby.
                if (CurrentState != MonsterState.Attack && CurrentState != MonsterState.SuccessOnAttackSearchForEgg)
                {

                    Kobold[] kobolds = FindObjectsOfType<Kobold>();
                    foreach (Kobold kobold in kobolds)
                    {
                        Location kLoc = kobold.GetCurrentTile();
                        long distanceFromKobold = Location.GetDistance(current, kLoc);

                        if (distanceFromKobold < ENEMY_SEE_DISTANCE)
                        {
                            // Make sure there's a path between the Kobold and the mob.
                            Location[] path = GameManager.Instance?.TheMap?.GetPath(current, kLoc);

                            if (path != null)
                            {
                                StartAttacking();
                                SetPath(path);
                                m_lastKoboldLocation = kLoc;
                                m_lastKobold = kobold;
                                break;
                            }
                        }
                    }
                }

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

                    case MonsterState.Attack:

                        // If the Kobold Object was destoryed or is missing.
                        if (m_lastKobold == null)
                        {
                            StartIdling();
                            break;
                        }

                        // The location of the Kobold.
                        Location kLoc = m_lastKobold.GetCurrentTile();

                        // If the Kobold has moved.
                        if (kLoc != m_lastKoboldLocation)
                        {
                            // Create a new path to them.
                            m_lastKoboldLocation = kLoc;
                            SetDestination(m_lastKobold);
                        }

                        break;

                    case MonsterState.SuccessOnAttackSearchForEgg:

                        // We have begun moving towards the egg.
                        if (IsPathing && m_targetEgg != null)
                        {
                            // If the player or another monster picked up the egg.
                            if (m_targetEgg.IsBeingHeld)
                            {
                                // Start idling this will initiate attack if the player is close enough.
                                SetPath(null);
                                m_targetEgg = null;
                                StartTinyExplore();
                                return;
                            }

                            // The egg is still up for grabs.
                            // Try and pick it up.
                            if (PickupLargeItem(m_targetEgg, false))
                                SetPath(null);
                        }
                        else
                        {
                            //Idle for a little bit.
                            if (Time.time > m_idlingUntil)
                            {
                                StartTinyExplore();
                            }
                            return;
                        }
                        break;

                    case MonsterState.STOP:
                        return;
                }
            }

            if (IsPathing)
                KeepPathing();

            // If we're currently chasing a Kobold.
            if (CurrentState == MonsterState.Attack && m_lastKobold != null)
            {
                // Get the distance between us
                Vector2 absoluteKoboldPos = m_lastKobold.Position;
                Vector2 absoluteMonsterPos = Position;

                // If we're within range damage them.
                float distance = Vector2.Distance(absoluteKoboldPos, absoluteMonsterPos);
                if (distance < m_pawnSizeDiameter / 2f + m_lastKobold.m_pawnSizeDiameter / 2f)
                {
                    // Make them drop whatever they're holding.
                    // Before we deal damage in case they die.
                    m_lastKobold?.DropLargeItem();

                    // Deal damage to Kobold
                    m_lastKobold.DealDamage(1);

                    // Change the state to pause/search for egg.
                    StartSuccessfullyAttackedFindEgg();
                }
            }
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
            DropLargeItem();
            FlickerAnimation.StartFlickerOn(gameObject, true, 0.5f);
            TookDamage();
            Destroy(this);
        }

        public override void TookDamage()
        {
            PlaySound(m_deathSound);
        }
    }
}
