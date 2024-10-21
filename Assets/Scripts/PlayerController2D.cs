using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float m_movementSpeed = 20f;
    [SerializeField] float m_swimmingSpeed = 20f;
    [SerializeField] float waterGravityScale = 1;
    [SerializeField] float gravityScale = 9;
    [SerializeField] private float tiltAngle = 5f;
    [SerializeField] float jumpForce = 500f;

    [SerializeField] public float waterDragFactor = 0.2f;
    [SerializeField] public float buoyancyForce = 0.1f;
    [SerializeField] public float impulseFactor = 0.1f;

    [SerializeField] private OxygenSystem oxygenSystem;
    [Range(0, .3f)][SerializeField] private float m_movementSmooth = 0.125f;

    private Vector2 inputDirection = Vector2.zero;
    float m_horizontalInput;
    float m_verticalInput;
    Vector2 m_moveDirection;
    Vector2 m_currentVelocity = Vector2.zero;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip swimSound;
    [SerializeField] private AudioClip drownSound;
    [SerializeField] private List<AudioClip> BackgroundAmbience;

    private Animator animator;

    private AudioSource ambientAudioSource;
    private AudioSource footstepsAudioSource;
    private AudioSource jumpAudioSource;
    private AudioSource swimAudioSource;
    private AudioSource drownAudioSource;

    bool m_isSwimming;
    bool m_isGrounded;
    bool isPlayingFootsteps = false;
    bool isPlayingSwimSound = false;
    bool hasPlayedDrownSound = false;
    bool hasJumped = false;

    [SerializeField] public ParticleSystem bubblePrefab;
    private ParticleSystem bubbleParticleSystem;
    private ParticleSystem.EmissionModule bubbleEmission;
    [SerializeField] public float emissionRateFactor = 10f;
    private Vector3 previousPosition;

    public Rigidbody2D rb;

    public bool GetGrounded()
    {
        return m_isGrounded;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        // Dynamically assign each Audio Source
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 5)
        {
            ambientAudioSource = audioSources[0];  // Ambient audio
            footstepsAudioSource = audioSources[1];  // Footsteps
            jumpAudioSource = audioSources[2];  // Jump
            swimAudioSource = audioSources[3];  // Swim
            drownAudioSource = audioSources[4];  // Drowning
        }
        else
        {
            Debug.LogError("Not enough AudioSources attached to Player!");
        }

        // Play ambient sound
        if (BackgroundAmbience.Count > 0)
        {
            int randomIndex = Random.Range(0, BackgroundAmbience.Count);
            ambientAudioSource.clip = BackgroundAmbience[randomIndex];
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }

        rb = GetComponent<Rigidbody2D>();
        m_isGrounded = true;
        m_isSwimming = false;
        rb.freezeRotation = true;

        if (oxygenSystem == null)
        {
            oxygenSystem = GetComponent<OxygenSystem>();
        }
        if (bubblePrefab != null)
        {
            bubbleParticleSystem = Instantiate(bubblePrefab, transform);
            bubbleEmission = bubbleParticleSystem.emission;
            bubbleParticleSystem.Stop();
        }
    }

    void Update()
    {
        PlayerInput();
        ApplyTilt();

        // Handle swimming sound
        if (m_isSwimming && oxygenSystem.GetCurrentOxygen() > 0)
        {
            oxygenSystem.DrainOxygen();
            if (!isPlayingSwimSound)
            {
               
                swimAudioSource.clip = swimSound;
                swimAudioSource.Play();
                isPlayingSwimSound = true;
            }
            // Start emitting bubbles
            if (!bubbleParticleSystem.isPlaying)
            {
                bubbleParticleSystem.Play();
            }
        }
        else
        {
            StopSound(swimAudioSource);  // Stop swim sound when leaving water
            isPlayingSwimSound = false;
            // Stop emitting bubbles when exiting water
            if (bubbleParticleSystem.isPlaying)
            {
                bubbleParticleSystem.Stop();
            }
            oxygenSystem.RefillOxygen();
        }
        Debug.Log(oxygenSystem.GetCurrentOxygen());

        // Oxygen threshold for drowning sound (20% of max oxygen)
        float oxygenThreshold = oxygenSystem.GetMaxOxygen() * 0.2f;

        // Handle drowning sound when oxygen is 20% or lower
        if (oxygenSystem.GetCurrentOxygen() <= oxygenThreshold && oxygenSystem.GetCurrentOxygen() > 0 && !hasPlayedDrownSound)
        {
            StopSound(swimAudioSource);  // Stop swim sound
            drownAudioSource.PlayOneShot(drownSound);  // Play drowning sound once
            hasPlayedDrownSound = true;
        }

        // If oxygen is replenished above 20%, reset drowning sound flag
        if (oxygenSystem.GetCurrentOxygen() > oxygenThreshold)
        {
            hasPlayedDrownSound = false;  // Reset for the next time oxygen falls below 20%
        }

        // Handle oxygen depletion and player respawn
        if (oxygenSystem.GetCurrentOxygen() <= 0)
        {
            Debug.Log("Oxygen depleted, respawning player.");
            StopSound(swimAudioSource);  // Stop swim sound
            RespawnPlayer();
        }

        UpdateAnimator();
        HandleFootsteps();  // Handle footsteps sound
        AdjustBubbleEmissionBasedOnVelocity(m_moveDirection);
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (m_isGrounded && !m_isSwimming)
        {
            rb.gravityScale = gravityScale;
        }
        else if (m_isSwimming)
        {
            ApplyUnderwaterMovement(m_moveDirection);
            rb.gravityScale = waterGravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void AdjustBubbleEmissionBasedOnVelocity(Vector2 m_moveDirection)
    {
        if (bubbleParticleSystem == null) return;

        // Calculate current speed and direction of movement
        float currentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        Vector3 bubbleDirection = (transform.position - previousPosition).normalized;

        // Set up the velocity over lifetime module
        var velocityModule = bubbleParticleSystem.velocityOverLifetime;
        velocityModule.enabled = true;  // Enable the velocity over lifetime module

        // Ensure constant upward velocity (independent of player direction)
        velocityModule.y = new ParticleSystem.MinMaxCurve(0.5f);  // Constant upward force

        if (currentSpeed > 0.1f)  // Only emit bubbles when moving
        {
            if (!bubbleParticleSystem.isPlaying)
            {
                bubbleParticleSystem.Play();  // Start emitting bubbles if they aren't already
            }

            previousPosition = transform.position;  // Update the previous position

            // Ensure minimum and consistent emission rate based on speed
            float emissionRate = Mathf.Max(3f, Mathf.Lerp(3f, emissionRateFactor, currentSpeed / m_movementSpeed));  // Ensuring a minimum of 3

            // Update the emission rate in the particle system
            bubbleEmission.rateOverTime = emissionRate;

            // Adjust horizontal velocity based on movement direction
            float horizontalVelocity = 0.0f;  // Default to no horizontal movement

            if (bubbleDirection.x < 0)  // Player is moving left
            {
                horizontalVelocity = 0.1f;  // Bubbles drift slightly to the right
            }
            else if (bubbleDirection.x > 0)  // Player is moving right
            {
                horizontalVelocity = -0.1f;  // Bubbles drift slightly to the left
            }

            // Apply both horizontal and upward velocities
            velocityModule.x = new ParticleSystem.MinMaxCurve(horizontalVelocity);
        }
        else  // If the player is not moving
        {
            // Reduce emission rate to zero but don't stop the particle system
            bubbleEmission.rateOverTime = 0;
        }
    }

    private void PlayerInput()
    {
        m_horizontalInput = Input.GetAxisRaw("Horizontal");

        if (m_isSwimming)
        {
            m_verticalInput = Input.GetAxisRaw("Vertical");
        }
        else
        {
            m_verticalInput = 0f;
        }

        m_moveDirection = new Vector2(m_horizontalInput, m_verticalInput).normalized;

        if (m_isGrounded && Input.GetButtonDown("Jump") && !hasJumped)
        {
            //Debug.Log("Jump pressed, playing jump sound.");
            Jump();
            hasJumped = true;
        }
        else if (m_isGrounded)
        {
            hasJumped = false;  // Reset jump flag when grounded
        }
    }

    void MovePlayer()
    {
        float currentSpeed = m_isSwimming ? m_swimmingSpeed : m_movementSpeed;
        Vector2 targetVelocity = m_moveDirection * currentSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref m_currentVelocity, m_movementSmooth);
    }

    private void Jump()
    {
        //Debug.Log("Executing Jump.");
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
        animator.SetTrigger("Jump!");
        jumpAudioSource.PlayOneShot(jumpSound);  // Play jump sound once
       // Debug.Log("Jump sound played.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = true;
            animator.SetBool("isGrounded", m_isGrounded);
           // Debug.Log("Player is grounded.");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Water"))
        {
           // Debug.Log("Entering water, starting swimming.");
            m_isSwimming = true;
            animator.SetBool("isSwimming", m_isSwimming);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = false;
            animator.SetBool("isGrounded", m_isGrounded);
           // Debug.Log("Player left the ground.");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Water"))
        {
           // Debug.Log("Exiting water, stopping swimming.");
            m_isSwimming = false;
            animator.SetBool("isSwimming", m_isSwimming);
            
        }
    }

    private void ApplyTilt()
    {
        if (m_horizontalInput != 0)
        {
            float tilt = -m_horizontalInput * tiltAngle;
            transform.rotation = Quaternion.Euler(0f, 0f, tilt);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
    {
        if (source != null && clip != null)
        {
            source.clip = clip;
            source.loop = loop;
            source.Play();
        }
    }

    private void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }

    private void HandleFootsteps()
    {
        // Debugging logic for footsteps
       // Debug.Log($"HandleFootsteps: m_horizontalInput: {m_horizontalInput}, m_isGrounded: {m_isGrounded}, isPlayingFootsteps: {isPlayingFootsteps}, m_isSwimming: {m_isSwimming}");

        // Play footsteps only when grounded, moving, not swimming, and not jumping
        if (m_horizontalInput != 0 && m_isGrounded && !isPlayingFootsteps && !m_isSwimming && !Input.GetButton("Jump"))
        {
           // Debug.Log("Playing footsteps.");
            footstepsAudioSource.clip = footstepSound;
            footstepsAudioSource.Play();  // Play footsteps sound
            isPlayingFootsteps = true;
        }
        else if (m_horizontalInput == 0 || !m_isGrounded || m_isSwimming || Input.GetButton("Jump"))
        {
            if (isPlayingFootsteps)
            {
               // Debug.Log("Stopping footsteps.");
                footstepsAudioSource.Stop();  // Stop footsteps
                isPlayingFootsteps = false;
            }
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(m_horizontalInput));

            // Flip sprite based on direction
            if (m_horizontalInput < 0)
            {
                transform.localScale = new Vector3(2, 2, 1); // Flip left
            }
            else if (m_horizontalInput > 0)
            {
                transform.localScale = new Vector3(-2, 2, 1); // Flip right
            }
        }
    }

    private void RespawnPlayer()
    {
        transform.position = new Vector2(8.64f, -2.54f);
        oxygenSystem.Start();  // Reset oxygen after respawn
    }

    private void ApplyUnderwaterMovement(Vector2 direction)
    {
        Vector2 movementForce = direction * m_currentVelocity * (0.01f + impulseFactor);
        movementForce.y += buoyancyForce;
        rb.AddForce(movementForce, ForceMode2D.Force);
    }
}
