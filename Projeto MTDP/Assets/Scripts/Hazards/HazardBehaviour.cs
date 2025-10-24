using UnityEngine;
using System.Collections;

public class HazardBehaviour : MonoBehaviour
{
    [SerializeField] private bool damageable = true;
    [SerializeField] private int healthAmount = 100;
    [SerializeField] private float invulnerabilityTime = 0.2f;
    public bool giveUpwardForce = true;
    private bool hit;
    private int currentHealth;

    private PlayerCombat playerCombat;

    void Start()
    {
        currentHealth = healthAmount;
        playerCombat = FindAnyObjectByType<PlayerCombat>();
    }

    public void Damage(int amount)
    {
        if (damageable && !hit && currentHealth > 0)
        {
            hit = true;
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(TurnOffHit());
            }
        }
    }
    
    IEnumerator TurnOffHit()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        hit = false;
    }
}
