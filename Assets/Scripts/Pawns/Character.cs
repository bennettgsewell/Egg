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
        public enum Direction
        {
            South = 0,// 0 for default
            North,
            East,
            West
        }

        // The current speed of the character.
        [Range(0f, 10f)]
        public float m_speed;

        [Range(0f, 5f)]
        public float m_pickupDistance;

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
        public void Move(Vector2 direction)
        {
            Vector2 oldPos = Position;

            // Move the character.
            Vector2 newPos = oldPos + direction * Time.deltaTime * m_speed;

            // Determine the direction the Character is moving in.
            bool moving = false;
            if (direction.x > 0)
            {
                moving = true;
                FacingDirection = Direction.East;
            }
            else if (direction.x < 0)
            {
                moving = true;
                FacingDirection = Direction.West;
            }
            else if (direction.y > 0)
            {
                moving = true;
                FacingDirection = Direction.North;
            }
            else if (direction.y < 0)
            {
                moving = true;
                FacingDirection = Direction.South;
            }

            /*
            // If the Character is on a Map make sure it's not moving into a wall.
            Map map = GameManager.Instance?.TheMap;
            if (map != null)
            {
                // Get the current display position of the character.
                Vector2 pixelPosition = PixelPosition;
                pixelPosition.x += 0.5f;
                Location currentTileLocation = new Location(pixelPosition);

                // Get the location of the Tile we're moving towards.
                Location nextTileLocation = currentTileLocation;
                switch (FacingDirection)
                {
                    case Direction.North:
                        nextTileLocation.Y++;
                        break;
                    case Direction.South:
                        nextTileLocation.Y--;
                        break;
                    case Direction.East:
                        nextTileLocation.X++;
                        break;
                    case Direction.West:
                        nextTileLocation.X--;
                        break;
                }

                // The type of Tile we're moving towards.
                Tile nextTile = map.GetTile(nextTileLocation);

                // If the next Tile is not a floor tile, you can't walk on it.
                if (nextTile != Tile.Floor)
                {
                    switch (FacingDirection)
                    {
                        case Direction.North:
                            newPos.y = Mathf.Ceil(oldPos.y);
                            break;
                        case Direction.South:
                            newPos.y = Mathf.Floor(oldPos.y);
                            break;
                        case Direction.East:
                            newPos.y = Mathf.Ceil(oldPos.y);
                            break;
                        case Direction.West:
                            newPos.x = Mathf.Floor(oldPos.x);
                            break;

                    }
                }
            }
            */

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
                m_holding = null;
                m_holding.Dropped();
            }
        }
    }
}
