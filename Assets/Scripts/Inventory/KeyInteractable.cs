using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    // --------- Base variables for each item ---------
    [SerializeField] protected string itemName;
    [SerializeField] protected int quantity;
    [SerializeField] protected Sprite sprite;
    [TextArea][SerializeField] protected string itemDescription;

    // --------- Inventory Manager ---------
    protected InventoryManager inventoryManager;

    public virtual void Initialise(string name, int quantity, Sprite sprite, string description)
    {
        itemName = name;
        this.quantity = quantity;
        this.sprite = sprite;
        itemDescription = description;
    }

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
