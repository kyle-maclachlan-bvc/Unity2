using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator anim;

    [Header("AttackingAnimation")]
    [SerializeField] private Shooter shooter;
    [SerializeField] private float attackTimer;

    [Header("CheeringAnimation")]
    [SerializeField] private int clearedLevelHash;
    
    private Vector3 _playerVelocity;


    void Start()
    {
        clearedLevelHash = Animator.StringToHash("ClearedLevel");
    }
    
    // Update is called once per frame
    void Update()
    {
        anim.SetBool("IsGrounded", playerController.IsGrounded());

        _playerVelocity = playerController.GetPlayerVelocity();
        _playerVelocity.y = 0;
        
        anim.SetFloat("Velocity", _playerVelocity.sqrMagnitude);
    }

    void FixedUpdate()
    {
        
    }

    void OnEnable()
    {
        playerController.OnJumpEvent += OnJump;
        shooter.ReadyAttackEvent += PrepAttack;
        shooter.AttackEvent += DoAttack;

    }

    void OnDisable()
    {
        playerController.OnJumpEvent -= OnJump;
        shooter.ReadyAttackEvent -= PrepAttack;
        shooter.AttackEvent -= DoAttack;
    }

    private void OnJump()
    {
        anim.SetTrigger("Jump");
    }

    private void PrepAttack()
    {
        Debug.Log("Ready Attack Event Triggered");
        anim.SetTrigger("ReadyAttack");
    }

    public void FireArrow()
    {
        shooter.FireArrow();
    }

    private void DoAttack()
    {
        Debug.Log("Second Attack is Firing");
        anim.SetBool("IsAttacking", true);
    }

    public void ResetAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    public void PlayCheer()
    {
        anim.SetTrigger(clearedLevelHash);
    }
}
