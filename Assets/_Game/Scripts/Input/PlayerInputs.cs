using UnityEngine;
using UnityEngine.InputSystem;

namespace DLS.Input
{
    public class PlayerInputs : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        [field:Header("Input References")]
        [field:SerializeField] public InputActionReference MoveInputAction { get; set; }
        [field:SerializeField] public InputActionReference LookInputAction { get; set; }
        [field:SerializeField] public InputActionReference JumpInputAction { get; set; }
        [field:SerializeField] public InputActionReference SprintInputAction { get; set; }
        [field:SerializeField] public InputActionReference FireInputAction { get; set; }
        [field:SerializeField] public InputActionReference ReloadInputAction { get; set; }
        [field:SerializeField] public InputActionReference SwitchWeaponInputAction { get; set; }
        [field:SerializeField] public InputActionReference AimInputAction { get; set; }
#endif
        
        [field:Header("Character Input Values")]
        [field: SerializeField] public Vector2 Move { get; set; }
        [field: SerializeField] public Vector2 Look { get; set; }
        [field: SerializeField] public bool Jump { get; set; }
        [field: SerializeField] public bool Sprint { get; set; }
        [field: SerializeField] public bool FirePressed { get; set; }
        [field: SerializeField] public bool FireHeld { get; set; }
        [field: SerializeField] public bool FireReleased { get; set; }
        [field: SerializeField] public bool Reload { get; set; }
        [field: SerializeField] public float SwitchWeapon { get; set; }
        [field: SerializeField] public int SelectWeapon { get; set; }
        [field: SerializeField] public bool Aim { get; set; }
        
        [field:Header("Movement Settings")]
        [field: SerializeField] public bool AnalogMovement { get; set; }
        
        [field:Header("Mouse Cursor Settings")]
        [field: SerializeField] public bool CursorLocked { get; set; } = true;
        [field: SerializeField] public bool CursorInputForLook { get; set; } = true;


        protected virtual void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
        MoveInputAction.action.Enable();
        LookInputAction.action.Enable();
        JumpInputAction.action.Enable();
        SprintInputAction.action.Enable();
        FireInputAction.action.Enable();
        ReloadInputAction.action.Enable();
        SwitchWeaponInputAction.action.Enable();
        AimInputAction.action.Enable();
        MoveInputAction.action.performed += ctx => MoveInput(ctx.ReadValue<Vector2>());
        MoveInputAction.action.canceled += ctx => MoveInput(ctx.ReadValue<Vector2>());
        LookInputAction.action.performed += ctx => LookInput(ctx.ReadValue<Vector2>());
        LookInputAction.action.canceled += ctx => LookInput(Vector2.zero);
        JumpInputAction.action.performed += ctx => JumpInput(ctx.ReadValueAsButton());
        SprintInputAction.action.performed += ctx => SprintInput(ctx.ReadValueAsButton());
        SprintInputAction.action.canceled += ctx => SprintInput(false);
        FireInputAction.action.performed += ctx =>
        {
            if (ctx.action.WasPressedThisFrame())
            {
                FirePressedInput(true);
            }
            else if (ctx.action.WasReleasedThisFrame())
            {
                FirePressedInput(false);
            }
            FireHeldInput(ctx.ReadValueAsButton());
        };
        FireInputAction.action.canceled += ctx => FirePressedInput(false);
        ReloadInputAction.action.performed += ctx => ReloadInput(ctx.ReadValueAsButton());
        SwitchWeaponInputAction.action.performed += ctx => SwitchWeaponInput(ctx.ReadValue<float>());
        SwitchWeaponInputAction.action.canceled += ctx => SwitchWeaponInput(0f);
        AimInputAction.action.performed += ctx => AimInput(ctx.ReadValueAsButton());
        AimInputAction.action.canceled += ctx => AimInput(false);
#endif
        }

        protected virtual void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            MoveInputAction.action.Disable();
            LookInputAction.action.Disable();
            JumpInputAction.action.Disable();
            SprintInputAction.action.Disable();
            FireInputAction.action.Disable();
            ReloadInputAction.action.Disable();
            SwitchWeaponInputAction.action.Disable();
            AimInputAction.action.Disable();
#endif
        }

        protected virtual void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current.numpad1Key.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SelectWeapon = 1;
            }
            else if (Keyboard.current.numpad2Key.wasPressedThisFrame|| Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SelectWeapon = 2;
            }
            else if (Keyboard.current.numpad3Key.wasPressedThisFrame|| Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                SelectWeapon = 3;
            }
            else if (Keyboard.current.numpad4Key.wasPressedThisFrame|| Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                SelectWeapon = 4;
            }
            else if (Keyboard.current.numpad5Key.wasPressedThisFrame|| Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                SelectWeapon = 5;
            }
            else if (Keyboard.current.numpad6Key.wasPressedThisFrame|| Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                SelectWeapon = 6;
            }
            else if (Keyboard.current.numpad7Key.wasPressedThisFrame|| Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                SelectWeapon = 7;
            }
            else if (Keyboard.current.numpad8Key.wasPressedThisFrame|| Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                SelectWeapon = 8;
            }
            else if (Keyboard.current.numpad9Key.wasPressedThisFrame|| Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                SelectWeapon = 9;
            }
            else if (Keyboard.current.numpad0Key.wasPressedThisFrame|| Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                SelectWeapon = 0;
            }
            else
            {
                SelectWeapon = -1;
            }
#endif
        }


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
            
            public void FirePressedInput(bool newFireState)
            {
                FireReleased = !newFireState;
                FirePressed = newFireState;
            }
            
            public void FireHeldInput(bool newFireState)
            {
                FireHeld = newFireState;
            }
            
            public void ReloadInput(bool newReloadState)
            {
                Reload = newReloadState;
            }
            
            
            public void SwitchWeaponInput(float nextWeaponState)
            {
                SwitchWeapon = nextWeaponState;
            }
            
            public void AimInput(bool newAimState)
            {
                Aim = newAimState;
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