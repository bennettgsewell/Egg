using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.HUD
{
    public class Heart : MonoBehaviour
    {
        [SerializeField]
        private Sprite
            m_emptyHeart,
            m_fullHeart;

        [SerializeField]
        private SpriteRenderer m_renderer;

        public void SetStatus(bool isFull)
        {
            m_renderer.sprite = isFull ? m_fullHeart : m_emptyHeart;
        }
    }
}
