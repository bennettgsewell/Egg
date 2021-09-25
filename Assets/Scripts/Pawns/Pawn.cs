using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public abstract class Pawn : MonoBehaviour
    {
        /// <summary>
        /// The internal position of the pawn in floating space.
        /// This is not going to be the same as the position being displayed on screen.
        /// </summary>
        private Vector2 m_actualPosition;

        /// <summary>
        /// The position of the pawn.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return m_actualPosition;
            }
            set
            {
                m_actualPosition = value;
                transform.position = RoundToPixel(value);
            }
        }

        /// <summary>
        /// The display position of the pawn in pixels on screen.
        /// </summary>
        public Vector2 PixelPosition
        {
            get
            {
                return transform.position;
            }
            private set
            {
                Position = value;
            }
        }

        private void Start()
        {
            Position = transform.position;
        }

        /// <summary>
        /// Rounds the float to the nearest PPU.
        /// </summary>
        public float RoundToPixel(float value)
        {
            int ppu = ForceGameboyAspectRatio.PPU;
            return Mathf.Round(value * ppu) / ppu;
        }

        /// <summary>
        /// Rounds the Vector2 to the nearest PPU.
        /// </summary>
        public Vector2 RoundToPixel(Vector2 value)
        {
            return new Vector2(RoundToPixel(value.x), RoundToPixel(value.y));
        }
    }
}
