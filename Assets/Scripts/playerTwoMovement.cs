using UnityEngine;

public class Player2Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 9f;
    public float slideSpeed = 6f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] private Transform visualTransform;

    private Rigidbody2D rb;
    public Animator animator;
    public Collider2D hitbox;
    private bool isGrounded;
    private bool isFacingRight = true;

    private float slideTimer;
    private float slideDuration = 0.2f;

    private float duckTimer;
    private float duckDuration = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        visualTransform = transform.Find("VisualP2");
    }

    private void Update()
    {
        // Movement input for arrow keys
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

        bool downPressed = Input.GetKey(KeyCode.DownArrow);
        bool downPressedThisFrame = Input.GetKeyDown(KeyCode.DownArrow);
        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow);
        bool isJumpPressed = Input.GetKey(KeyCode.UpArrow);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);

        // Movement
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("jumpTakeoff");
        }

        // Walking animation
        animator.SetBool("isWalking", horizontal != 0 && isGrounded && !animator.GetBool("isSliding") && !animator.GetBool("isDucking"));
        animator.SetBool("isJumping", !isGrounded);

        // Flip sprite
          if (horizontal != 0)
          {
              Vector3 scale = visualTransform.localScale;
              scale.x = horizontal > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
              visualTransform.localScale = scale;
              isFacingRight = horizontal > 0;
          }

        // Slide
        if (downPressedThisFrame && horizontal != 0 && isGrounded && !animator.GetBool("isSliding"))
        {
            animator.SetBool("isSliding", true);
            animator.SetBool("isDucking", false);
            slideTimer = slideDuration;
            rb.linearVelocity = new Vector2(horizontal * slideSpeed, rb.linearVelocity.y);
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

        // Duck
        if (downPressed && horizontal == 0 && isGrounded && !animator.GetBool("isSliding"))
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
            if (!downPressed || duckTimer <= 0f || upPressed)
            {
                animator.SetBool("isDucking", false);
            }
        }
    }

    public void ResetState()
    {
        // 1. Snap the animator back to your idle state
        animator.Play("Idle");              // replace "Idle" with your exact state name

        // 2. Reset any motion
        if (rb != null)
            rb.velocity = Vector2.zero;

        // 3. If you’ve disabled colliders or controls on death, re-enable them:
        var coll = GetComponent<Collider2D>();
        if (coll != null)
            coll.enabled = true;

        // 4. Reset any flags you use for jumping, dashing, etc.
        //    e.g. isGrounded = true; canDash = true; etc.
    }
}
