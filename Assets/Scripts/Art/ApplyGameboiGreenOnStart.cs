using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Art
{
    public class ApplyGameboiGreenOnStart : MonoBehaviour
    {
        [Tooltip("Randomize the hue of the original gameboy colors on start.")]
        public bool m_randomizeHue;

        void Start()
        {
            if (m_randomizeHue)
                GameboiTheme.s_defaultGameboyTheme.GetRandomTint().ApplyColorTheme();
            else
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
                GameboiTheme.s_defaultGameboyTheme.GetRandomTint().ApplyColorTheme();
            if (Input.GetKeyUp(KeyCode.G))
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme();
        }
    }
}