using UnityEngine;
using DG.Tweening;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator anim;

    private int isOpenHash;
    private Tween _loopTween;
    private Tween _collectTween;
    
    void Start()
    {
        if (anim == null) return;
        isOpenHash = Animator.StringToHash("IsOpen");
        
        
    }

    public void OnHoverIn()
    {
        Debug.Log("Interactor In!");
        anim?.SetBool(isOpenHash, true);
        
        _loopTween = transform.DOScale(1.2f, .5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        
        Toast.Instance.ShowToast("Press \"Y\" to Interact");
        
    }

    public void OnHoverOff()
    {
        Debug.Log("Interactor Out!");
        anim?.SetBool(isOpenHash, false);
        
        _loopTween.Kill();
        
        Toast.Instance.HideToast();
    }

    public void OnInteract()
    {
        Debug.Log($"interacted with {gameObject.name}");
        
        AudioManager.Instance.PlayTreasurePickup();

        _collectTween = transform.DOScale(0, .5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        
        Toast.Instance.HideToast();
    }

    void OnDestroy()
    {
        DOTween.Kill(this.gameObject);
    }
}
