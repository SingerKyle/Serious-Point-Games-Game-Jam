using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key : InteractableItems
{

    private void Start()
    {
        itemName = "Key";

        // Get our Inventory Menu
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player")) // set player to player tag
        {
            Debug.Log("Press _ to pick up + itenName");
            if (Input.GetButton("Fire2"))
            {
                inventoryManager.AddItem(itemName, quantity, sprite);
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Nothing");
        }
    }
}
