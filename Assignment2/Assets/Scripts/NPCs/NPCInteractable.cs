using UnityEngine;
using UnityEngine.Serialization;

public class NPCInteractable : NPC, IInteractable
{
    // The interactable behavior for approaching NPC characters.
    [SerializeField] protected string[] dialogueLines;

    protected int waveHash;
    protected int interactHash;

    protected bool _isInteracting;
    
    

    protected override void Start()
    {
        base.Start();       // Keep NavMesh working from NPC.c
        
        waveHash = Animator.StringToHash("Wave");
        interactHash = Animator.StringToHash("Interact");
    }

    protected override void Update()
    {
        // only ander if NOT interacting
        if (!_isInteracting)
        {
            base.Update();
        }
    }
    
    public virtual void OnHoverIn()
    {
        Toast.Instance.ShowToast("Press \"Y\" to Talk");
        anim?.SetBool(waveHash, true);

        _canMove = false;
        
        // stop movement
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public virtual void OnHoverOff()
    {
        Toast.Instance.HideToast();
        anim?.SetBool(waveHash, false);

        _canMove = true;

        if (agent != null)
            agent.isStopped = false;

        EndInteraction();
    }

    public virtual void OnInteract()
    {
        if (_isInteracting)
        {
            EndInteraction();
            return;
        }
        
        _isInteracting = true;
        
        anim?.SetTrigger(interactHash);
        anim?.SetBool(waveHash, false);
        
        string randomLine = dialogueLines[Random.Range(0, dialogueLines.Length)];
        Toast.Instance.ShowToast(randomLine);

        PerformInteraction();
    }

    public void EndInteraction()
    {
        _isInteracting = false;
        
        if (agent != null)
            agent.isStopped = false;
        
        Toast.Instance.HideToast();
    }
}
