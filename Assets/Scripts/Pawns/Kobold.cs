using PHC.Art;
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
        public InputActionAsset m_inputActionSet;

        // The direction the player is moving per frame.
        private Vector2 m_moving;

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
                GameObject.FindObjectOfType<Jelly>()?.SetDestinationToClosestPawnOfType<Egg>(out Egg _);
            };

            openInventoryAction.performed += (c) =>
            {
                GameObject.FindObjectOfType<Jelly>()?.SetDestinationToClosestPawnOfType<EggHole>(out EggHole _);
            };

            // Enable the player InputActionMap
            actionMap.Enable();
        }

        // The "use" button was pressed.
        private void UseAction_performed(InputAction.CallbackContext obj)
        {
            if (IsHoldingItem)
                DropLargeItem();
            else
                AttemptToPickupLargeItem();
        }

        /// <summary>
        /// Attempts to pickup the closest LargeItem.
        /// </summary>
        private void AttemptToPickupLargeItem()
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

            PickupLargeItem(closestItem, false);
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
            // Move every frame.
            Move(m_moving);
        }
    }
}
