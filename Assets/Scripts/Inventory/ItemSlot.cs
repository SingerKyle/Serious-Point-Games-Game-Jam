using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // --------- Item Data ---------
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    // If the slot is currently being used
    public bool isFull;
    // when you select an item, use this description
    public string itemDescription;
    // Empty Sprite to clear images
    public Sprite emptySprite;

    [SerializeField] public int maxItemCount = 5;

    // --------- Item Slot ---------
    [SerializeField] TMP_Text quantityText;
    [SerializeField] Image itemImage;

    // --------- Desc Slot ---------
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionDescriptionText;

    // --------- Currently Selected Box with bool for check ---------
    public GameObject selectedBox;
    public bool thisItemIsSelected;
    // --------- Inventory Manager ---------
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        isFull = false;
    }

    public int AddItem(string addItemName, int addQuantity, Sprite addItemSprite, string addItemDescription)
    {
        // check if slot is full
        if(isFull)
        {
            return quantity;
        }

        // set slot
        this.itemName = addItemName;
        
        this.itemSprite = addItemSprite;

        this.itemDescription = addItemDescription;

        this.quantity += addQuantity;

        itemImage.sprite = itemSprite;
        itemImage.enabled = true;

        // check if invenetory is true
        if (this.quantity >= maxItemCount)
        {
            quantityText.text = maxItemCount.ToString();
            quantityText.enabled = true;
            isFull = true;
        

            // return extra
            int extraItems = this.quantity - maxItemCount;
            this.quantity = maxItemCount;
            return extraItems;
        }

        // update quantity
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void RemoveItems()
    {
        quantityText.enabled = false;
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemDescription = "";
        itemName = "";
        itemSprite = null;

        itemDescriptionNameText.text = "";
        itemDescriptionDescriptionText.text = "";
        itemDescriptionImage.sprite = null;

    }

    public void OnLeftClick()
    {
        if (thisItemIsSelected)
        {
            inventoryManager.UseItem(itemName);
            if (this.quantity > 0)
            {
                this.quantity -= 1;
                quantityText.text = this.quantity.ToString();
            }
            if (this.quantity <= 0)
            {
                RemoveItems();
            }

            thisItemIsSelected = false;
        }
        else
        {

            // better way to do this, look at later
            inventoryManager.DeselectAllSlots();

            selectedBox.SetActive(true);
            thisItemIsSelected = true;

            itemDescriptionNameText.text = itemName;
            itemDescriptionDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
            if (itemDescriptionImage.sprite == null)
            {
                itemDescriptionImage.sprite = emptySprite;
            }
        }
    }

    public void OnRightClick()
    {
        if (this.quantity > 0)
        {
            // Drop Items - Create dropped item
            GameObject dropItem = new GameObject(itemName);
            Key newItem = dropItem.AddComponent<Key>();
            
            newItem.Initialise(itemName, 1, itemSprite, itemDescription);

            SpriteRenderer sr = dropItem.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprite;
            sr.sortingOrder = 5;
            sr.sortingLayerName = "Player";

            // add collider
            dropItem.AddComponent<BoxCollider2D>().isTrigger = true;

            // add float script
            dropItem.AddComponent<FloatItem>();

            // set posiiton
            dropItem.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(2f, 0, 0);
            dropItem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        
            this.quantity -= 1;
            quantityText.text = this.quantity.ToString();
        }

        if (this.quantity <= 0)
        {
            RemoveItems();
        }
    }
}
