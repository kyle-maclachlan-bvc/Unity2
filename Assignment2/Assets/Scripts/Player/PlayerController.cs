using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MovementHandler _movementHandler;
    private GroundChecker _groundChecker;
    
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
    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;
    private Vector3 _defaultAimTrackerPosition;
    private Vector3 _tempAimTrackerPosition;
    

    private PlayerState _currentState;
    
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
        _movementHandler = new MovementHandler();
        _groundChecker = new GroundChecker();
        
        // set the default state
        _currentState = PlayerState.EXPLORE;
        OnStateUpdated?.Invoke(_currentState);
        
        _characterController = GetComponent<CharacterController>(); // Set up the Character Controller
        _defaultAimTrackerPosition = aimTrack.localPosition;        // Tracker Position
    }

    // Update is called once per frame
    void Update()
    {
        if (HandleAutoMove()) return;
        if (_controlIsLocked) return;
        HandleMovement();
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _isGrounded = _groundChecker.CheckGrounded(
            transform,
            groundCheckOffset,
            groundCheckRadius,
            groundCheckDistance,
            groundLayer,
            coyoteTime
        );
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -0.2f;
        }
    }

    #region Input
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

    private void HandleMovement()
    {
        switch (_currentState)
        {
            case PlayerState.EXPLORE:
                HandleExploreMovement();
                break;
            case PlayerState.AIM:
                HandleAimMovement();
                break;
        }
    }

    private void HandleExploreMovement()
    {
        _velocity = _movementHandler.CalculateExploreMovement(
            _moveInput,
            playerCamera,
            moveSpeed,
            rotationSpeed,
            transform,
            ref _velocity,
            gravity
        );
        aimTrack.localPosition = _defaultAimTrackerPosition;
    }

    private void HandleAimMovement()
    {
        _velocity = _movementHandler.CalculateAimMovement(
            _moveInput,
            _lookInput,
            moveSpeedAim,
            rotationSpeedAim,
            transform,
            ref _velocity,
            gravity
        );

        UpdateAimTrack();
    }

    private void UpdateAimTrack()
    {
        _tempAimTrackerPosition = aimTrack.localPosition;
        _tempAimTrackerPosition.y -= _lookInput.y * rotationSpeedAim * Time.deltaTime;
        _tempAimTrackerPosition.y = Mathf.Clamp(_tempAimTrackerPosition.y, minAimHeight, maxAimHeight);
        aimTrack.localPosition = _tempAimTrackerPosition;
    }
    #endregion
    
    #region Grounding
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance/2, 
                    new Vector3(1.5f* groundCheckRadius, groundCheckDistance , 1.5f * groundCheckRadius) );
    }
    #endregion

    #region Input
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
    #endregion

    #region AutoMove
    public void MoveToPosition(Vector3 target)
    {
        _autoMove = true;
        _controlIsLocked = true;
        _autoMoveTarget = target;
        LevelClear.Instance.ShowLevelClear("You Cleared the Level");
    }

    private bool HandleAutoMove()
    {
        if (!_autoMove) return false;
        AutoMove();
        return true;
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
    #endregion
}