using UnityEngine;
using System.Collections;
using System.Numerics;

public class HazardBehaviour : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool damageable = true;
    [SerializeField] private bool causesRespawn = true;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private int healthAmount = 100;
    [SerializeField] private float invulnerabilityTime = 0.2f;
    public bool giveUpwardForce = true;
    private bool hit;
    private int currentHealth;
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private GameObject player;
    private UIManager uiM;
    private SequenceManager sequenceManager;

    void Start()
    {
        currentHealth = healthAmount;
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        player = playerCombat.gameObject;
        uiM = FindAnyObjectByType<UIManager>();
        sequenceManager = FindAnyObjectByType<SequenceManager>();
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

    IEnumerator Hit()
    {
        playerCombat.TakeDamage(damageAmount);
        yield return new WaitForSeconds(invulnerabilityTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Hit());
            if (causesRespawn)
            {
                StartCoroutine(sequenceManager.Respawn());
            }
        }
    }
}
