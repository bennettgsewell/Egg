using PHC.Pawns;
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

        /// <summary>
        /// Attempts to find the Door in this Tile.
        /// </summary>
        public static Door IsTileDoor(Location pos)
        {
            foreach (var dt in GameObject.FindObjectsOfType<DoorTile>())
            {
                if (dt.GetCurrentTile() == pos)
                {
                    return dt.m_door;
                }
            }
            return null;
        }

        public void AttemptToOpen(bool hasKey)
        {
            if(Status == DoorStatus.Locked)
            {
                if (hasKey)
                    Status = DoorStatus.Open;
            }
            else
            {
                if (Status == DoorStatus.Closed)
                    Status = DoorStatus.Open;
                else
                {
                    DoorTile[] myTiles = GetComponentsInChildren<DoorTile>();
                    Rect[] myTileRects = new Rect[myTiles.Length];
                    for (int i = 0; i < myTiles.Length; i++)
                    {
                        myTileRects[i] = new Rect(myTiles[i].transform.position, Vector2.one);
                    }

                    // Before closing, make sure there are no mobs or items in it.
                    Pawn[] pawns = FindObjectsOfType<Pawn>();
                    foreach(Pawn pawn in pawns)
                    {
                        float pawnSize = 1f;

                        if(pawn is Character)
                        {
                            Character character = (Character)pawn;
                            pawnSize = character.m_pawnSizeDiameter;
                        }

                        Vector2 pawnAbsPos = pawn.Position;
                        Rect pawnRect = new Rect(pawnAbsPos, new Vector2(pawnSize, pawnSize));

                        // If a Pawn overlaps one of the Door's Tiles, return.
                        for (int i = 0; i < myTiles.Length; i++)
                        {
                            if (myTileRects[i].Overlaps(pawnRect))
                                return;
                        }
                    }

                    Status = DoorStatus.Closed;
                }
            }
        }
    }
}
