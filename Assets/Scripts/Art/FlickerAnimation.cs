using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    public class FlickerAnimation : MonoBehaviour
    {
        /// <summary>
        /// If set the GameObject will be destroyed afterwards.
        /// </summary>
        public bool m_destroyAfter = false;

        /// <summary>
        /// The time that the FlickAnimation will be removed. -1 or 0 for never.
        /// </summary>
        public float m_stopAfter = 0f;

        private SpriteRenderer m_renderer;

        void Update()
        {
            if (m_renderer == null)
            {
                m_renderer = GetComponent<SpriteRenderer>();
                if (m_renderer == null)
                {
                    enabled = false;
                    return;
                }
            }

            m_renderer.enabled = ((long)(Time.time * 15f)) % 2 == 1;
            if (m_stopAfter > 0 && Time.time > m_stopAfter)
            {
                // DestroyAfter Destroys the entire GameObject, not just the flicker effect.
                if (m_destroyAfter)
                    Destroy(gameObject);
                else
                {
                    // Make sure it's visible when destroying.
                    m_renderer.enabled = true;

                    Destroy(this);
                }
            }
        }

        /// <summary>
        /// Adds the FlickerAnimation Component to a GameObject.
        /// </summary>
        /// <param name="go">The GameObject.</param>
        /// <param name="destroyAfter">If set the GameObject will Destroyed along with the component after the time has elapsed.</param>
        /// <param name="time">The amount of time the flick component will stay on them.</param>
        public static void StartFlickerOn(GameObject go, bool destroyAfter, float time)
        {
            // If there is already a flicker on this GameObject, don't do anything.
            if (go.TryGetComponent(out FlickerAnimation _))
                return;

            // Make sure there's a SpriteRender Component on the GameObject.
            if (go.TryGetComponent(out SpriteRenderer sprite))
            {
                // Add the Flicker
                FlickerAnimation flicker = go.AddComponent<FlickerAnimation>();
                flicker.m_renderer = sprite;
                flicker.m_destroyAfter = destroyAfter;
                flicker.m_stopAfter = Time.time + time;
            }
        }
    }
}
