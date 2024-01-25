using System;
using Input;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputs))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonBasicController : MonoBehaviour
    {
        [field: Header("Player")]
        [field: Tooltip("Move speed of the character in m/s")]
        [field: SerializeField] public virtual float MoveSpeed { get; set; } = 4.0f;

        [field: Tooltip("Sprint speed of the character in m/s")]
        [field: SerializeField] public virtual float SprintSpeed { get; set; } = 8.0f;

        [field: Tooltip("Rotation speed of the character")]
        [field: SerializeField] public virtual float RotationSpeed { get; set; } = 1.0f;

        [field: Tooltip("Acceleration and deceleration")]
        [field: SerializeField] public virtual float SpeedChangeRate { get; set; } = 10.0f;

        [field: Space(10)]
        [field: Tooltip("The height the player can jump")]
        [field: SerializeField] public virtual float JumpHeight { get; set; } = 1.2f;

        [field: Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [field: SerializeField] public virtual float Gravity { get; set; } = -15.0f;

        [field: Space(10)]
        [field: Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [field: SerializeField] public virtual float JumpTimeout { get; set; } = 0.1f;

        [field: Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [field: SerializeField] public virtual float FallTimeout { get; set; } = 0.15f;

        [field: Header("Player Grounded")]
        [field: Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        [field: SerializeField] public virtual bool Grounded { get; set; } = true;

        [field: Tooltip("Useful for rough ground")]
        [field: SerializeField] public virtual float GroundedOffset { get; set; } = -0.14f;

        [field: Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [field: SerializeField] public virtual float GroundedRadius { get; set; } = 0.5f;

        [field: Tooltip("What layers the character uses as ground")]
        [field: SerializeField] public virtual LayerMask GroundLayers { get; set; }

        [field: Header("Cinemachine")]
        [field: Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [field: SerializeField] public virtual GameObject CinemachineCameraTarget { get; set; }

        [field: Tooltip("How far in degrees can you move the camera up")]
        [field: SerializeField] public virtual float TopClamp { get; set; } = 90.0f;

        [field: Tooltip("How far in degrees can you move the camera down")]
        [field: SerializeField] public virtual float BottomClamp { get; set; } = -90.0f;

        // cinemachine
        protected float _cinemachineTargetPitch;

        // player
        protected float _speed;
        protected float _rotationVelocity;
        protected float _verticalVelocity;
        protected float _terminalVelocity = 53.0f;

        // timeout deltatime
        protected float _jumpTimeoutDelta;
        protected float _fallTimeoutDelta;

        // components
        protected CharacterController _controller;
        protected PlayerInputs _input;
        protected GameObject _mainCamera;

#if ENABLE_INPUT_SYSTEM
        protected PlayerInput _playerInput;
#endif

        protected const float _threshold = 0.01f;
        
        protected virtual bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        protected virtual void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            if (Camera.main != null)
            {
                _mainCamera = Camera.main.gameObject;
            }

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        }

        protected virtual void Start()
        {
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        protected virtual void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        protected virtual void LateUpdate()
        {
            CameraRotation();
        }
        
        public virtual void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }
        
        public virtual void CameraRotation()
        {
            // if there is an input
            if (_input.Look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : UnityEngine.Time.deltaTime;
				
                _cinemachineTargetPitch += _input.Look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.Look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        public virtual void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.AnalogMovement ? _input.Move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, UnityEngine.Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.Move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.Move.x + transform.forward * _input.Move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * UnityEngine.Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * UnityEngine.Time.deltaTime);
        }
        
        public virtual void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= UnityEngine.Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= UnityEngine.Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * UnityEngine.Time.deltaTime;
            }
        }

        public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}