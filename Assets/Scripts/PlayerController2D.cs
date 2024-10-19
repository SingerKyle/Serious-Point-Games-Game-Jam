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

        m_isSwimming = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        // Use this function for physics calculations Mikey :)
        MovePlayer();

        if (m_isGrounded)
        {
            
        }

    }

    private void PlayerInput()
    {
        // Get input from key presses
        m_horizontalInput = Input.GetAxisRaw("Horizontal");
        m_verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate Movement Direction
        m_moveDirection = new Vector2(m_horizontalInput, m_verticalInput).normalized;
    }

    void MovePlayer()
    {
        // Calculate the target velocity
        Vector2 targetVelocity = m_moveDirection * m_movementSpeed;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref m_currentVelocity, m_movementSmooth);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = false;
        }
    }
}
