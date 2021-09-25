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
        /// Returns true if this Character is holding a LargeItem.
        /// </summary>
        public bool IsHoldingItem => m_holding != null;

        /// <summary>
        /// Moves the character in a direction.
        /// </summary>
        public void Move(Vector2 direction)
        {
            Position = Position + direction * Time.deltaTime * m_speed;

            // Determine the direction of the movement for the Animator.
            if(m_animator != null)
            {
                m_animator.SetBool("walk_east", direction.x > 0);
                m_animator.SetBool("walk_west", direction.x < 0);
                m_animator.SetBool("walk_north", direction.y > 0);
                m_animator.SetBool("walk_south", direction.y < 0);
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

            if(m_holding == null && item.m_beingHeldBy == null && (ignorePickupDistance || Vector2.Distance(item.Position, Position) < m_pickupDistance))
            {
                m_holding = item;
                m_holding.m_beingHeldBy = this;
                m_holding.MoveWithCharacter();
            }
        }

        /// <summary>
        /// Drops a LargeItem if one is currently being held.
        /// </summary>
        public void DropLargeItem()
        {
            if(m_holding != null)
            {
                m_holding.m_beingHeldBy = null;
                m_holding = null;
            }
        }
    }
}
