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

        [SerializeField]
        private AudioSource m_audioSource;

        [SerializeField]
        private AudioClip m_openSound, m_closeSound, m_lockedSound;

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
            // Switch based on the current status of the door.
            switch (Status)
            {
                // If it's locked and they have the key.
                case DoorStatus.Locked:
                    if (hasKey)
                    {
                        Status = DoorStatus.Open;
                        PlaySound(m_openSound);
                    }
                    else
                    {
                        PlaySound(m_lockedSound);
                    }
                    break;

                // If the door is closed, open it.
                case DoorStatus.Closed:
                    Status = DoorStatus.Open;
                    PlaySound(m_openSound);
                    break;

                // If the door is open, make sure there's nothing inside it first.
                case DoorStatus.Open:
                    // Doors usually have two or more Tiles inside them that make up the shape of the door.
                    DoorTile[] myTiles = GetComponentsInChildren<DoorTile>();
                    
                    // Create Rect(s) based on the Door Tiles.
                    Rect[] myTileRects = new Rect[myTiles.Length];
                    for (int i = 0; i < myTiles.Length; i++)
                        myTileRects[i] = new Rect(myTiles[i].transform.position, Vector2.one);

                    // Before closing, make sure there are no mobs or items in it.
                    foreach (Pawn pawn in FindObjectsOfType<Pawn>())
                    {
                        float pawnSize = 1f;

                        // If the pawn is a character it may have a custom size.
                        if (pawn is Character)
                        {
                            Character character = (Character)pawn;
                            pawnSize = character.m_pawnSizeDiameter;
                        }

                        // Get the position and make a Rect based on their size.
                        Vector2 pawnPosition = pawn.Position;

                        // The size of the pawn adjusts the position of it.
                        // Since the pivot point is bottom left we need to move the Rect up/right a little.
                        pawnPosition.x += (1f - pawnSize) / 2f;
                        pawnPosition.y += (1f - pawnSize) / 2f;

                        Rect pawnRect = new Rect(pawnPosition, new Vector2(pawnSize, pawnSize));

                        // If the Pawn overlaps one of the Door's Tiles, return.
                        for (int i = 0; i < myTiles.Length; i++)
                            if (myTileRects[i].Overlaps(pawnRect))
                                return;
                    }

                    Status = DoorStatus.Closed;
                    PlaySound(m_closeSound);
                    break;
            }
        }

        protected void PlaySound(AudioClip clip)
        {
            if (clip != null && m_audioSource != null)
                m_audioSource.PlayOneShot(clip);
        }
    }
}
