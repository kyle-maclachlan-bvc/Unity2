using UnityEngine;

public class ArrowLifetime : MonoBehaviour
{
    [SerializeField] private GameObject popEffect;      // When the balloon paps, this pops out

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Invoke(nameof(DestroyAfter), 5f);
    }

    void FixedUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            transform.forward = _rb.linearVelocity.normalized;
        }
    }

    void DestroyAfter()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {

        IArrowInteractable interactable = collision.gameObject.GetComponent<IArrowInteractable>();

        if (interactable != null)
            interactable.OnArrowHit();
    }
}
