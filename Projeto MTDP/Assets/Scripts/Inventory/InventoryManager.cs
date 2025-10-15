using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool isMenuActivated;
    public ItemSlot[] itemSlot;

    void Update()
    {

    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (ItemSlot slot in itemSlot)
        {
            if (slot.isFull == false)
            {
                slot.AddItem(itemName, quantity, itemSprite, itemDescription);
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
}
