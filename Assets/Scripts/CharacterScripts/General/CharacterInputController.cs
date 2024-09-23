using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputController : MonoBehaviour
{
        public Vector2 moveInput;
        public Vector2 aimInput;
        public bool dashInput;
        public bool parryInput;
        public bool activeInput;
        public bool jumpInput;
        public bool attackInput;
        public void SetMoveInput(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void SetDashInput(InputAction.CallbackContext context)
        {
            dashInput = context.action.triggered;
        }

        public void SetParryInput(InputAction.CallbackContext context)
        {
            parryInput = context.action.triggered;
        }

        public void SetActiveInput(InputAction.CallbackContext context)
        {
            activeInput = context.action.triggered;
        }

        public void SetJumpInput(InputAction.CallbackContext context)
        {
            jumpInput = context.action.triggered;
        }

        public void SetAttackInput(InputAction.CallbackContext context)
        {
            attackInput = context.action.triggered;
        }

        public void SetAimInput(InputAction.CallbackContext context)
        {
            aimInput = context.ReadValue<Vector2>();
        }

        private void LateUpdate() 
        {
            dashInput = false;
            parryInput = false;
            jumpInput = false;
            attackInput = false;
            activeInput = false;

        }
}
