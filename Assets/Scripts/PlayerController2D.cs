using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// -------------------------------- Mechanics--------------------------------------
// Oxygen Meter - remove and add over time
// swim bool
// 
//
// --------------------------------------------------------------------------------

public class PlayerController : MonoBehaviour
{
    // Variables for editor adjustment
    //SerializeField] float m_jumpForce = 400f;
    [SerializeField] float m_movementSpeed = 20f;
    [SerializeField] float m_swimmingSpeed = 20f;
    [SerializeField] float waterGravityScale = 1;
    [SerializeField] float gravityScale = 9;
    [SerializeField] private float tiltAngle = 5f;  // Angle to tilt forward during movement
    [SerializeField] float jumpForce = 500f;         // Jump force applied to player


    [SerializeField] public float waterDragFactor = 0.2f;
    [SerializeField] public float buoyancyForce = 0.1f;
    [SerializeField] public float impulseFactor = 0.1f;

    // Reference to Oxygen System
    [SerializeField] private OxygenSystem oxygenSystem;

    [Range(0, .3f)][SerializeField] private float m_movementSmooth = 0.125f;


    // Movement Direction Variables
    private Vector2 inputDirection = Vector2.zero;
    float m_horizontalInput;
    float m_verticalInput;
    Vector2 m_moveDirection;
    Vector2 m_currentVelocity = Vector2.zero;

    // Audio clips for different player actions
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip swimSound;
    [SerializeField] private AudioClip drownSound;
    [SerializeField] private AudioClip BackgroundAmbience;

    // Animator reference
    private Animator animator;

    bool m_isSwimming;

    bool m_isGrounded;

    // Physics Body
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // Check and assign the Animator
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on the player object!");  // Error message if the Animator is not found
        }
        else
        {
            //Debug.Log("Animator found and assigned successfully.");
        }

        GameManager.instance.PlaySFX(BackgroundAmbience);

        rb = GetComponent<Rigidbody2D>();
        m_isGrounded = true;
        m_isSwimming = false;

        // Lock the player's rotation to prevent flipping
        rb.freezeRotation = true;

        if (oxygenSystem == null)
        {
            oxygenSystem = GetComponent<OxygenSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = inputDirection.normalized;
        PlayerInput();
        ApplyTilt();  // Apply tilt in direction of movement

        if (m_isSwimming)
        {
            oxygenSystem.DrainOxygen(); // Drain oxygen while swimming
        }
        else
        {
            oxygenSystem.RefillOxygen(); // Refill oxygen when out of water
        }
    

        // Check if oxygen is zero (placeholder for death)
        if (oxygenSystem.GetCurrentOxygen() <= 0)
        {
            //Debug.Log("Player has drowned!");
            RespawnPlayer();  // Respawn player at (0, 0) when drowned
            // Placeholder for future death mechanic
        }
        // Update animator parameters
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(m_horizontalInput));  // Line 101
        }
        else
        {
            //Debug.LogError("Animator is null!");
        }

        if (m_horizontalInput < 0)
        {
            if(m_isSwimming)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }
            
        }
        else if (m_horizontalInput > 0)
        {
            if (m_isSwimming)
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }
            else
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            
        }
    }


    private void FixedUpdate()
    {
        // Use this function for physics calculations Mikey :)
        MovePlayer();

        if (m_isGrounded && !m_isSwimming)
        {
            //change player move speed,dis allow y axis movement, change gravity effect
            rb.gravityScale = gravityScale; // Default gravity
        }
        else if (m_isSwimming && !m_isGrounded)
        {
            ApplyUnderwaterMovement(m_moveDirection);
            //change player move speed, allow y axis movement, change gravity effect
            rb.gravityScale = waterGravityScale; // No gravity when swimming
        }
        //Debug.Log(oxygenSystem.GetCurrentOxygen());

    }

    private void PlayerInput()
    {
        // Get input from key presses
        m_horizontalInput = Input.GetAxisRaw("Horizontal");

        if(m_isSwimming)
        {
            m_verticalInput = Input.GetAxisRaw("Vertical");
            //Debug.Log(m_verticalInput);
        }
        else
        {
            m_verticalInput= 0f;
        }
        
        // Calculate Movement Direction
        m_moveDirection = new Vector2(m_horizontalInput, m_verticalInput).normalized;

        if (m_isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        //Debug.Log($"Horizontal Input: {m_horizontalInput}");
    }

    public void HandleDirectionalInput(Vector2 direction)
    {
        inputDirection = direction;
    }

    void MovePlayer()
    {
        
        // use different speed when the player is in the water versus out of it
        float currentSpeed = m_isSwimming ? m_swimmingSpeed : m_movementSpeed;

        // Calculate the target velocity
        Vector2 targetVelocity = m_moveDirection * currentSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref m_currentVelocity, m_movementSmooth);
        
    }

    private void Jump()
    {
        // Apply upward force to jump
        rb.AddForce(new Vector2(0f, jumpForce));
        //Debug.Log("Jumping!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("We've hit dry captain");
            m_isGrounded = true;
            //m_isSwimming = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    { 
        if(collider.gameObject.CompareTag("Water"))
        {
            //Debug.Log("Entering the water");
            //m_isGrounded = false;
            m_isSwimming = true;
            animator.SetBool("isSwimming", m_isSwimming);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = false;
            //Debug.Log("Exiting the ground");
        }

    }

    private void OnTriggerExit2D(Collider2D collider)
    { 
        if(collider.gameObject.CompareTag("Water"))
        {
            m_isSwimming = false;
            animator.SetBool("isSwimming", m_isSwimming);
            //Debug.Log("Exiting the water");
        }
    }
    private void ApplyTilt()
    {
        // Apply a forward lean based on horizontal movement direction
        if (m_horizontalInput != 0)
        {
            // Calculate the tilt angle based on direction of movement
            float tilt = -m_horizontalInput * tiltAngle;

            // Create a new rotation keeping the player upright but tilted slightly forward or backward
            transform.rotation = Quaternion.Euler(0f, 0f, tilt);
        }
        else
        {
            // Reset rotation when there is no movement
            transform.rotation = Quaternion.identity;
        }
    }
    private void DrainOxygen()
    {
        // Drain oxygen while in water
        oxygenSystem.DrainOxygen();  // Call DrainOxygen without arguments
    }

    private void RefillOxygen()
    {
        // Refill oxygen when out of water
        oxygenSystem.RefillOxygen();
    }
    private void ApplyUnderwaterMovement(Vector2 direction)
    {
        // Apply force based on direction with some impulse to simulate water resistance
        Vector2 movementForce = direction * m_currentVelocity * (0.01f + impulseFactor);
        movementForce.y += buoyancyForce;

        rb.AddForce(movementForce, ForceMode2D.Force);
    }

    private void RespawnPlayer()
    {
        // Log respawn action
        //Debug.Log("Player has drowned. Respawning...");

        // Reset player's position to (0, 0)
        transform.position = new Vector2(0, 0);

        // Optional: Reset the oxygen to full after respawn
        oxygenSystem.Start(); // Reinitialize oxygen
    }
}
