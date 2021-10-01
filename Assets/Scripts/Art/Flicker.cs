using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    /// <summary>
    /// When attached to a GameObject with SpriteRenderers, it will cause them to flicker in a classic arcade game style.
    /// </summary>
    public class Flicker : MonoBehaviour
    {
        /// <summary>
        /// The default amount of times to flicker per second.
        /// </summary>
        public const float DEFAULT_FLICKERS_PER_SECOND = 15f;

        /// <summary>
        /// The default amount of time for a death flicker.
        /// </summary>
        public const float DEFAULT_TIME = 0.5f;

        /// <summary>
        /// If set the GameObject will be destroyed afterwards.
        /// </summary>
        public bool DestroyGO { set; get; }

        /// <summary>
        /// The time that the FlickAnimation will be removed. -1 or 0 for never.
        /// </summary>
        public float WillEndAfter { set; get; }

        /// <summary>
        /// The amount of times the SpriteRenderer(s) should flicker per second.
        /// </summary>
        public float FlickersPerSecond { set; get; }

        private SpriteRenderer[] m_renderers;

        private void Update()
        {
            // If there are no known SpriteRender(s)
            if (m_renderers == null)
            {
                // Find all of the SpriteRenderers on the GameObject and its children.
                m_renderers = GetComponentsInChildren<SpriteRenderer>(false);

                // If no SpriteRender(s) were found, just disable this Component.
                if (m_renderers == null)
                {
                    Done();
                    return;
                }
            }

            // Cause the flicker based on time.
            bool enable = ((long)(Time.time * FlickersPerSecond)) % 2 == 1; ;

            for (int i = 0; i < m_renderers.Length; i++)
                m_renderers[i].enabled = enable;

            // If WillEndAfter <= 0 it will flicker for inifinity.
            // Call Done() when the time is up.
            if (WillEndAfter > 0 && Time.time > WillEndAfter)
                Done();
        }

        /// <summary>
        /// Adds the FlickerAnimation Component to a GameObject.
        /// </summary>
        /// <param name="go">The GameObject.</param>
        /// <param name="destroyAfter">If set the GameObject will Destroyed along with the component after the time has elapsed.</param>
        /// <param name="time">The amount of time the flick component will stay on them.</param>
        /// <param name="flickersPerSecond">The amount of times to flicker per second.</param>
        public static void StartFlickerOn(GameObject go, bool destroyAfter, float time = DEFAULT_TIME, float flickersPerSecond = DEFAULT_FLICKERS_PER_SECOND)
        {
            // Make sure there's a GameObject.
            if (go == null)
                return;

            // If there is already a flicker on this GameObject, modify the settings on it.
            // Otherwise, add a new one.
            Flicker flicker;
            if (!go.TryGetComponent(out flicker))
                flicker = go.AddComponent<Flicker>();

            // Set the values on it.
            flicker.DestroyGO = destroyAfter;
            flicker.WillEndAfter = Time.time + time;
            flicker.FlickersPerSecond = flickersPerSecond;
        }

        /// <summary>
        /// Calls to Destroy this component and GameObject if set.
        /// </summary>
        private void Done() => Destroy(DestroyGO ? gameObject : this);
    }
}
