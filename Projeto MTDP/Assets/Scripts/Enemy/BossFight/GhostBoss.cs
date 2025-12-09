using System.Collections;
using UnityEngine;

public class GhostBoss : MonoBehaviour
{
    [Header("Movement")]
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    public float teleportDistance = 5f;
    public float teleportCooldown = 2f;

    [Header("Attack")]
    public GameObject knifePrefab;
    public Transform attackPoint;
    public float attackCooldown = 1.5f;

    [Header("Second Phase")]
    public float phase2HealthThreshold = 50f;
    public float phase2TeleportCooldown = 1f;

    [Header("Particles")]
    public ParticleSystem teleportEffect;
    public ParticleSystem deathEffect;

    [Header("Life")]
    public float maxHealth = 100f;
    private float currentHealth;

    private Vector3 initialPos;
    private Transform player;
    private bool canTeleport = true;
    private bool canAttack = true;
    private bool phase2 = false;

    void Start()
    {
        initialPos = transform.position;
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        FloatMovement();
        CheckPhase2();
    }

    void FloatMovement()
    {
        transform.position = initialPos + Vector3.up * Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
    }

    void CheckPhase2()
    {
        if (!phase2 && currentHealth <= phase2HealthThreshold)
        {
            phase2 = true;
            teleportCooldown = phase2TeleportCooldown;
            // Particulas, Cor...
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        // SFXManager (Grito)
        Destroy(gameObject);
        // Cutscene Final (Pré-Alpha)
    }

    IEnumerator Teleport()
    {
        canTeleport = false;
        if (teleportEffect != null) Instantiate(teleportEffect, transform.position, Quaternion.identity);

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 newPos = player.position - dir * teleportDistance;

        transform.position = newPos;

        if (teleportEffect != null) Instantiate(teleportEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    IEnumerator Attack()
    {
        canAttack = false;

        if (knifePrefab != null && attackPoint != null)
        {
            GameObject knife = Instantiate(knifePrefab, attackPoint.position, Quaternion.identity);
            Vector3 dir = (player.position - attackPoint.position).normalized;
            knife.GetComponent<Rigidbody2D>().linearVelocity = dir * 10f; // ajuste a velocidade
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void FixedUpdate()
    {
        if (canTeleport) StartCoroutine(Teleport());
        if (canAttack) StartCoroutine(Attack());
    }
}
