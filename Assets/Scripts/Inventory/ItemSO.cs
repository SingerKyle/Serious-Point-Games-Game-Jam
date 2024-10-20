using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChange;

    public bool useItem()
    {
        bool usable = false;

        switch (statToChange) 
        {
            case StatToChange.none:
                return true;
            case StatToChange.match:
                usable = LightCandle();
                return usable;
            case StatToChange.key:
                //Debug.Log("useItem Enum");
                usable = DoorUnlockAttempt();
                return usable;
        }

        return true; // change when extra functionality is added
    }

    protected bool LightCandle()
    {

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Player Null!");
            return false;
        }

        // Get the player's "candle"
        Light2D candleLight = player.GetComponentInChildren<Light2D>();
        if (candleLight != null)
        {
            PlayerLighting candleScript = player.GetComponentInChildren<PlayerLighting>();

            // Light the candle
            if (candleScript != null ) 
            {
                candleScript.ReplenishCandle();
            }
            else
            {
                Debug.Log("Candle Script Null!");
            }

            // Successfully used match
            return true;
        }
        else
        {
            Debug.Log("Candle Null!");
        }

        return false;
    }    
    protected bool DoorUnlockAttempt()
    {
        //Debug.Log("DoorUnlockAttempt");
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return false;
        }

        // Get the player's collider
        Collider2D playerCollider = player.GetComponentInChildren<CircleCollider2D>();
        if (playerCollider == null)
        {
            return false;
        }

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
        match,
        key
    };
}
