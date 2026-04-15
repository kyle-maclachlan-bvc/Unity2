using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public abstract class NPC : MonoBehaviour
{
    // Abstract class makes so the script cannot be added as a component.
    [SerializeField] protected float wanderRadius = 5f;
    [SerializeField] protected float wanderDelay = 3f;

    [SerializeField] protected Animator anim;

    protected NavMeshAgent agent;
    protected Vector3 startPosition;
    protected bool _canMove = true;
    private float _wanderTimer;

    protected int speedHash;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        _wanderTimer = wanderDelay;
        speedHash = Animator.StringToHash("Speed");
    }

    protected virtual void Update()
    {
        HandleWander();
        UpdateAnimation();
    }
    
    protected void HandleWander()
    {
        if (agent == null || !_canMove) return;
        
        _wanderTimer += Time.deltaTime;

        if (_wanderTimer >= wanderDelay)
        {
            Vector3 newPos = GetRandomPoint(startPosition, wanderRadius);
            agent.SetDestination(newPos);
            
            _wanderTimer = 0;
        }
    }

    protected Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        
        return hit.position;
    }

    protected void UpdateAnimation()
    {
        if (anim == null || agent == null) return;
        
        float speed = agent.velocity.magnitude;
        anim.SetFloat(speedHash, speed);
    }

    protected virtual void PerformInteraction()
    {
        Debug.Log("You are interacting with me");
    }

    protected virtual void Damage()
    {
        Debug.Log("You have damaged me");
        
    }
    
    
    
}
