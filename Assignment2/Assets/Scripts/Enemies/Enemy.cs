using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    private EnemyState _currentState;
    private Transform _currentTarget;
    private bool _isWaiting = false;
    private Vector3 _directionToPlayer;
    
    // used in a different script traditionally, like EnemyManager
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float giveUpDistance;
    [SerializeField] private float chaseAngle;
    
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
}