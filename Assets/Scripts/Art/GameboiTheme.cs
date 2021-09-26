// Written by Bennett Sewell
// 2021-09-23
// Stores GameBoy color pallets for the Gameboi Shader.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Art
{
    /// <summary>
    /// Stores color sets for the Gameboi shader.
    /// </summary>
    public struct GameboiTheme
    {
        /// <summary>
        /// The original gameboy green theme.
        /// </summary>
        public static readonly GameboiTheme s_defaultGameboyTheme =
            new GameboiTheme(0x081820, 0x346856, 0x88c070, 0xe0f8d0);

        /// <summary>
        /// The darkest pixel.
        /// </summary>
        public Color m_color1;

        /// <summary>
        /// Dark gray.
        /// </summary>
        public Color m_color2;

        /// <summary>
        /// Light gray.
        /// </summary>
        public Color m_color3;

        /// <summary>
        /// White pixel.
        /// </summary>
        public Color m_color4;

        /// <summary>
        /// Creates a color theme for the Gameboi shader.
        /// </summary>
        /// <param name="c1">The darkest pixel.</param>
        /// <param name="c2">Dark gray.</param>
        /// <param name="c3">Light gray.</param>
        /// <param name="c4">White pixel.</param>
        public GameboiTheme(Color c1, Color c2, Color c3, Color c4)
        {
            m_color1 = c1;
            m_color2 = c2;
            m_color3 = c3;
            m_color4 = c4;
        }

        /// <summary>
        /// Creates a color theme for the Gameboi shader.
        /// </summary>
        /// <param name="c1">The darkest pixel.</param>
        /// <param name="c2">Dark gray.</param>
        /// <param name="c3">Light gray.</param>
        /// <param name="c4">White pixel.</param>
        public GameboiTheme(int c1, int c2, int c3, int c4)
        {
            Color ConvertFromHex(int v)
            {
                int r = (v & 0xFF0000) >> 16;
                int g = (v & 0x00FF00) >> 8;
                int b = (v & 0x0000FF) >> 0;
                return new Color(r / 255f, g / 255f, b / 255f, 1f);
            }

            m_color1 = ConvertFromHex(c1);
            m_color2 = ConvertFromHex(c2);
            m_color3 = ConvertFromHex(c3);
            m_color4 = ConvertFromHex(c4);
        }

        /// <summary>
        /// Returns a randomly hued color set based on the original gameboy colors.
        /// </summary>
        public GameboiTheme GetRandomTint()
        {
            float hueShift = Random.value;

            Color ShiftHue(float shift, Color color)
            {
                Color.RGBToHSV(color, out float h, out float s, out float v);
                return Color.HSVToRGB((h + shift) % 1f, s, v);
            }

            return new GameboiTheme(
                ShiftHue(hueShift, s_defaultGameboyTheme.m_color1),
                ShiftHue(hueShift, s_defaultGameboyTheme.m_color2),
                ShiftHue(hueShift, s_defaultGameboyTheme.m_color3),
                ShiftHue(hueShift, s_defaultGameboyTheme.m_color4));
        }

        /// <summary>
        /// Applies this color set globally to the Gameboi shader.
        /// </summary>
        public void ApplyColorTheme()
        {
            Shader.SetGlobalColor(@"_GameboiColor1", m_color1);
            Shader.SetGlobalColor(@"_GameboiColor2", m_color2);
            Shader.SetGlobalColor(@"_GameboiColor3", m_color3);
            Shader.SetGlobalColor(@"_GameboiColor4", m_color4);
            if (Camera.main != null)
                Camera.main.backgroundColor = m_color3;
        }
    }

}