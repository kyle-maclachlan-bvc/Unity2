using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private GameObject levelClear;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered End Level State");
            GameManager.Instance.Win();
        }
    }
}
