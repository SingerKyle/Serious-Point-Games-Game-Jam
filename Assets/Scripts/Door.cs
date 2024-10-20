using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked = false;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private float DoorOpenWaitTime = 2.5f;

    private bool isOpen = false;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    private IEnumerator OpenDoor(Collider2D playerCollider)
    {
        // Set bool - stops door being spammed
        isOpen = true;
        Debug.Log("Opening Dooor!");

        // Add open sound

        // add anim if we want one:

        // disable collision
        doorCollider.isTrigger = true;

        // Wait Time for Door
        yield return new WaitForSeconds(DoorOpenWaitTime);

        // Enable collision
        doorCollider.isTrigger = false;

        // Reset Bool
        isOpen = false;
        Debug.Log("Door Closed!");

        // Add Closed Sound
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Trigger");

        // if door is locked return
        if (isLocked) 
        {
            return;
        }   
        
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
        if(isLocked)
        {
            isLocked = false;
            // testing for now
            Debug.Log("Door Unlocked!");
            return true;
        }
        return false;
    }

    public bool GetLocked()
    {
        return isLocked;
    }
}
