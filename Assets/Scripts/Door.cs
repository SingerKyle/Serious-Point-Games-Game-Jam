using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked = false;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private float DoorOpenWaitTime = 2.5f;

    // Audio clips for door actions
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip lockedAttemptSound;  // New sound when door is locked and player can't unlock

    // AudioSources for playing sounds
    private AudioSource lockedAudioSource;
    private AudioSource doorAudioSource;
    private AudioSource attemptAudioSource;  // New AudioSource for the locked attempt sound

    private bool isOpen = false;
    private bool hasPlayedOpenSound = false;   // Ensures open sound only plays once
    private bool hasPlayedLockedSound = false; // Ensures locked sound only plays when the door is locked
    private bool hasPlayedLockedAttempt = false; // Ensures locked attempt sound only plays once per interaction

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();

        // Get audio sources attached to the door
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 3)
        {
            doorAudioSource = audioSources[0];   // First audio source for door open sound
            lockedAudioSource = audioSources[1]; // Second audio source for locked sound
            attemptAudioSource = audioSources[2]; // Third audio source for locked attempt sound
        }
        else
        {
            Debug.LogError("Not enough AudioSources attached to the door!");
        }
    }

    private IEnumerator OpenDoor(Collider2D playerCollider)
    {
        // Set bool - stops door being spammed
        isOpen = true;
        Debug.Log("Opening Door!");

        // Play the open sound only once
        if (!hasPlayedOpenSound && doorAudioSource != null && openSound != null)
        {
            doorAudioSource.PlayOneShot(openSound);
            hasPlayedOpenSound = true; // Mark the open sound as played
        }

        // Add anim if we want one:
        // (Optional: Add door opening animation here)

        // Disable collision (open door)
        doorCollider.isTrigger = true;

        // Wait Time for Door to remain open
        yield return new WaitForSeconds(DoorOpenWaitTime);

        // Enable collision (close door)
        doorCollider.isTrigger = false;

        // Reset Bool for door state
        isOpen = false;
        Debug.Log("Door Closed!");

        // Add Closed Sound (Optional: Add a closing sound if needed)
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Player Near Door");

        // If the door is locked and the player doesn't have the key
        if (isLocked)
        {
            if (!hasPlayedLockedSound && lockedAudioSource != null && lockedSound != null)
            {
                lockedAudioSource.PlayOneShot(lockedSound);
                hasPlayedLockedSound = true; // Mark the locked sound as played
            }

            // Play locked attempt sound when the door is locked and the player tries to open it without a key
            if (!hasPlayedLockedAttempt && attemptAudioSource != null && lockedAttemptSound != null && Input.GetButton("Interact"))
            {
                attemptAudioSource.PlayOneShot(lockedAttemptSound); // Play attempt sound once per interaction
                hasPlayedLockedAttempt = true;
            }

            return;
        }

        // If the player presses the interact button and the door is not open, open it
        if (collision.CompareTag("Player") && Input.GetButton("Interact"))
        {
            if (!isOpen)
            {
                StartCoroutine(OpenDoor(collision));
            }
        }
    }

    public bool Unlock()
    {
        if (isLocked)
        {
            isLocked = false;
            Debug.Log("Door Unlocked!");

            // Reset the locked sound flag so it won't play again after unlocking
            hasPlayedLockedSound = false;

            // Reset the locked attempt sound flag so it can play again if the door gets locked again
            hasPlayedLockedAttempt = false;

            return true;
        }
        return false;
    }

    public bool GetLocked()
    {
        return isLocked;
    }
    public bool IsOpen()
    {
        return isOpen;
    }
    public void Open()
    {
        isOpen = true;
        // Trigger door open animation or any other logic here
    }
}
