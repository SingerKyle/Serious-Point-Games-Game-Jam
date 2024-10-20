using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItems : MonoBehaviour
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
}


