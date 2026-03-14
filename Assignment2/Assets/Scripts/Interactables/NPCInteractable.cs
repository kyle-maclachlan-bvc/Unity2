using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator anim;
    [SerializeField] private string dialogue = "Hello, Ever! Take good care of my bow!";

    private int waveHash;
    private int interactHash;

    void Start()
    {
        waveHash = Animator.StringToHash("Wave");
        interactHash = Animator.StringToHash("Interact");
    }

    public void OnHoverIn()
    {
        Toast.Instance.ShowToast("Press \"Y\" to Talk");
        anim.SetBool(waveHash, true);
    }

    public void OnHoverOff()
    {
        Toast.Instance.HideToast();
        anim.SetBool(waveHash, false);

    }

    public void OnInteract()
    {
        Debug.Log("Talking to NPC");

        anim.SetTrigger(interactHash);
        Toast.Instance.ShowToast(dialogue);
        anim.SetBool(waveHash, false);
    }
}
