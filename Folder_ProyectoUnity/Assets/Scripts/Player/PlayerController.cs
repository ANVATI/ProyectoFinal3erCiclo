using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public PlayerAttributes playerAttributes;
    public Transform cameraTransform;
    public PlayerActions playerAction;
    public GameObject Trail;
    
    private Vector2 movementInput;
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private Vector3 standingColliderCenter;
    private float standingColliderHeight;

    private PlayerState playerState;
    private bool canAttack = true;
    private bool isRolling = false;
    private bool isAttacking = false;

    private enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Crouching,
        Rolling,
        Attacking,
        CrouchingAttack,
        Taunting,
        Rage
    }

    void Awake()
    {
        playerAttributes.Stamina = 100f;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        standingColliderCenter = capsuleCollider.center;
        standingColliderHeight = capsuleCollider.height;
        playerState = PlayerState.Idle;
        playerAction = GetComponent<PlayerActions>();
    }

    private void Update()
    {
        //Debug.Log(playerAttributes.Stamina);
    }

    void FixedUpdate()
    {
        HandleMovement();
        UpdateAnimation();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnRunning(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Crouching && playerState != PlayerState.Attacking && !isAttacking)
        {
            if (context.started && (movementInput.x != 0 || movementInput.y != 0) && playerAttributes.Stamina > 0)
            {
                playerState = PlayerState.Running;
            }
            else if (context.canceled)
            {
                playerState = PlayerState.Walking;
            }
        }
    }

    public void OnRolling(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Crouching && playerAttributes.Stamina > 5 && !isAttacking)
        {
            if (context.started && playerState == PlayerState.Running)
            {
                StartCoroutine(Roll());
            }
        }
    }

    public void OnCrouched(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking)
        {
            ToggleCrouch();
        }
    }

    public void OnAttackStand(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack && playerState != PlayerState.Rolling)
        {
            if (playerState == PlayerState.Crouching && playerAttributes.Stamina > 5)
            {
                StartCoroutine(CrouchAttack());
            }
            else if (playerState != PlayerState.Crouching && playerState != PlayerState.Rolling && playerAttributes.Stamina > 5)
            {
                StartCoroutine(Attack());
            }
        }
    }

    public void RageMode(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Crouching && playerAction.GetEnemyKillCount() >= 10 && !isAttacking)
        {
            if (context.performed)
            {
                StartCoroutine(Rage());
                playerAction.TriggerRage();
                playerAttributes.Stamina = 100f;
            }
        }
    }

    public void Taunt(InputAction.CallbackContext context)
    {
        if (playerState == PlayerState.Idle)
        {
            if (context.performed)
            {
                StartCoroutine(Taunt());

            }
        }
    }

    private IEnumerator Roll()
    {
        playerState = PlayerState.Rolling;
        isRolling = true;
        DecreaseStamina(5);
        animator.SetBool("IsRolling", true);
        rb.AddForce(transform.forward * playerAttributes.rollForce, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        animator.SetBool("IsRolling", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
        isRolling = false;
    }
    private IEnumerator Attack()
    {
        isAttacking = true;
        Trail.SetActive(true);
        playerState = PlayerState.Attacking;
        DecreaseStamina(5);
        canAttack = false;
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.95f);

        animator.SetBool("IsAttacking", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
        canAttack = true;
        Trail.SetActive(false);
        isAttacking = false;
    }

    private IEnumerator CrouchAttack()
    {
        isAttacking = true;
        Trail.SetActive(true);
        playerState = PlayerState.CrouchingAttack;
        DecreaseStamina(5);
        canAttack = false;
        animator.SetBool("AttackCrouch", true);

        yield return new WaitForSeconds(0.8f);

        animator.SetBool("AttackCrouch", false);
        playerState = PlayerState.Crouching;
        canAttack = true;
        Trail.SetActive(false);
        isAttacking = false;
    }

    private IEnumerator Rage()
    {

        playerState = PlayerState.Rage;
        animator.SetBool("RageMode", true);

        yield return new WaitForSeconds(2.5f);

        animator.SetBool("RageMode", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
    }

    private IEnumerator Taunt()
    {
        playerState = PlayerState.Taunting;
        animator.SetBool("Taunt", true);

        yield return new WaitForSeconds(2f);

        animator.SetBool("Taunt", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
    }

    private void HandleMovement()
    {
        if (playerState == PlayerState.Attacking || playerState == PlayerState.Rage
            || playerState == PlayerState.Taunting || playerState == PlayerState.Rolling
            || playerState == PlayerState.CrouchingAttack || isRolling)
        {
            return;
        }
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * movementInput.y + right * movementInput.x;

        if (moveDirection != Vector3.zero)
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 0.18f);

            if (playerState == PlayerState.Running)
            {
                if (playerAttributes.Stamina > 0 && !playerAction.inRageMode)
                {
                    DecreaseStamina(0.1f);
                    playerAttributes.currentSpeed += playerAttributes.acceleration * Time.fixedDeltaTime;
                    playerAttributes.currentSpeed = Mathf.Min(playerAttributes.currentSpeed, playerAttributes.maxSpeed); 
                }
                else if (playerAction.inRageMode)
                {
                    playerAttributes.currentSpeed = playerAttributes.runSpeed;
                }
                else
                {
                    playerState = PlayerState.Walking;
                    playerAttributes.currentSpeed = playerAttributes.walkSpeed;
                }
            }
            else if (playerState == PlayerState.Crouching)
            {
                playerAttributes.currentSpeed = playerAttributes.crouchSpeed;
                IncreaseStamina(Time.deltaTime * 5f);
            }
            else
            {
                playerAttributes.currentSpeed = playerAttributes.walkSpeed;
                IncreaseStamina(Time.deltaTime * 2f);
            }

            rb.MovePosition(transform.position + moveDirection * playerAttributes.currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            IncreaseStamina(Time.deltaTime * 5f);

            if (playerState == PlayerState.Walking || playerState == PlayerState.Running)
            {
                playerState = PlayerState.Idle;
            }
        }
    }
    private void UpdateAnimation()
    {
        animator.SetFloat("X", movementInput.x);
        animator.SetFloat("Y", movementInput.y);
        animator.SetBool("IsRunning", playerState == PlayerState.Running);
    }

    private void ToggleCrouch()
    {
        if (playerState == PlayerState.Crouching)
        {
            if (!Physics.Raycast(transform.position + capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2f), Vector3.up, 1f))
            {
                playerState = PlayerState.Idle;
                animator.SetBool("IsCrouched", false);
                capsuleCollider.height = standingColliderHeight;
                capsuleCollider.center = standingColliderCenter;
            }
        }
        else
        {
            playerState = PlayerState.Crouching;
            animator.SetBool("IsCrouched", true);
            capsuleCollider.height = standingColliderHeight / 1.5f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y / 1.5f, capsuleCollider.center.z);
        }
    }
    private void IncreaseStamina(float amount)
    {
        playerAttributes.Stamina = Mathf.Min(playerAttributes.Stamina + amount, 100);
    }
    private void DecreaseStamina(float amount)
    {
        playerAttributes.Stamina = Mathf.Max(playerAttributes.Stamina - amount, 0);
    }
}