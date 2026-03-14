using Unity.Cinemachine;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    
    [SerializeField] private PlayerController playerController;
    
    private Transform _playerOriginalParent;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerOriginalParent = other.transform.parent;
            other.transform.SetParent(transform);
        }
    }
    
    

    void OnTriggerExit(Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(_playerOriginalParent);
        }
    }
    
}
