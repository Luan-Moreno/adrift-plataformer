using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    #region Variables
    public static InventoryManager instance { get; private set; }
    public GameObject InventoryMenu;
    private bool isMenuActivated;
    public ItemSlot[] itemSlot;
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one InventoryManager in the scene!");
        }
        instance = this;
    }

    public void AddItem(string itemName, string displayName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isFull == false)
            {
                slot.AddItem(itemName, displayName, quantity, itemSprite, itemDescription);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        foreach (ItemSlot slot in itemSlot)
        {
            slot.selectedPanel.SetActive(false);
            slot.thisItemSelected = false;
        }
    }
    
    public void SaveData(ref GameData data)
    {
        data.itemNames.Clear();
        data.itemQuantities.Clear();

        foreach (ItemSlot slot in itemSlot)
        {
            if(slot.isFull)
            {
                data.itemNames.Add(slot.itemName);
                data.itemQuantities.Add(slot.quantity);
            }
        }
    }

    public void LoadData(GameData data)
    {
        for (int i = 0; i < data.itemNames.Count; i++)
        {
            string itemName = data.itemNames[i];
            int quantity = data.itemQuantities[i];

            ItemData itemData = Resources.Load<ItemData>("Items/" + itemName);
            if (itemData != null)
            {
                itemSlot[i].AddItem(
                    itemData.itemName,
                    itemData.displayName,
                    quantity,
                    itemData.itemSprite,
                    itemData.itemDescription
                );
            }
        }
    }
}
