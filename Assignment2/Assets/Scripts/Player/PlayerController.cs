using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float jumpVelocity = 10f;
    public float gravity = -9.8f;
    
    [Space(10)]
    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 0.15f;

    private float _lastGroundedTime;

    [Space(10)] [Header("Pausing")]
    [SerializeField] private InputAction PauseInput;

    private bool _autoMove;
    private bool _controlIsLocked;
    private Vector3 _autoMoveTarget;
    
    public event Action OnJumpEvent;
    
    private Vector2 _moveInput;
    private Vector3 _camForward;
    private Vector3 _camRight;
    private Vector3 _moveDirection;
    private CharacterController _characterController;
    private Quaternion _targetRotation;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _readyAttack;
    private bool _isAttacking;
    
    // Property of the variable so it may be accessed by other codes.
    public bool IsGrounded()
    {
        return _isGrounded;
    }
    public Vector3 GetPlayerVelocity()
    {
        return _velocity;
    }
    public bool ReadyAttack()
    {
        return _readyAttack;
    }
    public bool IsAttacking()
    {
        return _isAttacking;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
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
            return;
        }
        
        CalculateMovement();
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
    
    

    private void CalculateMovement()
    {
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
        
        //Calculate gravity
        _velocity = Vector3.up * _velocity.y + _moveDirection * moveSpeed;
        _velocity.y += gravity * Time.deltaTime;

        
        
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


