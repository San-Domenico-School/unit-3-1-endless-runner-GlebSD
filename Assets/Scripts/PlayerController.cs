using UnityEngine;
using UnityEngine.InputSystem;

/***************************  
 *  Gleb
 * Player Controller v2
 * Handles player inputs and moves a wall object.
 ****************************/

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;          // Movement speed
    [SerializeField] private GameObject wall;
    

    private Rigidbody playerRb;
    private Animator animator;
    private float jumpForce = 900f;                     // Jump force
    private Vector3 moveDirection = Vector3.zero;       
    private bool isOnGround = true;                     // Ground check
    public bool gameOver { get; private set; } = false; // Game over state

    void Start()
    {
        // Initialize Rigidbody component
        playerRb = GetComponent<Rigidbody>();

        // Player starts on the ground
        isOnGround = true;

      animator = GetComponent<Animator>();



    }

    private void OnMove(InputValue input)
    {
        Vector2 moveInput = input.Get<Vector2>(); // Get the input vector (X and Y)
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y); // Save the direction
        
    }

    void Update()
    {
        // Move the wall object instead of the player
        if (wall != null && moveDirection != Vector3.zero) // Check if wall exists and there's input
        {
            wall.transform.Translate(moveDirection * speed * Time.deltaTime); // Move the wall
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with the ground
        if (collision.gameObject.name == "Ground")
        {
            isOnGround = true; // Reset isOnGround when touching the ground
            Debug.Log("Player is on the ground.");
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {   
            Debug.Log("Player collided with an obstacle.");
            animator.SetTrigger("Death_b");
        }
    }

    private void OnJump(InputValue input)
    {
        // Player only jumps if on the ground and the game is not over
        if (isOnGround && !gameOver)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            animator.SetTrigger("Jump_trig"); // Trigger jump animation
            isOnGround = false; // Set player as not on the ground
            Debug.Log("Player jumped.");
        }
    }
}
