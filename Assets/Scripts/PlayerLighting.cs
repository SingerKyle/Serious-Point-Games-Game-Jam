using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLighting : MonoBehaviour
{
    // Reference to the player's transform and light component
    public Transform player;
    public Light2D spotlight; // If using Unity's 2D light system

    // Customize these parameters for the light effect
    public float lightIntensity = 1.0f;
    public float lightRange = 5.0f;
    public float intensityChangeSpeed = 0.1f; // Change speed for dynamic effects
    public float lightOffset = 0.5f; // Light offset from the player

    private Vector3 initialLightPosition;

    void Start()
    {
        if (player == null)
        {
            player = this.transform; // Assume the script is on the player if no player is assigned
        }

        if (spotlight == null)
        {
            spotlight = GetComponent<Light2D>(); // Find the Light2D component on this object
        }

        // Store the initial light position relative to the player
        initialLightPosition = spotlight.transform.localPosition;
    }

    void Update()
    {
        // Keep the spotlight attached to the player
        FollowPlayer();

        // You can adjust light properties dynamically based on certain conditions (optional)
        UpdateLightIntensity();
    }

    void FollowPlayer()
    {
        // Keep the light at the player's position with a slight offset
        Vector3 lightPosition = player.position + new Vector3(lightOffset, lightOffset, 0);
        spotlight.transform.position = lightPosition;
    }

    void UpdateLightIntensity()
    {
        // Example: Increase or decrease the light intensity based on some condition (e.g., player speed or input)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            spotlight.intensity += intensityChangeSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            spotlight.intensity -= intensityChangeSpeed * Time.deltaTime;
        }

        // Clamp the intensity to avoid negative values
        spotlight.intensity = Mathf.Clamp(spotlight.intensity, 0.0f, lightIntensity);
    }
}
