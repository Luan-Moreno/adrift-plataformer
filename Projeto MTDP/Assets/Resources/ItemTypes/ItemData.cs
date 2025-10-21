using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class ItemData : ScriptableObject
{
    [Header("Display Info")]
    public string displayName;
    public Sprite itemSprite;
    [TextArea] public string itemDescription;
    
    [Header("Item Info")]
    public string itemName;
    public int quantity = 1;
    public bool disappears = true;

    public abstract void OnUse(GameObject player);

    public virtual bool CanUse(GameObject player) {return true;}
}
