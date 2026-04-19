using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class Shooter : MonoBehaviour
{
    [Header("Shooter Settings")]                        // Variables designed to set up the Shooter Methods
    [SerializeField] private InputAction shootInput;    // Button to press to shoot
    [SerializeField] private Transform shootPoint;      // Origin Point of the Arrow
    [SerializeField] private Transform aimTrack;        // The object being aimed at
    [SerializeField] private GameObject shootObject;    // Arrow
    [SerializeField] private float shootForce;          // Force applied to arrow to launch
    [SerializeField] private float attackDelay = 1f;    // Time delayed for Arrow Shot to match "Release Animation".
    
    public event Action ReadyAttackEvent;               // Event to help trigger Ready to Attack Animation
    public event Action AttackEvent;                    // Eventto Help trigger Attack Animation

    private PlayerAnimator _playerAnimator;             // Grab the playerAnimator for the Events to react.
    
    private bool _isReadyToAttack = false;              // Set up Code to register player is ready to attack.
    // TODO: Perhaps change this bool to match AIM mode.
    private bool _isAttacking = false;
    
    private GameObject _arrow;
    private Vector3 _shootDirection;
    private PlayerState _currentState;
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
    
    void OnEnable()
    {
        shootInput.Enable();
        shootInput.performed += Shoot;
        _playerController.OnStateUpdated += StateUpdate;
    }

    void OnDisable()
    {
        shootInput.performed -= Shoot;
        _playerController.OnStateUpdated -= StateUpdate;
    }

    void StateUpdate(PlayerState state)
    {
        _currentState = state;

        if (_currentState == PlayerState.AIM)
        {
            ReadyAttackEvent?.Invoke();
            _isReadyToAttack = true;
        }
        else
        {
            _isReadyToAttack = false;
            _playerAnimator.ResetAttack();
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (_currentState != PlayerState.AIM) return;

        if (_isAttacking) return;
        
        // Calculate direction
        _shootDirection = aimTrack.position - shootPoint.position;
        _shootDirection.Normalize();
        
        // Fire Weapon
        if (!_isReadyToAttack)
        {
            ReadyAttackEvent?.Invoke();
            _isReadyToAttack = true;
        }
        else
        {
            AttackEvent?.Invoke();
            _isAttacking = true;
            StartCoroutine(FireArrowDelayed());
            _isReadyToAttack = false;
        }
        
        
        
    }

    private IEnumerator FireArrowDelayed()
    {
        yield return new WaitForSecondsRealtime(attackDelay);   // Accounts for pausing mid-shot
        FireArrow();
        _playerAnimator.ResetAttack();
        _isAttacking = false;
    }

    public void FireArrow()
    {
        // Add a Coroutine to Arrow for delaying FireArrow to the Release Animation
        AudioManager.Instance.PlayArrowSFX();
        
        // create a new arrow
        _arrow = Instantiate(shootObject, shootPoint.position, Quaternion.LookRotation(_shootDirection));
                
        // apply a force
        _arrow.GetComponent<Rigidbody>().AddForce(shootForce * _shootDirection, ForceMode.Impulse);
    }
}
