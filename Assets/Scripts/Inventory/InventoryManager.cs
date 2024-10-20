using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private bool menuActivated = false;

    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && menuActivated)
        {
            // Play Game
            Time.timeScale = 1f;
            // Close Window
            inventoryMenu.SetActive(false);
            menuActivated = false;
            return;
        }
        else if (Input.GetButtonDown("Fire2") && !menuActivated)
        {
            // Pause Game
            Time.timeScale = 0f;
            // Open Window
            inventoryMenu.SetActive(true);
            menuActivated = true;
            return;
        }
    }

    public void UseItem(string itemName)
    {
        for (int i = 0; i  < itemSOs.Length; i++) 
        {
            if (itemSOs[i].itemName == itemName)
            {
                itemSOs[i].useItem();
            }
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        Debug.Log("item - " + itemName + " quantity - " +  quantity);
        for (int i = 0; i < itemSlot.Length; i++) 
        {
            if (!itemSlot[i].isFull && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
            {
                int leftoverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftoverItems > 0) 
                {
                    leftoverItems = AddItem(itemName, leftoverItems, itemSprite, itemDescription);
                }

                return leftoverItems;
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedBox.SetActive(false);
            itemSlot[i].thisItemIsSelected = false;
        }
    }
}
