using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    /// <summary>
    /// An item that can be picked up by a Character.
    /// </summary>
    public class LargeItem : Pawn
    {
        /// <summary>
        /// The amount of space between the character and the held LargeItem.
        /// </summary>
        private const float HOLD_OFFSET = 0.5f;

        /// <summary>
        /// The SpriteRenderers inside this LargeItem.
        /// </summary>
        public SpriteRenderer m_sprite;

        [NonSerialized]
        public Character m_beingHeldBy = null;

        public bool IsBeingHeld => m_beingHeldBy != null;

        public void MoveWithCharacter()
        {
            if (m_beingHeldBy != null)
            {
                // The position of the Character holding the LargeItem.
                Vector2 characterPos = m_beingHeldBy.Position;

                // The offset between the Character and the LargeItem.
                Vector2 offset;

                // Figure out the offset using the direction the Character is facing.
                switch (m_beingHeldBy.FacingDirection)
                {
                    case Direction.South:
                        offset = new Vector2(0, -HOLD_OFFSET);
                        break;
                    case Direction.West:
                        offset = new Vector2(-HOLD_OFFSET, 0);
                        break;
                    case Direction.North:
                        offset = new Vector2(0, HOLD_OFFSET);
                        break;
                    case Direction.East:
                        offset = new Vector2(HOLD_OFFSET, 0);
                        break;
                    default:
                        offset = Vector2.zero;
                        break;
                }

                // Set the final position of the LargeItem.
                Position = characterPos + offset;

                // Set the rendering height
                // If going north, render below the Character.
                if (m_sprite != null)
                    m_sprite.sortingOrder = m_beingHeldBy.FacingDirection == Direction.North ? 0 : 20;
            }
        }

        /// <summary>
        /// Called by Character when picked up.
        /// </summary>
        public void PickedUp()
        {
            if (m_sprite != null)
                m_sprite.sortingLayerName = "Player";
        }

        /// <summary>
        /// Called by Character when dropped.
        /// </summary>
        public void Dropped()
        {
            if (m_sprite != null)
                m_sprite.sortingLayerName = "Default";
        }
    }
}
