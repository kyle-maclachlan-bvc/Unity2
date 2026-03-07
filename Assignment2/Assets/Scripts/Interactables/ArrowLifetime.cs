using UnityEngine;

public class ArrowLifetime : MonoBehaviour
{
    [SerializeField] private GameObject popEffect;
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Balloon"))
        {
            Instantiate(popEffect, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);  // Pop the balloon
        }
    }
}
