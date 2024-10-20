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
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            // Play Game
            Time.timeScale = 1f;
            // Close Window
            inventoryMenu.SetActive(false);
            menuActivated = false;
            return;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            // Pause Game
            Time.timeScale = 0f;
            // Open Window
            inventoryMenu.SetActive(true);
            menuActivated = true;
            return;
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            //Debug.Log("Search Name - " + itemName + " Current ItemSO - " + itemSOs[i].ToString() + " Length of ItemSO - " + itemSOs.Length.ToString());

            if (itemSOs[i].itemName == itemName)
            {
                Debug.Log("Item Usable in Manager");
                bool usable = itemSOs[i].useItem();
                return usable;
            }
        }

        return false;
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
