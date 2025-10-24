using UnityEngine;

public class Weapon : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string uniqueId;
    [SerializeField] private bool collected = false;
    [SerializeField] private ItemData itemData;

    private InventoryManager inventoryManager;

    [ContextMenu("Generate New GUID")]
    private void GenerateNewGUID()
    {
        uniqueId = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            inventoryManager.AddWeapon(
            itemData.itemName,
            itemData.displayName,
            itemData.itemSprite,
            itemData.itemDescription
            );

            if (itemData.disappears) { gameObject.SetActive(false); }
            collected = true;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.collectables.ContainsKey(uniqueId))
        {
            data.collectables.Remove(uniqueId);
        }
        data.collectables.Add(uniqueId, collected);
    }

    public void LoadData(GameData data)
    {
        data.collectables.TryGetValue(uniqueId, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }
}
