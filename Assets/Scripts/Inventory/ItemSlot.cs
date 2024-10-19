using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class ItemSlot : MonoBehaviour
{
    // --------- Item Data ---------
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;

    // --------- Item Slot ---------
    [SerializeField] TMP_Text quantityText;
    [SerializeField] Image itemImage;

    public void addItem(string addItemName, int addQuantity, Sprite addItemSprite)
    {
        this.itemName = addItemName;
        this.quantity = addQuantity;
        this.itemSprite = addItemSprite;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;

        itemImage.sprite = itemSprite;
        itemImage.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
