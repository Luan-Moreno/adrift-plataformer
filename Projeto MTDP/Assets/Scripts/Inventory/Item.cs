using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private int quantity;
    [SerializeField] private Sprite itemSprite;
    [TextArea][SerializeField] private string itemDescription;
    private InventoryManager inventoryManager;
    
    void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemName, quantity, itemSprite, itemDescription);
            Destroy(gameObject);
        }
    }
}
