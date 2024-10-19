using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private bool menuActivated = false;

    public ItemSlot[] itemSlot;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && menuActivated)
        {
            // Play Game
            Time.timeScale = 1f;
            // Close Window
            inventoryMenu.SetActive(false);
            menuActivated = false;
            return;
        }
        else if (Input.GetButtonDown("Fire1") && !menuActivated)
        {
            // Pause Game
            Time.timeScale = 0f;
            // Open Window
            inventoryMenu.SetActive(true);
            menuActivated = true;
            return;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
        Debug.Log("item - " + itemName + " quantity - " +  quantity);
        for (int i = 0; i < itemSlot.Length; i++) 
        {
            if (!itemSlot[i].isFull)
            {
                itemSlot[i].addItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }
}
