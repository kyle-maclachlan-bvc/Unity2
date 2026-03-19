using System;
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
    
    public event Action ReadyAttackEvent;
    public event Action AttackEvent;

    private bool isReadyToAttack = false;
    
    private GameObject _arrow;
    private Vector3 _shootDirection;
    private PlayerState _currentState;
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
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
        
        if (!isReadyToAttack)
        {
            // First press - Ready's attack
            ReadyAttackEvent?.Invoke();
            isReadyToAttack = true;
        }
        else
        {
            // Second press - Fires Weapon
            FireArrow();
            isReadyToAttack = false;
        }
        
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
