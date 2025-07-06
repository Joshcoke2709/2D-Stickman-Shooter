using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 9f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] private Transform visualTransform;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isFacingRight = true;

    private float slideTimer;
    private float slideDuration = 0.2f;
    public float slideSpeed = 6f;

    private float duckTimer;
    private float duckDuration = 2f;

    private float walkTime;
    public float walkThreshold = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        visualTransform = transform.Find("Visual");
    }

    void Update()
    {
        //float move = Input.GetAxisRaw("Horizontal"); // A/D or Left/R
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        else if (Input.GetKey(KeyCode.D)) move = 1f;
        bool movePressed = move != 0;

        bool downPressed = Input.GetKey(KeyCode.S);
        bool downPressedThisFrame = Input.GetKeyDown(KeyCode.S);
        bool upPressed = Input.GetKeyDown(KeyCode.W);
        bool isHoldingDown = downPressed;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        animator.SetBool("isWalking", move != 0 && isGrounded && !animator.GetBool("isSliding") && !animator.GetBool("isDucking"));

        // Movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move > 0)
            visualTransform.localScale = new Vector3(1, 1, 1);
        else if (move < 0)
            visualTransform.localScale = new Vector3(-1, 1, 1);

        walkTime = (move != 0) ? walkTime + Time.deltaTime : 0f;

        // Damping
        rb.linearDamping = (isGrounded && move == 0) ? 5f : 0f;

        // Jumping (using W)
        if (upPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("jumpTakeoff");

            float jumpAnimSpeed = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
            animator.speed = Mathf.Lerp(1.3f, 0.7f, jumpAnimSpeed);
        }

        if (isGrounded && animator.speed != 1f)
        {
            animator.speed = 1f;
        }

        // Sliding
        if (downPressedThisFrame && movePressed && isGrounded && !animator.GetBool("isSliding"))
        {
            animator.SetBool("isSliding", true);
            animator.SetBool("isDucking", false);
            slideTimer = slideDuration;
            rb.linearVelocity = new Vector2(move * slideSpeed, rb.linearVelocity.y);
        }

        if (animator.GetBool("isSliding"))
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                animator.SetBool("isSliding", false);
                animator.SetBool("isDucking", true);
                duckTimer = duckDuration;
                rb.linearVelocity = Vector2.zero;
            }
        }

        // Ducking
        if (upPressed && animator.GetBool("isDucking"))
        {
            animator.SetBool("isDucking", false);
        }

        if (isHoldingDown && move == 0 && isGrounded && !animator.GetBool("isSliding"))
        {
            if (!animator.GetBool("isDucking"))
            {
                animator.SetBool("isDucking", true);
                duckTimer = duckDuration;
            }
        }

        if (animator.GetBool("isDucking"))
        {
            duckTimer -= Time.deltaTime;
            if (!isHoldingDown || duckTimer <= 0f || upPressed)
            {
                animator.SetBool("isDucking", false);
            }
        }

        animator.SetBool("isJumping", !isGrounded);

        // Update facing direction
        if (move != 0)
        {
            Vector3 visualScale = visualTransform.localScale;
            visualScale.x = Mathf.Sign(move);
            Mathf.Abs(visualScale.x); // Ensure scale is always positive
            visualTransform.localScale = visualScale;

            isFacingRight = move > 0;
        }
    }
}
