using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Input
{
    public class PlayerInputs : MonoBehaviour
    {
        [field:Header("Character Input Values")]
        [field: SerializeField] public Vector2 Move { get; set; }
        [field: SerializeField] public Vector2 Look { get; set; }
        [field: SerializeField] public bool Jump { get; set; }
        [field: SerializeField] public bool Sprint { get; set; }
        [field: SerializeField] public bool Fire { get; set; }
        
        [field:Header("Movement Settings")]
        [field: SerializeField] public bool AnalogMovement { get; set; }
        
        [field:Header("Mouse Cursor Settings")]
        [field: SerializeField] public bool CursorLocked { get; set; } = true;
        [field: SerializeField] public bool CursorInputForLook { get; set; } = true;
        
#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if(CursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        
        public void OnFire(InputValue value)
        {
            FireInput(value.isPressed);
        }
#endif
            public void MoveInput(Vector2 newMoveDirection)
            {
                Move = newMoveDirection;
            } 
    
            public void LookInput(Vector2 newLookDirection)
            {
                Look = newLookDirection;
            }
    
            public void JumpInput(bool newJumpState)
            {
                Jump = newJumpState;
            }
    
            public void SprintInput(bool newSprintState)
            {
                Sprint = newSprintState;
            }
            
            public void FireInput(bool newFireState)
            {
                Fire = newFireState;
            }
        
            private void OnApplicationFocus(bool hasFocus)
            {
                SetCursorState(CursorLocked);
            }

            private void SetCursorState(bool newState)
            {
                Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            }
    }
}