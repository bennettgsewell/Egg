using PHC.Art;
using PHC.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PHC.Pawns
{
    /// <summary>
    /// The player character.
    /// </summary>
    public class Kobold : Character
    {
        [System.Serializable]
        public class KoboldDirectionSprites
        {
            public Sprite
                m_idle,
                m_walk1,
                m_walk2,
                m_attack,
                m_sword1,
                m_sword2,
                m_sword3;
            public Vector3 m_swordLocalPosition;
        }

        public KoboldDirectionSprites m_spritesNorth, m_spritesEast, m_spritesSouth, m_spritesWest;

        public InputActionAsset m_inputActionSet;

        // The direction the player is moving per frame.
        private Vector2 m_moving;

        [SerializeField]
        private AudioClip m_damagedClip;

        [SerializeField]
        private SpriteRenderer m_renderer;

        [SerializeField]
        private SpriteRenderer m_swordRenderer;

        private int m_swordLevel = 1;

        // This is when the attack animation ends.
        private float m_attackEnds;

        // Map the inputs to their actions.
        void Start()
        {
            base.Start();

            InputActionMap actionMap = m_inputActionSet.FindActionMap("Player", true);

            // Movement
            InputAction
                moveAction = actionMap.FindAction("Move", true),
                useAction = actionMap.FindAction("Use", true),
                attackAction = actionMap.FindAction("Attack", true),
                pauseAction = actionMap.FindAction("Pause", true),
                openInventoryAction = actionMap.FindAction("Open Inventory", true);

            moveAction.performed += MoveInput_performed;
            moveAction.canceled += _ => m_moving = Vector2.zero;

            useAction.performed += UseAction_performed;

            pauseAction.performed += (c) =>
            {
                GameObject.FindObjectOfType<Jelly>()?.Kill();
            };

            openInventoryAction.performed += (c) =>
            {
                m_swordLevel++;
                if (m_swordLevel == 4)
                    m_swordLevel = 1;

            };

            attackAction.performed += AttackAction_performed;

            // Enable the player InputActionMap
            actionMap.Enable();
        }

        private void AttackAction_performed(InputAction.CallbackContext obj)
        {
            // If we're in the middle of an Attack we cannot start another.
            if (Time.time < m_attackEnds)
                return;

            // This will start the animation and pause movement.
            m_attackEnds = Time.time + 0.3f;

            // Damage all enemies inside zone
            Vector2 hitbotPos = Position;
            switch (FacingDirection)
            {
                case Direction.East: hitbotPos.x++; break;
                case Direction.South: hitbotPos.y--; break;
                case Direction.West: hitbotPos.x--; break;
                case Direction.North: hitbotPos.y++; break;
                default: throw new System.Exception();
            }
            Rect hitbox = new Rect(hitbotPos, Vector2.one);

            // Get all Monster objects and compare them to the Hitbox
            Monster[] monsters = FindObjectsOfType<Monster>();
            foreach (var monster in monsters)
            {
                if (hitbox.Contains(monster.Position + new Vector2(0.5f, 0.5f)))
                {
                    monster.DealDamage(m_swordLevel);
                }
            }
        }

        // The "use" button was pressed.
        private void UseAction_performed(InputAction.CallbackContext obj)
        {
            if (IsHoldingItem)
            {
                if (HeldItem is Key)
                {
                    Door door = Door.IsTileDoor(FacingDirection.Shift(GetCurrentTile()));
                    if (door != null && door.Status == Door.DoorStatus.Locked)
                    {
                        door.AttemptToOpen(true);
                        LargeItem held = HeldItem;
                        DropLargeItem();
                        Destroy(held.gameObject);
                    }
                }

                DropLargeItem();
            }
            else
            {
                // Attempt to pickup item.
                if (!AttemptToPickupLargeItem())
                {
                    // If failed to pickup an item.
                    // Try opening door.
                    Door.IsTileDoor(FacingDirection.Shift(GetCurrentTile()))?.AttemptToOpen(false);
                }
            }
        }

        /// <summary>
        /// Attempts to pickup the closest LargeItem.
        /// </summary>
        private bool AttemptToPickupLargeItem()
        {
            // Get all of the LargeItems
            LargeItem[] largeItems = FindObjectsOfType<LargeItem>();

            // The point to calculate distance from.
            Vector2 lookNear = Position;

            // This will be the closest item to the character.
            LargeItem closestItem = null;
            float shortestDistance = float.PositiveInfinity;

            // Iterate through all the LargeItems in the game.
            for (int i = 0; i < largeItems.Length; i++)
            {
                Vector2 itemPos = largeItems[i].Position;

                // Get the distance of this item.
                float distance = Vector2.Distance(lookNear, itemPos);

                // If this item is closer than the previously looked at.
                if (distance < shortestDistance)
                {
                    // This is now the closest item.
                    shortestDistance = distance;
                    closestItem = largeItems[i];
                }
            }

            return PickupLargeItem(closestItem, false);
        }

        // Handles movement input.
        private void MoveInput_performed(InputAction.CallbackContext context)
        {
            // Set the moving direction of the player.
            Vector2 newDir = context.ReadValue<Vector2>();
            // Round input to 1s or 0s
            newDir.x = Mathf.Round(newDir.x);
            newDir.y = Mathf.Round(newDir.y);

            // If new input is a diagonal
            if (newDir.x != 0 && newDir.y != 0)
            {
                // If just started moving, and input was a diagonal, cancel.
                if (context.phase == InputActionPhase.Started)
                {
                    m_moving = Vector2.zero;
                    return;
                }

                // Find the axis of the old movement.
                if (m_moving.x != 0)
                {
                    // Was moving on the X axis.
                    // Switch to the Y axis.
                    newDir.x = 0;
                }
                else
                {
                    // Was moving on the Y aixs.
                    // Switch to the X axis.
                    newDir.y = 0;
                }


                m_moving = newDir;
            }
            else
            {
                // This will only be called if not a diagonal or 0,0 input.
                m_moving = newDir;
            }
        }

        protected void Update()
        {
            bool freezeMovement = false;

            KoboldDirectionSprites spriteSet;

            switch (FacingDirection)
            {
                case Direction.East: spriteSet = m_spritesEast; break;
                case Direction.South: spriteSet = m_spritesSouth; break;
                case Direction.West: spriteSet = m_spritesWest; break;
                case Direction.North: spriteSet = m_spritesNorth; break;
                default: throw new System.Exception();
            }

            // Handle the animations
            if (Time.time < m_attackEnds)
            {
                m_renderer.sprite = spriteSet.m_attack;
                freezeMovement = true;

                m_swordRenderer.enabled = true;
                m_swordRenderer.transform.localPosition = spriteSet.m_swordLocalPosition;
                switch (m_swordLevel)
                {
                    case 1: m_swordRenderer.sprite = spriteSet.m_sword1; break;
                    case 2: m_swordRenderer.sprite = spriteSet.m_sword2; break;
                    case 3: m_swordRenderer.sprite = spriteSet.m_sword3; break;
                }
            }
            else
            {
                m_swordRenderer.enabled = false;

                if (m_moving != Vector2.zero)
                {
                    m_renderer.sprite = ((long)(Time.time / 0.3f)) % 2 == 0 ? spriteSet.m_walk1 : spriteSet.m_walk2;
                }
                else
                {
                    m_renderer.sprite = spriteSet.m_idle;
                }
            }

            // Move every frame.
            if (!freezeMovement)
            {
                Location currentTile = GetCurrentTile();
                Move(m_moving);
                Location newTile = GetCurrentTile();

                // Victory check, see if they're standing on a set of stairs if the current tile has changed.
                if (currentTile != newTile)
                {
                    if (IsHoldingItem && HeldItem is Egg)
                    {
                        foreach (Stairs stair in FindObjectsOfType<Stairs>())
                        {
                            if (newTile == stair.GetCurrentTile() && stair.m_direction == Stairs.StairDirection.Ascending)
                            {
                                GameManager.Instance?.NextLevel();
                            }
                        }
                    }
                }

            }
        }

        public override void Kill()
        {
            DropLargeItem();
            FlickerAnimation.StartFlickerOn(gameObject, false, 0.5f);
            TookDamage();
        }

        public override void TookDamage()
        {
            PlaySound(m_damagedClip);
        }
    }
}
