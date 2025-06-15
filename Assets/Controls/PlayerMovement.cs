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

    private float walkTime;
    public float walkThreshold = 0.1f; // Time to wait before walking again

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // Looks for Animator on Visual child
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // ← FIX: Restore horizontal movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move > 0 )
            visualTransform.localScale = new Vector3(1, 1, 1); // Facing right
        else if (move < 0)
            visualTransform.localScale = new Vector3(-1, 1, 1);  //Facing left 

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
                rb.linearDamping = 5f;  // Apply drag only when grounded & not moving
            else
                rb.linearDamping = 0f;
        }
        else
        {
            rb.linearDamping = 0f;  // Always 0 in the air
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        animator.SetBool("isWalking", move != 0 && isGrounded);
        animator.SetBool("isJumping", !isGrounded);
    }
}
