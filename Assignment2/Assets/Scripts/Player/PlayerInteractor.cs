using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    // Input
    [SerializeField] private InputAction interactionInput;

    private IInteractable _interactable;

    void OnEnable()
    {
        interactionInput.Enable();
        interactionInput.performed += Interact;
    }

    void OnDisable()
    {
        interactionInput.performed -= Interact;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        _interactable = other.GetComponent<IInteractable>();

        if (_interactable != null)
        {
            _interactable?.OnHoverIn(); 
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        _interactable?.OnHoverOff();
        _interactable = null;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        _interactable?.OnInteract();    // checks if _interactable is null.
    }
}
