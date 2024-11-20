using UnityEngine;
using UnityEngine.InputSystem;

/***************************  
 *  Gleb
 * Player Controller v3
 * Handles player inputs, jumping, and animations.
 ****************************/

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;          // Movement speed
    [SerializeField] private GameObject wall;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem runEffect;
    [SerializeField] private AudioClip JumpSound;
    [SerializeField] private AudioClip Deathsound;

    private Rigidbody playerRb;
    private Animator animator;
    private AudioSource audioSource;                    // AudioSource for playing sounds
    private float jumpForce = 900f;                     // Jump force
    private Vector3 moveDirection = Vector3.zero;
    private bool isOnGround = true;                     // Ground check
    private bool dead = false;
    public bool gameOver { get; private set; } = false; // Game over state

    void Start()
    {
        // Initialize Rigidbody, Animator, and AudioSource components
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Player starts on the ground
        isOnGround = true;
    }

    void Update()
    {
        // Play run effect only when on the ground, not dead, and moving
        if (isOnGround && !dead && moveDirection != Vector3.zero)
        {
            if (!runEffect.isPlaying)
                runEffect.Play();
        }
        else
        {
            runEffect.Stop();
        }

        // Idle animation when standing still
        if (moveDirection == Vector3.zero && !dead)
        {
            animator.SetFloat("Speed_f", 0.0f); // Set idle animation
        }

        // Handle movement and animations
        if (!dead)
        {
            if (wall != null && moveDirection != Vector3.zero)
            {
                wall.transform.Translate(moveDirection * speed * Time.deltaTime); // Move the wall
                animator.SetFloat("Speed_f", Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1.0f : 0.4f); // Adjust animation speed
            }

            // Adjust speed for running or walking
            speed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 7f : 3f;
        }
        else
        {
            animator.SetFloat("Speed_f", 0.0f); // Stop animations or movement
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true; // Reset isOnGround when touching the ground
            Debug.Log("Player is on the ground.");
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player collided with an obstacle.");
            animator.SetTrigger("Death_b");

            if (deathEffect != null)
                deathEffect.Play(); // Play death effect
            runEffect.Stop();

            // Play death sound once
            if (!audioSource.isPlaying && Deathsound != null)
                audioSource.PlayOneShot(Deathsound);

            dead = true;
            jumpForce = 0f;
            speed = 0f;
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

            // Play jump sound once
            if (JumpSound != null)
                audioSource.PlayOneShot(JumpSound);

            Debug.Log("Player jumped.");
        }
    }

    private void OnMove(InputValue input)
    {
        Vector2 moveInput = input.Get<Vector2>(); // Get the input vector (X and Y)
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y); // Save the direction
    }
}
