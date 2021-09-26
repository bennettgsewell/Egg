using PHC.Environment;
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

                // The new position MUST be inside the map.
                Map map = GameManager.Instance?.TheMap;
                if (map != null)
                {
                    Location size = map.Size;

                    //Clamp it to be inside the map.
                    m_actualPosition.x = Mathf.Clamp(m_actualPosition.x, 0, size.X-1);
                    m_actualPosition.y = Mathf.Clamp(m_actualPosition.y, 0, size.Y-1);
                }

                transform.position = RoundToPixel(m_actualPosition);
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

        protected void Start()
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
