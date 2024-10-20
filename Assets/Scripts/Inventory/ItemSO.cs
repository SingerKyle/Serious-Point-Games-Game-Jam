using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChange;

    public bool useItem()
    {
        switch (statToChange) 
        {
            case StatToChange.none:
                return true;
            case StatToChange.lightSource:
                break;
            case StatToChange.key:
                Debug.Log("useItem Enum");
                bool usable = DoorUnlockAttempt();
                return usable;
        }

        return true; // change when extra functionality is added
    }

    protected bool DoorUnlockAttempt()
    {
        Debug.Log("DoorUnlockAttempt");
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // Get the player's collider
        Collider2D playerCollider = player.GetComponent<CircleCollider2D>();
        if (playerCollider == null) return false;

        // Check for doors within the player's collider
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerCollider.transform.position, playerCollider.bounds.extents.x);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Door"))
            {
                Door door = collider.GetComponent<Door>();
                if (door != null && door.GetLocked())
                {
                    return door.Unlock();
                }
            }
        }
        return false;
    }

    // for any potential healing items etc
    public enum StatToChange
    {
        none,
        lightSource,
        key
    };
}
