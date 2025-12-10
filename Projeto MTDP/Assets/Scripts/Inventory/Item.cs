using UnityEngine;

public class Item : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string uniqueId;
    [SerializeField] private bool collected = false;
    [SerializeField] private ItemData itemData;
    [SerializeField] private AudioClip itemPickupSoundClip;

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
            int leftOverItems = inventoryManager.AddItem(
            itemData.itemName,
            itemData.displayName,
            itemData.quantity,
            itemData.itemSprite,
            itemData.itemDescription
            );

            if (leftOverItems == 0) { Destroy(gameObject); }
            else { itemData.quantity = leftOverItems; }

            if (itemData.disappears) { gameObject.SetActive(false); }
            SoundFXManager.instance.PlaySoundFX(itemPickupSoundClip, transform);
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
