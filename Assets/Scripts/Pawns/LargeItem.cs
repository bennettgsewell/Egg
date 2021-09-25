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
        [NonSerialized]
        public Character m_beingHeldBy = null;

        public void MoveWithCharacter()
        {
            if(m_beingHeldBy != null)
            {
                Vector2 characterPos = m_beingHeldBy.Position;
                Position = characterPos + new Vector2(0, 0.5f);
            }
        }
    }
}
