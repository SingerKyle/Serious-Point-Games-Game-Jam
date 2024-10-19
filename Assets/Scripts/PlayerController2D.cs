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
    [SerializeField] private float tiltAngle = 10f;  // Angle to tilt forward during movement
    [SerializeField] float jumpForce = 500f;         // Jump force applied to player

    [Range(0, .3f)][SerializeField] private float m_movementSmooth = 0.125f;


    // Movement Direction Variables
    float m_horizontalInput;
    float m_verticalInput;
    Vector2 m_moveDirection;
    Vector2 m_currentVelocity = Vector2.zero;
   

    bool m_isSwimming;

    bool m_isGrounded;

    // Physics Body
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_isGrounded = true;
        m_isSwimming = false;

        // Lock the player's rotation to prevent flipping
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        ApplyTilt();  // Apply tilt in direction of movement
    }

    private void FixedUpdate()
    {
        // Use this function for physics calculations Mikey :)
        MovePlayer();

        if (m_isGrounded && !m_isSwimming)
        {
            //change player move speed,dis allow y axis movement, change gravity effect
            rb.gravityScale = 4f; // Default gravity
        }
        else if (m_isSwimming && !m_isGrounded)
        {
            //change player move speed, allow y axis movement, change gravity effect
            rb.gravityScale = waterGravityScale; // No gravity when swimming
        }

    }

    private void PlayerInput()
    {
        // Get input from key presses
        m_horizontalInput = Input.GetAxisRaw("Horizontal");

        if(m_isSwimming)
        {
            m_verticalInput = Input.GetAxisRaw("Vertical");
            Debug.Log(m_verticalInput);
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
        Debug.Log("Jumping!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("We've hit dry captain");
            m_isGrounded = true;
            //m_isSwimming = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    { 
        if(collider.gameObject.CompareTag("Water"))
        {
            Debug.Log("Entering the water");
            //m_isGrounded = false;
            m_isSwimming = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = false;
            Debug.Log("Exiting the ground");
        }

    }

    private void OnTriggerExit2D(Collider2D collider)
    { 
        if(collider.gameObject.CompareTag("Water"))
        {
            m_isSwimming = false;
            Debug.Log("Exiting the water");
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
}
