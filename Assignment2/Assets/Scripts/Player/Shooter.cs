using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class Shooter : MonoBehaviour
{
    
    [SerializeField] private InputAction shootInput;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform aimTrack;        // The object being aimed at
    [SerializeField] private GameObject shootObject;
    [SerializeField] private float shootForce;
    [SerializeField] private float attackDelay = 1f;
    
    public event Action ReadyAttackEvent;
    public event Action AttackEvent;

    private PlayerAnimator _playerAnimator;
    
    private bool _isReadyToAttack = false;
    
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
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (_currentState != PlayerState.AIM) return;
        
        // Calculate direction
        _shootDirection = aimTrack.position - shootPoint.position;
        _shootDirection.Normalize();
        
        if (!_isReadyToAttack)
        {
            // First press - Ready's attack
            ReadyAttackEvent?.Invoke();
            _isReadyToAttack = true;
        }
        else
        {
            // Second press - Fires Weapon
            AttackEvent?.Invoke();
            StartCoroutine(FireArrowDelayed());
            _isReadyToAttack = false;
        }
        
    }

    private IEnumerator FireArrowDelayed()
    {
        yield return new WaitForSecondsRealtime(attackDelay);   // Accounts for pausing mid-shot
        FireArrow();
        
        _playerAnimator.ResetAttack();
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
