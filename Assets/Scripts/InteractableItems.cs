using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableItems : MonoBehaviour
{
    [SerializeField] protected string itemName;
    public abstract void Interact();
}

public class Key : InteractableItems
{
    public override void Interact()
    {
        Debug.Log($"Collected {itemName}");

        Destroy(gameObject); // Remove from the scene
    }
}



