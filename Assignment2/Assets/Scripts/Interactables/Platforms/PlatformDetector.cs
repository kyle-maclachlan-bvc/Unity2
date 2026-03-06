using Unity.Cinemachine;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    
    [SerializeField] private PlayerController playerController;
    
    private Transform _currentParent;
    //private float _tempGravity;
    
    void OnTriggerEnter(Collider other)
    {
        
        
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<PlayerController>().gravity = _tempGravity;
            
            _currentParent = other.transform.parent;
            other.transform.SetParent(transform);
        }
    }
    
    

    void OnTriggerExit(Collider other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<PlayerController>().gravity = -9.8f;
            other.transform.SetParent(_currentParent);
        }
    }
    
}
