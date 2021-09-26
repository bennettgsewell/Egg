using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    /// <summary>
    /// A character that can walk around.
    /// </summary>
    public class Character : Pawn
    {
        /// <summary>
        /// Cardinal directions.
        /// </summary>
        public enum Direction
        {
            South = 0,// 0 for default
            North = 1,
            East = 2,
            West = 3
        }

        /// <summary>
        /// Returns a random Direction enum.
        /// </summary>
        public Direction GetRandomDirection() => (Direction)Random.Range(0, 4);

        // The current speed of the character.
        [Range(0f, 10f)]
        public float m_speed;

        [Range(0f, 5f)]
        public float m_pickupDistance;

        /// <summary>
        /// The Pawn size in diameter.
        /// </summary>
        public float m_pawnSizeDiameter = (ForceGameboyAspectRatio.PPU - 4) / (float)ForceGameboyAspectRatio.PPU;

        // If set the chracter is holding this large item.
        private LargeItem m_holding = null;

        // The animator component.
        public Animator m_animator;

        /// <summary>
        /// The direction this Character is facing.
        /// </summary>
        public Direction FacingDirection { private set; get; }

        /// <summary>
        /// Returns true if this Character is holding a LargeItem.
        /// </summary>
        public bool IsHoldingItem => m_holding != null;

        /// <summary>
        /// Moves the character in a direction.
        /// </summary>
        public void Move(Vector2 destinationDelta)
        {
            float willMove = Time.deltaTime * m_speed;

            if (destinationDelta.magnitude > willMove)
                destinationDelta = destinationDelta.normalized * willMove;

            Vector2 oldPos = Position;

            // Move the character.
            Vector2 newPos = oldPos + destinationDelta;

            // Determine the direction the Character is moving in.
            bool moving = false;
            if (destinationDelta.x > 0)
            {
                moving = true;
                FacingDirection = Direction.East;
            }
            else if (destinationDelta.x < 0)
            {
                moving = true;
                FacingDirection = Direction.West;
            }
            else if (destinationDelta.y > 0)
            {
                moving = true;
                FacingDirection = Direction.North;
            }
            else if (destinationDelta.y < 0)
            {
                moving = true;
                FacingDirection = Direction.South;
            }

            // If the Character is on a Map make sure it's not moving into a wall.
            Map map = GameManager.Instance?.TheMap;
            if (map != null)
            {
                // Get the current Tile Location that the player is on.
                Location currentTileLocation = GetCurrentTile();

                float pawnSizeRadius = m_pawnSizeDiameter / 2f;

                Vector2 newPosCenterOfTile = newPos + new Vector2(0.5f, 0.5f);

                switch (FacingDirection)
                {
                    case Direction.North:
                        // If the top left or top right of Pawn is inside a blocking tile.
                        if(map.GetTile(new Location(newPosCenterOfTile + new Vector2(-pawnSizeRadius, pawnSizeRadius))) == Tile.Blocking
                         || map.GetTile(new Location(newPosCenterOfTile + new Vector2(pawnSizeRadius, pawnSizeRadius))) == Tile.Blocking)
                        {
                            // If the Tile directly above is open
                            Location tileDirectlyAbove = new Location(newPosCenterOfTile + new Vector2(0, pawnSizeRadius));
                            if (map.GetTile(tileDirectlyAbove) == Tile.Empty)
                            {
                                // Get the point inside the current tile.
                                float insideTile = newPosCenterOfTile.x % 1;
                                insideTile -= 0.5f;

                                // If it's close enough to be inside the tile above
                                if(Mathf.Abs(insideTile) <= pawnSizeRadius)
                                {
                                    break;
                                }
                            }

                            newPos.y = Mathf.Floor(newPos.y);
                            newPos.y += 1f - m_pawnSizeDiameter - (1 / (float)ForceGameboyAspectRatio.PPU);
                        }
                        break;
                    case Direction.South:
                        // If the bottom left or bottom right of Pawn is inside a blocking tile.
                        if (map.GetTile(new Location(newPosCenterOfTile + new Vector2(-pawnSizeRadius, -pawnSizeRadius))) == Tile.Blocking
                         || map.GetTile(new Location(newPosCenterOfTile + new Vector2(pawnSizeRadius, -pawnSizeRadius))) == Tile.Blocking)
                        {
                            // If the Tile directly below is open
                            Location tileDirectlyBelow = new Location(newPosCenterOfTile + new Vector2(0, -pawnSizeRadius));
                            if (map.GetTile(tileDirectlyBelow) == Tile.Empty)
                            {
                                // Get the point inside the current tile.
                                float insideTile = newPosCenterOfTile.x % 1;
                                insideTile -= 0.5f;

                                // If it's close enough to be inside the tile above
                                if (Mathf.Abs(insideTile) <= pawnSizeRadius)
                                {
                                    break;
                                }
                            }

                            newPos.y = Mathf.Ceil(newPos.y);
                            newPos.y -= 1f - m_pawnSizeDiameter - (1 / (float)ForceGameboyAspectRatio.PPU);
                        }
                        break;
                    case Direction.East:
                        // If the top right or bottom right of Pawn is inside a blocking tile.
                        if (map.GetTile(new Location(newPosCenterOfTile + new Vector2(pawnSizeRadius, pawnSizeRadius))) == Tile.Blocking
                         || map.GetTile(new Location(newPosCenterOfTile + new Vector2(pawnSizeRadius, -pawnSizeRadius))) == Tile.Blocking)
                        {
                            // If the Tile directly east is open
                            Location tileDirectlyEast = new Location(newPosCenterOfTile + new Vector2(pawnSizeRadius, 0));
                            if (map.GetTile(tileDirectlyEast) == Tile.Empty)
                            {
                                // Get the point inside the current tile.
                                float insideTile = newPosCenterOfTile.y % 1;
                                insideTile -= 0.5f;

                                // If it's close enough to be inside the tile above
                                if (Mathf.Abs(insideTile) <= pawnSizeRadius)
                                {
                                    break;
                                }
                            }

                            newPos.x = Mathf.Floor(newPos.x);
                            newPos.x += 1f - m_pawnSizeDiameter - (1 / (float)ForceGameboyAspectRatio.PPU);
                        }
                        break;
                    case Direction.West:
                        // If the top left or bottom left of Pawn is inside a blocking tile.
                        if (map.GetTile(new Location(newPosCenterOfTile + new Vector2(-pawnSizeRadius, pawnSizeRadius))) == Tile.Blocking
                         || map.GetTile(new Location(newPosCenterOfTile + new Vector2(-pawnSizeRadius, -pawnSizeRadius))) == Tile.Blocking)
                        {
                            // If the Tile directly west is open
                            Location tileDirectlyEast = new Location(newPosCenterOfTile + new Vector2(-pawnSizeRadius, 0));
                            if (map.GetTile(tileDirectlyEast) == Tile.Empty)
                            {
                                // Get the point inside the current tile.
                                float insideTile = newPosCenterOfTile.y % 1;
                                insideTile -= 0.5f;

                                // If it's close enough to be inside the tile above
                                if (Mathf.Abs(insideTile) <= pawnSizeRadius)
                                {
                                    break;
                                }
                            }

                            newPos.x = Mathf.Ceil(newPos.x);
                            newPos.x -= 1f - m_pawnSizeDiameter - (1 / (float)ForceGameboyAspectRatio.PPU);
                        }
                        break;
                }
            }

            // Move the character to its new position.
            Position = newPos;

            // Determine the direction of the movement for the Animator.
            if (m_animator != null)
            {
                m_animator.SetBool("walk_east", moving && FacingDirection == Direction.East);
                m_animator.SetBool("walk_west", moving && FacingDirection == Direction.West);
                m_animator.SetBool("walk_north", moving && FacingDirection == Direction.North);
                m_animator.SetBool("walk_south", moving && FacingDirection == Direction.South);
            }

            // If holding a LargeItem, move it with us.
            m_holding?.MoveWithCharacter();
        }

        /// <summary>
        /// Attemp to pickup a LargeItem.
        /// </summary>
        /// <param name="item">The item to be picked up by the Character.</param>
        /// <param name="ignorePickupDistance">If set the m_pickupDistance value won't be taken into account.</param>
        public void PickupLargeItem(LargeItem item, bool ignorePickupDistance)
        {
            if (item == null)
                return;

            if (m_holding == null && item.m_beingHeldBy == null && (ignorePickupDistance || Vector2.Distance(item.Position, Position) < m_pickupDistance))
            {
                m_holding = item;
                m_holding.m_beingHeldBy = this;
                m_holding.MoveWithCharacter();
                m_holding.PickedUp();
            }
        }

        /// <summary>
        /// Drops a LargeItem if one is currently being held.
        /// </summary>
        public void DropLargeItem()
        {
            if (m_holding != null)
            {
                m_holding.m_beingHeldBy = null;
                m_holding.Dropped();
                m_holding = null;
            }
        }
    }
}
