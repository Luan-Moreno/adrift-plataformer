using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Data")]
    public string itemName;
    public string displayName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;

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
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void AddItem(string itemName, string displayName, int quantity, Sprite itemSprite, string itemDescription)
    {
        this.itemName = itemName;
        this.displayName = displayName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
        itemImage.enabled = true;
    }

    public void OnLeftClick()
    {
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
