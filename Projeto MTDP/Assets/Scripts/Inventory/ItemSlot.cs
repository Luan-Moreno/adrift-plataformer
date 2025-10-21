using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Interactions;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Data")]
    public string itemName = null;
    public string displayName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public bool isUsable;
    public Sprite emptySprite;
    [SerializeField] private int maxNumberOfItems;

    [Header("Item Slot")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    public GameObject selectedPanel;
    public bool thisItemSelected;

    [Header("Item Description")]
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionName;
    public TMP_Text itemDescriptionText;


    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public int AddItem(string itemName, string displayName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (string.IsNullOrEmpty(this.itemName))
        {
            this.itemName = itemName;
            this.displayName = displayName;
            this.itemSprite = itemSprite;
            this.itemDescription = itemDescription;
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;
        }

        if(this.itemName != itemName) { return quantity; }

        int total = this.quantity + quantity;

        if (total >= maxNumberOfItems)
        {
            int extra = total - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            isFull = true;
            quantityText.text = this.quantity.ToString();
            quantityText.enabled = true;
            return extra;
        }

        this.quantity = total;
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        return 0;
    }
    
    public void RemoveItem()
    {
        itemName = null;
        displayName = null;
        itemSprite = null;
        itemDescription = null;
        itemImage.sprite = emptySprite;
        itemImage.enabled = false;

        quantity = 0;
        quantityText.text = quantity.ToString();
        quantityText.enabled = false;
        isFull = false;
    }


    public void AddWeapon(string itemName, string displayName, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.displayName = displayName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        itemImage.sprite = itemSprite;
        itemImage.enabled = true;
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            if (quantity > 1)
            {
                inventoryManager.UseItem(itemName);
                quantity--;
                quantityText.text = quantity.ToString();
            }
            else if (quantity == 1)
            {
                isUsable = inventoryManager.UseItem(itemName);
                if(isUsable)
                {
                    RemoveItem();
                }
            }
        }
        
        inventoryManager.DeselectAllSlots();
        selectedPanel.SetActive(true);
        thisItemSelected = true;

        itemDescriptionName.text = displayName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        
        if(itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }
    }

    public void OnRightClick()
    {
        return;
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


}
