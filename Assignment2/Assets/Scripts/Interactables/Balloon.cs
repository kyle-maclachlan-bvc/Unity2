using UnityEngine;

public class Balloon : MonoBehaviour, IArrowInteractable
{

    [SerializeField] private GameObject popEffect;

    public void OnArrowHit()
    {
        AudioManager.Instance.PlayBalloonPop();
        Instantiate(popEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
