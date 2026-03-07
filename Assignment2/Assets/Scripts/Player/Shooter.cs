using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class Shooter : MonoBehaviour
{
    [SerializeField] private InputAction shootInput;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject shootObject;
    [SerializeField] private float shootForce;
    
    public event Action ReadyAttackEvent;
    public event Action AttackEvent;

    private bool isReadyToAttack = false;
    
    private GameObject _arrow;

    void OnEnable()
    {
        shootInput.Enable();
        shootInput.performed += Shoot;
    }

    void OnDisable()
    {
        shootInput.performed -= Shoot;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!isReadyToAttack)
        {
            // First press - Ready's attack
            ReadyAttackEvent?.Invoke();
            isReadyToAttack = true;
        }
        else
        {
            // Second press - Fires Weapon
            AttackEvent?.Invoke();
            isReadyToAttack = false;
        }
        
    }

    public void FireArrow()
    {
        // create a new arrow
        _arrow = Instantiate(shootObject, shootPoint.position, shootPoint.rotation);
                
        // apply a force
        _arrow.GetComponent<Rigidbody>().AddForce(shootForce * shootPoint.forward);
    }
}
