using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerController playerController; // Finds the playerController for Movement Methods
    [SerializeField] private Animator anim;                     // Finds the Animator to collect Animator Parameters

    [Header("AttackingAnimation")]
    [SerializeField] private Shooter shooter;                   // Finds the Shooter for Attacking Methods

    [Header("CheeringAnimation")]
    [SerializeField] private int clearedLevelHash;              // Create Hash to play the Cleared Level Cheer Animation
    
    private Vector3 _playerVelocity;                            // Reads Player Velocity for Animation Parameters


    void Awake()
    {
        // Sets up Hash in Code
        clearedLevelHash = Animator.StringToHash("ClearedLevel"); 
    }
    
    void Update()
    {
        // Sets up, and reads, Bool every frame to check if player can jump
        anim.SetBool("IsGrounded", playerController.IsGrounded());

        // Checks and sets Velocity for the animator parameters to play
        _playerVelocity = playerController.GetPlayerVelocity();
        _playerVelocity.y = 0;
        
        // Sets up, and reads, Float every frame to check if the player has velocity
        anim.SetFloat("Velocity", _playerVelocity.sqrMagnitude);
    }

    void OnEnable()
    {
        // Enables Methods based on actions from the PlayerController and Shooter
        playerController.OnJumpEvent += OnJump;
        shooter.ReadyAttackEvent += PrepAttack;
        shooter.AttackEvent += DoAttack;
    }

    void OnDisable()
    {
        // Disables Methods based on actions from the PlayerController and Shooter
        playerController.OnJumpEvent -= OnJump;
        shooter.ReadyAttackEvent -= PrepAttack;
        shooter.AttackEvent -= DoAttack;
    }

    private void OnJump()
    {
        // Activates the Jump Parameter to play the Jump Animation loop
        //Debug.Log("Jump Animation");
        anim.SetTrigger("Jump");
    }

    private void PrepAttack()
    {
        // Activates the Ready Attack Animation, setting the Bow in front of the player
        // TODO: Perhaps change this to a layer? so player running goes as well as Bow in front
        //Debug.Log("Ready Attack Event Triggered");
        anim.SetBool("IsAiming", true);
    }

    private void DoAttack()
    {
        // Activates the Attack Firing Animation loop
        //Debug.Log("Attack Event Triggered");
        anim.SetTrigger("IsAttacking");
    }

    public void ResetAttack()
    {
        // Prevents the Attack Loop from looping after arrow is fired. Also allows animation to go back to "Idle"
        anim.SetBool("IsAiming", false);
        
    }

    public void PlayCheer()
    {
        // Activates the Cheer Animation at the end of the level
        //Debug.Log("CHEER TRIGGERED!");
        anim.SetTrigger(clearedLevelHash);
    }
}
