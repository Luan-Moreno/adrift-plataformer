using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    #region Variables
    public static InventoryManager instance { get; private set; }
    public GameObject InventoryMenu;
    public GameObject player;
    private bool isMenuActivated;
    public ItemSlot[] itemSlot;
    public ItemSlot[] weaponSlot;
    private List<ItemData> itemDatas;
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one InventoryManager in the scene!");
        }
        instance = this;
        itemDatas = Resources.LoadAll<ScriptableObject>("Items").OfType<ItemData>().ToList();
    }

    public int AddItem(string itemName, string displayName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (!slot.isFull)
            {
                quantity = slot.AddItem(itemName, displayName, quantity, itemSprite, itemDescription);
                if (quantity == 0) return 0;
            }
        }
        return quantity;
    }

    public bool UseItem(string itemName)
    {
        foreach (ItemData data in itemDatas)
        {
            if (data.itemName == itemName)
            {
                if (data.CanUse(player))
                {
                    data.OnUse(player);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    public void AddWeapon(string itemName, string displayName, Sprite itemSprite, string itemDescription)
    {
        foreach (ItemSlot slot in weaponSlot)
        {
            if (slot.isFull == false)
            {
                slot.AddWeapon(itemName, displayName, itemSprite, itemDescription);
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

        foreach (ItemSlot slot in weaponSlot)
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
            if (slot.isFull || slot.quantity > 0)
            {
                data.itemNames.Add(slot.itemName);
                data.itemQuantities.Add(slot.quantity);
            }
        }

        data.weaponNames.Clear();
        
        foreach (ItemSlot slot in weaponSlot)
        {
            if(slot.isFull)
            {
                data.weaponNames.Add(slot.itemName);
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

        for (int i = 0; i < data.weaponNames.Count; i++)
        {
            string weaponName = data.weaponNames[i];
            ItemData weaponData = Resources.Load<ItemData>("Weapons/" + weaponName);
            if (weaponData != null)
            {
                weaponSlot[i].AddWeapon(
                    weaponData.itemName,
                    weaponData.displayName,
                    weaponData.itemSprite,
                    weaponData.itemDescription
                );
            }
        }
    }
}
