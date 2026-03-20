using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private GameObject levelClear;

    private bool _hasTriggered = false;
    
    public void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Entered End Level State");
            _hasTriggered = true;
            AudioManager.Instance.PlayLevelClear();
            GameManager.Instance.Win();
        }
    }
}
