using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    public class ForceGameboyAspectRatio : MonoBehaviour
    {
        // The original Gameboy screen resolution.
        public const int SCREEN_WIDTH = 160, SCREEN_HEIGHT = 144;

        // Pixels per unit.
        public const int PPU = 16;

        // The screen ratio.
        public const float CORRECT_ASPECT = SCREEN_HEIGHT / (float)SCREEN_WIDTH;

        // The camera on this GameObject.
        private Camera m_camera;

        void Start()
        {
            // Get the camera on this GameObject.
            m_camera = GetComponent<Camera>();

            // Disable if a Camera wasn't found.
            if (m_camera == null)
                enabled = false;

            Application.targetFrameRate = 59;
        }

        // Update every frame in case the user changes window size.
        void Update()
        {
            // The height of the camera should be a total of 9 units, 144px.
            // Camera.orthographicSize is like a radius, so it's halfed.
            m_camera.orthographicSize = SCREEN_HEIGHT / PPU / 2f;

            // Force the correct aspect ratio, since the width of the camera is dynamic.
            m_camera.rect = new Rect((1f - CORRECT_ASPECT) / 2f, 0, CORRECT_ASPECT, 1);
        }
    }
}
