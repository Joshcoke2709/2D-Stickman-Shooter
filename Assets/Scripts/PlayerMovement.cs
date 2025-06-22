using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform visualTransform;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    private float slideTimer;
    private float slideDuration = 0.2f; // ← How long the slide lasts
    public float slideSpeed = 6f; // Speed during sliding (ensures that the player moves while sliding)

    private float duckTimer; // ← NEW: time spent ducking
    private float duckDuration = 2f; // ← NEW: how long player stays ducked

    private Vector3 defaultScale = new Vector3(1, 1, 1); // Default scale for the player visual 
    private Vector3 defaultOffset = new Vector3(0, 0f, 0);

    private float walkTime;
    public float walkThreshold = 0.1f; // Time to wait before walking again

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // Looks for Animator on Visual child
        visualTransform = transform.Find("Visual");
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        bool movePressed = move != 0;
        bool downPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool downPressedThisFrame = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool isHoldingDown = downPressed;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("isWalking", move != 0 && isGrounded && !animator.GetBool("isSliding") && !animator.GetBool("isDucking"));

        // ← FIX: Restore horizontal movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move > 0)
            visualTransform.localScale = new Vector3(1, 1, 1); // Facing right
        else if (move < 0)
            visualTransform.localScale = new Vector3(-1, 1, 1); // Facing left 

        if (move != 0)
        {
            walkTime += Time.deltaTime;
        }
        else
        {
            walkTime = 0f; // Reset walk time if not moving
        }

        if (isGrounded)
        {
            if (move == 0)
                rb.linearDamping = 5f; // Apply drag only when grounded & not moving
            else
                rb.linearDamping = 0f;
        }
        else
        {
            rb.linearDamping = 0f; // Always 0 in the air 
        }

        // ← JUMP logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // ← FIX: Adjust animation speed based on jump movement
            float jumpAnimSpeed = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
            animator.speed = Mathf.Lerp(1.3f, 0.7f, jumpAnimSpeed); // Faster movement = slower animation
        }

        // ← FIX: Reset animation speed when grounded
        if (isGrounded && animator.speed != 1f)
        {
            animator.speed = 1f;
        }

        // ← SLIDING begins when moving + down pressed
        if (downPressedThisFrame && movePressed && isGrounded && !animator.GetBool("isSliding"))
        {
            animator.SetBool("isSliding", true);
            animator.SetBool("isDucking", false); // just in case
            slideTimer = slideDuration;
            rb.linearVelocity = new Vector2(move * slideSpeed, rb.linearVelocity.y);
        }

        // ← SLIDING logic countdown
        if (animator.GetBool("isSliding"))
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                animator.SetBool("isSliding", false);
                animator.SetBool("isDucking", true); // Transition to duck after slide
                duckTimer = duckDuration; // ← NEW: start duck timer
                rb.linearVelocity = Vector2.zero; // Stop sliding
            }
        }

        // ← STAND UP from ducking
        if (upPressed && animator.GetBool("isDucking"))
        {
            animator.SetBool("isDucking", false);
        }

        // ← Set ducking if holding down + NOT sliding + NOT moving
        if (isHoldingDown && move == 0 && isGrounded && !animator.GetBool("isSliding"))
        {
            if (!animator.GetBool("isDucking"))
            {
                animator.SetBool("isDucking", true);
                duckTimer = duckDuration; // ← NEW: reset duck timer
            }
        }

        // ← FIX: Auto exit duck after 2s or if key released
        if (animator.GetBool("isDucking"))
        {
            duckTimer -= Time.deltaTime;

            if (!isHoldingDown || duckTimer <= 0f || upPressed)
            {
                animator.SetBool("isDucking", false);
            }
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("jumpTakeoff");
        }
        //animator.SetBool("isWalking", move != 0 && isGrounded && !animator.GetBool("isSliding") && !animator.GetBool("isDucking"));
        animator.SetBool("isJumping", !isGrounded);
    }
}
