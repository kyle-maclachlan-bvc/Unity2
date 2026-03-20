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
        if (collision.gameObject.CompareTag("Balloon"))
        {
            AudioManager.Instance.PlayBalloonPop();
            Instantiate(popEffect, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);  // Pop the balloon, the arrow stays because Arrow is sharp, and does not make sense to destroy arrow.
        }
    }
}
