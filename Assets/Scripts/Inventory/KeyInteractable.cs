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
            if (Input.GetButton("Fire1"))
            {
                int leftoverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
                if(leftoverItems <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    quantity = leftoverItems;
                }
            }
        }
        else
        {
            Debug.Log("Nothing");
        }
    }
}
