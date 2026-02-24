using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Camera playerCamera;           // Sets up the Camera to follow the player
    [SerializeField] private float moveSpeed = 10f;         
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -5f;

    [Space(10)]
    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    
    private Vector2 _moveInput;             // movement input
    private Vector3 _camForward;            // the forward (+) and backwards (-) direction of the camera
    private Vector3 _camRight;              // the right (+) and left (-) direction of the camera
    private Vector3 _moveDirection;         // applies direction
    private Quaternion _targetRotation;     // applies character rotation
    private Vector3 _velocity;              // used to help calculate gravity
    
    
    
    private CharacterController _characterController;
    public void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Update()
    {
        CalculateMovement();
        _characterController.Move(_velocity * Time.deltaTime);
    }
    
    public void OnMove(InputValue value)
    {
        // Movement Controls, this piece is not rotation
        _moveInput = value.Get<Vector2>(); 
        
        // Movement Controls, rotation commands
    }

    public void OnJump()
    {
        Debug.Log("JUMP!");
        _velocity.y = jumpForce;
        if (isGrounded())
        {
            Debug.Log("GROUNDED!");
        }
    }

    private void CalculateMovement()
    {
        // self-contained calculations for controls
        _camForward = playerCamera.transform.forward;
        _camRight = playerCamera.transform.right;
        _camForward.y = 0;
        _camRight.y = 0;        // Y direction is set to 0, so no upward movement
        _camForward.Normalize();
        _moveInput.Normalize();
        
        _moveDirection = _camRight * _moveInput.x + _camForward * _moveInput.y;  // .x = horizontal input, .y = vertical input

        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            _targetRotation = Quaternion.LookRotation(_moveDirection); 
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);    
        }
        
        // calculate gravity
        _velocity = Vector3.up * _velocity.y + _moveDirection * moveSpeed;
        _velocity.y += gravity;

    }

    private bool isGrounded()
    {
        return Physics.SphereCast(
            transform.position + groundCheckOffset,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.mediumPurple;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawWireCube(transform.position + groundCheckOffset + (Vector3.down * groundCheckDistance) / 2,
            new Vector3(2 * groundCheckDistance, groundCheckRadius, 2 * groundCheckRadius));
    }
}
