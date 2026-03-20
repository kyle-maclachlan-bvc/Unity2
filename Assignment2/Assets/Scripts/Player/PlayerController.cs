using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("EXPLORE Movement")]                            // All Variables required for Explore Movement Actions
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float jumpVelocity = 10f;
    public float gravity = -9.8f;

    [Space(10)] [Header("AIM Movement")]            // All Variables required for Aim Movement Actions
    [SerializeField] private float moveSpeedAim = 2;
    [SerializeField] private float rotationSpeedAim = 10f;
    [SerializeField] private Transform aimTrack;
    [SerializeField] private float maxAimHeight;
    [SerializeField] private float minAimHeight;
    
    [Space(10)]
    [Header("Ground Check")]                                // All Variables required to GroundCheck for Jumping
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 0.15f;

    private float _lastGroundedTime;    // Helps with coyoteTime, does not require SerializeField

    [Space(10)]
    [Header("Pausing")]                                     // All variables required for pausing the game
    [SerializeField] private InputAction PauseInput;

    // All Variables required for Clearing Level Animation Loop
    private bool _autoMove;
    private bool _controlIsLocked;
    private Vector3 _autoMoveTarget;
    
    public event Action OnJumpEvent;                    // The OnJumpEvent sent to PlayerAnimator
    public event Action<PlayerState> OnStateUpdated;    // The OnStateUpdate sent to CameraSwitch
    
    // Private Variables not to be adjusted or preset
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _camForward;
    private Vector3 _camRight;
    private Vector3 _moveDirection;
    private CharacterController _characterController;
    private Quaternion _targetRotation;
    private Vector3 _velocity;
    private bool _isGrounded;
    private Vector3 _defaultAimTrackerPosition;
    private Vector3 _tempAimTrackerPosition;
    
    // Variable for changing player state
    private PlayerState _currentState;
    
    // Property of the variable so it may be accessed by other codes, but not editable
    public bool IsGrounded()
    {
        return _isGrounded;
    }
    public Vector3 GetPlayerVelocity()
    {
        return _velocity;
    }
    
    void Start()
    {
        // set the default state
        _currentState = PlayerState.EXPLORE;
        OnStateUpdated?.Invoke(_currentState);
        
        // set up the character controller
        _characterController = GetComponent<CharacterController>();
        
        // Tracker position
        _defaultAimTrackerPosition = aimTrack.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_autoMove)
             {
                 AutoMove();
                 return;
             }

        if (_controlIsLocked)
        {
            // Stop the player from Moving and Aiming during Clear Level
            return;
        }

        if (_currentState == PlayerState.EXPLORE)
        {
            CalculateMovementExplore();
            aimTrack.localPosition = _defaultAimTrackerPosition;
        }
        else if (_currentState == PlayerState.AIM)
        {
            CalculateMovementAim();
            UpdateAimTrack();
        }

        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -0.2f;
        }
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if(_isGrounded)
        {
            //Debug.Log("JUMP");
            AudioManager.Instance.PlayJumpSFX();
            _velocity.y = jumpVelocity;
            OnJumpEvent?.Invoke();
        }
    }

    public void OnAim(InputValue value)
    {
            _currentState = value.isPressed ? PlayerState.AIM : PlayerState.EXPLORE;
            
            if (_currentState == PlayerState.AIM)
            {
                _camForward = playerCamera.transform.forward;
                _camForward.y = 0;
                _camForward.Normalize();
                transform.rotation = Quaternion.LookRotation(_camForward);
            }
            
            OnStateUpdated?.Invoke(_currentState);
    }

    private void CalculateMovementExplore()
    {
        // This is for the Explore Camera-based movement
        _camForward = playerCamera.transform.forward;
        _camRight = playerCamera.transform.right;
        _camForward.y = 0;
        _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();

        _moveDirection = _camRight * _moveInput.x + _camForward * _moveInput.y;

        if(_moveDirection.sqrMagnitude > 0.01f)
        {
            _targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        //Calculate gravity, keeping the player to the ground
        _velocity = _velocity.y * Vector3.up + moveSpeed * _moveDirection ;
        _velocity.y += gravity * Time.deltaTime;
    }

    private void CalculateMovementAim()
    {
        // Rotate the player around the Y Axis based on X(Horizontal Input)
        transform.Rotate(Vector3.up, rotationSpeedAim * _lookInput.x * Time.deltaTime);
        
        // WASD relates to where the player currently faces
        // Left / Right = Strafing (sideways), forward / back = player's facing directions
        _moveDirection = _moveInput.x * transform.right + _moveInput.y * transform.forward;
        
        _velocity = _velocity.y * Vector3.up + moveSpeedAim * _moveDirection;
        _velocity.y += gravity * Time.deltaTime;
    }

    private void UpdateAimTrack()
    {
        _tempAimTrackerPosition = aimTrack.localPosition;
        _tempAimTrackerPosition.y -= _lookInput.y * rotationSpeedAim * Time.deltaTime;
        _tempAimTrackerPosition.y = Mathf.Clamp(_tempAimTrackerPosition.y, minAimHeight, maxAimHeight);
        aimTrack.localPosition = _tempAimTrackerPosition;
    }

    public void CheckGrounded()
    {
        _isGrounded = Physics.SphereCast(
            transform.position + groundCheckOffset,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );

        if (_isGrounded)
            _lastGroundedTime = Time.time;

        _isGrounded = Time.time - _lastGroundedTime <= coyoteTime;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance/2, 
                    new Vector3(1.5f* groundCheckRadius, groundCheckDistance , 1.5f * groundCheckRadius) );
    }

    void OnEnable()
    {
        PauseInput.Enable();
        PauseInput.performed += HandlePause;
    }

    void OnDisable()
    {
        PauseInput.performed -= HandlePause;
    }

    void HandlePause(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePause();
    }

    public void MoveToPosition(Vector3 target)
    {
        _autoMove = true;
        _controlIsLocked = true;
        _autoMoveTarget = target;
        
        LevelClear.Instance.ShowLevelClear("You Cleared the Level");
    }

    void AutoMove()
    {
        Vector3 direction = (_autoMoveTarget - transform.position);
        direction.y = 0;

        if (direction.magnitude < 0.1f)
        {
            _autoMove = false;
            _velocity = Vector3.zero;
            return;
        }

        direction.Normalize();
        
        _characterController.Move(direction * moveSpeed * Time.deltaTime);

        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }
}


