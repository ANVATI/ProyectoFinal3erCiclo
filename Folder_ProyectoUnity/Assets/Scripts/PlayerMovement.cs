using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Atributos del jugador")]

    public float walkSpeed = 5.0f;
    public float runSpeed = 10f;
    public float crouchSpeed = 3.0f;
    public float rollForce = 10f;
    public Transform cameraTransform;

    private Vector2 movementInput;
    private Rigidbody rb;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private Vector3 standingColliderCenter;
    private float standingColliderHeight;

    private PlayerState playerState;
    private bool canAttack = true;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        standingColliderCenter = capsuleCollider.center;
        standingColliderHeight = capsuleCollider.height;
        playerState = PlayerState.Idle;
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
        if (playerState != PlayerState.Crouching)
        {
            if (context.started && (movementInput.x != 0 || movementInput.y != 0))
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
        if (playerState != PlayerState.Crouching)
        {
            if (context.started && playerState == PlayerState.Running)
            {
                StartCoroutine(Roll());
            }
        }
    }
    public void OnCrouched(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ToggleCrouch();
        }
    }

    public void OnAttackStand(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            if (playerState == PlayerState.Crouching)
            {
                StartCoroutine(CrouchAttack());
            }
            else if (playerState != PlayerState.Crouching)
            {
                StartCoroutine(Attack());
            }
        }
    }

    public void RageMode(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Crouching)
        {
            if (context.performed)
            {
                StartCoroutine(Rage());
            }
        }
    }


    public void Taunt(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Crouching)
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
        animator.SetBool("IsRolling", true);
        rb.AddForce(transform.forward * rollForce, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        animator.SetBool("IsRolling", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
    }

    private IEnumerator Attack()
    {
        playerState = PlayerState.Attacking;
        canAttack = false;
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.95f);

        animator.SetBool("IsAttacking", false);
        playerState = movementInput != Vector2.zero ? PlayerState.Walking : PlayerState.Idle;
        canAttack = true;
    }

    private IEnumerator CrouchAttack()
    {
        playerState = PlayerState.CrouchingAttack;
        canAttack = false;
        animator.SetBool("AttackCrouch", true);

        yield return new WaitForSeconds(0.8f);

        animator.SetBool("AttackCrouch", false);
        playerState = PlayerState.Crouching;
        canAttack = true;
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
            || playerState == PlayerState.CrouchingAttack)
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

            float currentSpeed = 0f;

            if (playerState == PlayerState.Running)
            {
                currentSpeed = runSpeed;
            }
            else if (playerState == PlayerState.Crouching)
            {
                currentSpeed = crouchSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            rb.MovePosition(transform.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
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
}