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

        [NonSerialized]
        public Character m_beingHeldBy = null;

        public void MoveWithCharacter()
        {
            if(m_beingHeldBy != null)
            {
                // The position of the Character holding the LargeItem.
                Vector2 characterPos = m_beingHeldBy.Position;

                // The offset between the Character and the LargeItem.
                Vector2 offset;

                // Figure out the offset using the direction the Character is facing.
                switch(m_beingHeldBy.FacingDirection)
                {
                    case Character.Direction.South:
                        offset = new Vector2(0, -HOLD_OFFSET);
                        break;
                    case Character.Direction.West:
                        offset = new Vector2(-HOLD_OFFSET, 0);
                        break;
                    case Character.Direction.North:
                        offset = new Vector2(0, HOLD_OFFSET);
                        break;
                    case Character.Direction.East:
                        offset = new Vector2(HOLD_OFFSET, 0);
                        break;
                    default:
                        offset = Vector2.zero;
                        break;
                }

                // Set the final position of the LargeItem.
                Position = characterPos + offset;
            }
        }
    }
}
