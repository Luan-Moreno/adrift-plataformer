using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Item", menuName = "Items/Damage")]
public class DamageItemData : ItemData
{
    [Header("Effects")]
    public int damageAmount;

    public override void OnUse(GameObject player)
    {
        var combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.TakeDamage(damageAmount);
        }
    }
}
