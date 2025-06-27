using UnityEngine;
using UnityEngine.UIElements;

public class Player2Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpoForce = 4f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] private Transform visualTransform;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isFacingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        visualTransform = transform.Find("Visual");
    }

    private void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");// used for animations

        //arrow keys for player2 movement 
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

        bool isJumpPressed = Input.GetKey(KeyCode.UpArrow);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y); // mnovement controls

        //animation controls
        animator.SetBool("isWalking", horizontal != 0 && isGrounded);
        animator.SetBool("isJumping", !isGrounded);

        //flp the direction for visual 
        if (horizontal > 0)
        {
            visualTransform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
        else if (horizontal < 0)
        {
            visualTransform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }

        //jump
        if (isJumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpoForce);
            animator.SetTrigger("jumpTakeoff");
        }


    }
}