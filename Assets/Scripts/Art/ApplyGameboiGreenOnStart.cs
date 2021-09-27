using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Art
{
    public class ApplyGameboiGreenOnStart : MonoBehaviour
    {
        [Tooltip("Randomize the hue of the original gameboy colors on start.")]
        public bool m_randomizeHue;

        [Tooltip("Which of the 4 Gameboy colors to use as the Camera's background.")]
        [SerializeField]
        public int m_bgColor = 3;

        void Start()
        {
            if (m_randomizeHue)
                GameboiTheme.s_defaultGameboyTheme.GetRandomTint().ApplyColorTheme(m_bgColor);
            else
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme(m_bgColor);
        }

        /*
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
                GameboiTheme.s_defaultGameboyTheme.GetRandomTint().ApplyColorTheme();
            if (Input.GetKeyUp(KeyCode.G))
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme();
        }
        */
    }
}