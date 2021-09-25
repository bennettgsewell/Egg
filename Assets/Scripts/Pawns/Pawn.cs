using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public abstract class Pawn : MonoBehaviour
    {
        /// <summary>
        /// The current position of the pawn.
        /// </summary>
        public Vector2 Position { get => transform.position; set => transform.position = value; }

        /// <summary>
        /// Moves the character in a direction.
        /// </summary>
        public void Move(Vector2 direction)
        {
        }
    }
}
