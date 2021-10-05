using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PHC.Assets.Scripts.Art
{
    /// <summary>
    /// A GameBoy 15 bit color.
    /// </summary>
    [Serializable]
    public struct GColor
    {
        public const ushort R_BITS = 0x001F;
        public const ushort G_BITS = 0x03E0;
        public const ushort B_BITS = 0x7C00;

        /// <summary>
        /// The 15 bit color raw value.
        /// </summary>
        public ushort m_value;

        /// <summary>
        /// Returns the percetage of a specific color inside m_value.
        /// </summary>
        private float GetFloatFor(ushort BITS) => (m_value & BITS) / (float)BITS;

        /// <summary>
        /// Converts a float into the unsigned 5-bit int, then sets the bits inside the m_value accordingly.
        /// </summary>
        private void SetFloatFor(ushort BITS, float value)
        {
            // Get the current value.
            int val = m_value;
            // Zero out the bits in the portion we're modifying.
            val = val & (~BITS);
            // Set the new bits
            val = val | ((int)(value * BITS) & BITS);
            // Set the new m_value.
            m_value = (ushort)val;
        }

        /// <summary>
        /// Red percentage
        /// </summary>
        public float R
        {
            get => GetFloatFor(R_BITS);
            set => SetFloatFor(R_BITS, value);
        }

        /// <summary>
        /// Green percentage
        /// </summary>
        public float G
        {
            get => GetFloatFor(G_BITS);
            set => SetFloatFor(G_BITS, value);
        }

        /// <summary>
        /// Blue percentage
        /// </summary>
        public float B
        {
            get => GetFloatFor(B_BITS);
            set => SetFloatFor(B_BITS, value);
        }

        // Constructors
        public GColor(ushort color) { m_value = color; }
        public GColor(int color) : this(unchecked((ushort)color)) { }
        public GColor(long color) : this(unchecked((ushort)color)) { }
        public GColor(float r, float g, float b) : this((ushort)0)
        {
            R = r;
            G = g;
            B = b;
        }
        public GColor(Color color) : this(color.r, color.g, color.b) { }

        // GColor can be implicitly case to Color and vise versa.
        public static implicit operator Color(GColor color) => new Color(color.R, color.G, color.B);
        public static implicit operator GColor(Color color) => new GColor(color);
        public static implicit operator GColor(ushort color) => new GColor(color);
        public static implicit operator GColor(int color) => new GColor(color);
        public static implicit operator GColor(long color) => new GColor(color);
        public static implicit operator ushort(GColor color) => color.m_value;

        public override string ToString() => $"GColor({R}, {G}, {B})";
    }
}
