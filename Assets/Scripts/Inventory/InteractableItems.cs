using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItems : MonoBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected int quantity;
    [SerializeField] protected Sprite sprite;

    [SerializeField]protected InventoryManager inventoryManager;

}


