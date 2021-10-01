using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    /// <summary>
    /// The base class for all items, characters, and mobs in the game.
    /// </summary>
    public abstract class Pawn : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource m_audioSource;

        /// <summary>
        /// The internal position of the pawn in floating space.
        /// This is not going to be the same as the position being displayed on screen.
        /// </summary>
        private Vector2 m_actualPosition;

        /// <summary>
        /// The Pawn size in diameter.
        /// </summary>
        public float m_pawnSizeDiameter = 1f;

#if DEBUG
        /// <summary>
        /// The color of the Gizmo cube that's drawn on this Pawn.
        /// </summary>
        public Color m_gizmoCubeColor = Color.yellow;
#endif

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
                    m_actualPosition.x = Mathf.Clamp(m_actualPosition.x, 0, size.X - 1);
                    m_actualPosition.y = Mathf.Clamp(m_actualPosition.y, 0, size.Y - 1);
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

#if DEBUG
        public void OnDrawGizmos()
        {
            // Since padding from the local 0,0,0 of the Pawn.
            // This is the center the cube on the Pawn.
            float padding = (1f - m_pawnSizeDiameter) / 2f;

            // The size of the cube
            Vector3 size = new Vector3(m_pawnSizeDiameter, m_pawnSizeDiameter, m_pawnSizeDiameter);

            #region Pixel Location Cube

            // Get the position
            Vector3 pos = PixelPosition;
            // Add the padding.
            pos += new Vector3(padding, padding, 0);
            // Since Gizmo cubes are dawn from the center, add half the size in all directions.
            pos += size / 2f;

            // Darken the color of the next cube.
            Color.RGBToHSV(m_gizmoCubeColor, out float hue, out float sat, out float vib);
            vib *= 0.5f;
            Gizmos.color = Color.HSVToRGB(hue, sat, vib);

            Gizmos.DrawWireCube(pos, size);

            #endregion

            #region Absolute Location Cube

            // Get the position
            pos = Position;
            // Add the padding.
            pos += new Vector3(padding, padding, 0);
            // Since Gizmo cubes are dawn from the center, add half the size in all directions.
            pos += size / 2f;

            Gizmos.color = m_gizmoCubeColor;
            Gizmos.DrawWireCube(pos, size);

            #endregion
        }
#endif

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

        /// <summary>
        /// Returns the current Tile Location that this Pawn is on.
        /// </summary>
        public Location GetCurrentTile()
        {
            // Get the current position of the Pawn.
            Vector2 pos = Position;

            // Get the middle of the Tile we're on.
            pos.x += 0.5f;
            pos.y += 0.5f;

            return new Location(pos);
        }

        protected void PlaySound(AudioClip clip)
        {
            if (clip != null && m_audioSource != null)
                m_audioSource.PlayOneShot(clip);
        }
    }
}
