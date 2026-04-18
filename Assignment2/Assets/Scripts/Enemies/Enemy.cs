using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour, IArrowInteractable, IDamageable
{
    private EnemyState _currentState;
    private Transform _currentTarget;
    private bool _isWaiting = false;
    private Vector3 _directionToPlayer;
    
    // used in a different script traditionally, like EnemyManager
    [Header("AI")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float giveUpDistance;
    [SerializeField] private float chaseAngle;

    [Header("Combat")]
    [SerializeField] private int enemyHealth = 1;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private bool _canAttack = true;
    
    //Animator
    [SerializeField] private Animator enemyAnim;
    
    
    void Start()
    {
        // This is traditionally put into a unique script, like EnemyManager
        _currentState = EnemyState.IDLE;
    }

    void FixedUpdate()
    {
        if (_currentState == EnemyState.IDLE)
        {
            enemyAnim.SetBool("idle", true);
            
            if (!_isWaiting)
                StartCoroutine(WaitAndChooseRandomPointAndMove(5));

            if (IsPlayerInRange() && IsPlayerInFOV())
            {
                _currentState = EnemyState.CHASE;
                enemyAnim.SetBool("idle", false);
            }

        }
        else if (_currentState == EnemyState.PATROL)
        {

            enemyAnim.SetBool("patrol", true);
            if (agent.remainingDistance <= .2f)
            {
                _currentState = EnemyState.IDLE;
                enemyAnim.SetBool("patrol", false);
            }

            if (IsPlayerInRange() && IsPlayerInFOV())
            {
                _currentState = EnemyState.CHASE;
                enemyAnim.SetBool("patrol", false); 
            }
    }
        else if (_currentState == EnemyState.CHASE)
        {
            enemyAnim.SetBool("chase", true);
            
            agent.SetDestination(playerTransform.position);

            if (PlayerHasEscaped())
            {
                _currentState = EnemyState.IDLE;
                enemyAnim.SetBool("chase", false);
            }
            
        }
    }

    private IEnumerator WaitAndChooseRandomPointAndMove(float timeToWait)
    {
        _isWaiting = true;
        yield return new WaitForSeconds(timeToWait);
        _currentState = EnemyState.PATROL;
        enemyAnim.SetBool("idle", false);
        ChooseRandomPointAndMove();
        _isWaiting = false;
    }
    
    private void ChooseRandomPointAndMove()
    {
        if (patrolPoints.Length <= 0) return;
        _currentTarget = patrolPoints[Random.Range(0, patrolPoints.Length)];
        
        agent.SetDestination(_currentTarget.position);
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= chaseDistance;
    }
    
    private bool IsPlayerInFOV()
    {
        _directionToPlayer = (playerTransform.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, _directionToPlayer) <= chaseAngle;
    }

    private bool PlayerHasEscaped()
    {
        return Vector3.Distance(transform.position, playerTransform.position) >= giveUpDistance;
    }

    public void OnArrowHit()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        enemyHealth -= amount;

        if (enemyHealth <= 0)
        {
            Die();
        }
                
    }

    private void Die()
    {
        AudioManager.Instance.PlayBalloonPop();
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Enemy touching {other.name}");
        
        if (!_canAttack) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;
    }

    
}