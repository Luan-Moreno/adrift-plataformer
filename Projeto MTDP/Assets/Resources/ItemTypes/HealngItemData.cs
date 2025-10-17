using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Items/Healing")]
public class HealingItemData : ItemData
{
    [Header("Effects")]
    public int healAmount;

    public override void OnUse(GameObject player)
    {
        var combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.ReceiveHealing(healAmount);
        }
    }
}
