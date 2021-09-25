using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PHC.Pawns
{
    public class Kobold : Pawn
    {
        public InputActionAsset m_inputActionSet;

        // The direction the player is moving per frame.
        private Vector2 m_moving;

        // Map the inputs to their actions.
        void Start()
        {
            InputActionMap actionMap = m_inputActionSet.FindActionMap("Player", true);

            // Movement
            InputAction moveAction = actionMap.FindAction("Move", true);

            moveAction.performed += (context) =>
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
                    if(m_moving.x != 0)
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
            };

            moveAction.canceled += _ => m_moving = Vector2.zero;

            // Enable the player InputActionMap
            actionMap.Enable();
        }

        private void Update()
        {
            // Move every frame.
            Debug.Log("MOVING " + m_moving);
            Move(m_moving);
        }
    }
}
