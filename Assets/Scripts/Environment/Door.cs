using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Environment
{
    public class Door : MonoBehaviour
    {
        [System.Serializable]
        public class DoorSettings
        {
            public SpriteRenderer m_door;
            public Sprite
                m_closed,
                m_locked,
                m_open;

            public void SetStatus(DoorStatus status)
            {
                switch (status)
                {
                    case DoorStatus.Closed:
                        m_door.sprite = m_closed;
                        break;
                    case DoorStatus.Locked:
                        m_door.sprite = m_locked;
                        break;
                    case DoorStatus.Open:
                        m_door.sprite = m_open;
                        break;
                }
            }
        }

        public enum DoorStatus : byte
        {
            Closed,
            Locked,
            Open,
        }

        [SerializeField]
        private DoorStatus m_doorStatus;

        public DoorStatus Status
        {
            get => m_doorStatus;
            set
            {
                m_doorStatus = value;
                m_doorA.SetStatus(value);
                m_doorB.SetStatus(value);
            }
        }

        [SerializeField]
        private DoorSettings m_doorA, m_doorB;

        void Start()
        {
            Status = Status;
        }

        public bool IsOpen => Status == DoorStatus.Open;
    }
}
