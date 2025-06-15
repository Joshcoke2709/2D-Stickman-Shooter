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
    private bool isSliding = false;
    private bool isDucking = false;
    private float slideTimer;
    private float slideDuration = 0.4f;
    public float slideSpeed = 8f; // Speed during sliding (ensures that the player moves while sliding)
    private Vector3 defaultScale = new Vector3(1, 1, 1); // Default scale for the player visual 
    //private Vector3 duckingScale = new Vector3(1, 0.5f, 1);
    private Vector3 defaultOffset = new Vector3(0, 0f, 0);
    //private Vector3 duckingOffset = new Vector3(0, -0.25f, 0); // Offset when ducking

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

        bool movePressed = Input.GetAxisRaw("Horizontal") != 0;
        bool downPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool downPressedThisFrame = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool upPressed = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool isDucking = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (downPressedThisFrame && movePressed && isGrounded && !isSliding)
        {
            isSliding = true;
            animator.SetBool("isSliding", true);
            slideTimer = slideDuration;
            rb.linearVelocity = new Vector2(move * slideSpeed, rb.linearVelocity.y);
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                isDucking = true;
                animator.SetBool("isSliding", false);
                animator.SetBool("isDucking", true);
                rb.linearVelocity = Vector2.zero; // Stop sliding 

            }
        }

        if (upPressed && isDucking) 
        {
            isDucking = false;
            animator.SetBool("isDucking", false); 
        }

        
        animator.SetBool("isDucking", isDucking);
        animator.SetBool("isWalking", move != 0 && isGrounded);
        animator.SetBool("isJumping", !isGrounded);
        //visualTransform.localScale= targetScale;
        //visualTransform.localPosition = targetOffset;
    }
}
