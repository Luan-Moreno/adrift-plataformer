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

    IEnumerator Respawn()
    {
        if (playerCombat.IsRespawning)
        {
            yield break;
        }

        playerCombat.IsRespawning = true;
        playerCombat.IsImmortal = true;
        playerMovement.Speed = 0;
        playerMovement.Rig.linearVelocity = UnityEngine.Vector2.zero;
        uiM.fade.SetActive(true);
        yield return StartCoroutine(uiM.Fade(0, 1, 0.65f));
        yield return new WaitForSeconds(0.1f);
        player.transform.position = sequenceManager.spawnPoint;
        playerMovement.Rig.linearVelocity = UnityEngine.Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(uiM.Fade(1, 0, 0.65f));
        uiM.fade.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        playerMovement.Speed = playerMovement.InitialSpeed;
        playerCombat.IsRespawning = false;
        playerCombat.IsImmortal = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Hit());
            if (causesRespawn)
            {
                StartCoroutine(Respawn());
            }
        }
    }
}
