using System.Collections;
using UnityEngine;

public class GhostBoss : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float currentHealth;
    public float maxHealth = 100f;

    [Header("Movement")]
    [SerializeField] private bool canTeleport = true;
    public float teleportCooldown = 2f;
    
    [Header("Attack")]
    public GameObject knifePrefab;
    public Transform attackPoint;
    public float attackCooldown = 1.5f;


    [Header("Second Phase")]
    [SerializeField] private bool phase2 = false;
    public float phase2HealthThreshold = 50f;
    public float phase2TeleportCooldown = 1f;

    [Header("Sounds")]
    [SerializeField] private AudioClip teleportSoundClip;
    [SerializeField] private AudioClip phase2SoundClip;
    [SerializeField] private AudioClip dyingSoundClip;

    [Header("Particles")]
    public ParticleSystem teleportEffect;
    public ParticleSystem deathEffect;
    public Transform[] teleportPositions;

    private EnemyCombat enemyCombat;
    private bool canAttack = true;
    private Transform player;


    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
        canTeleport = true;
        enemyCombat = GetComponent<EnemyCombat>();
        currentHealth = enemyCombat.enemyHp;
    }

    void Update()
    {
        
        currentHealth = enemyCombat.enemyHp;
        if(enemyCombat.enemyHp == 1)
        {
            currentHealth = 0;
        }

        if (canTeleport) 
        {
            StartCoroutine(Teleport());
        }

        if (canAttack) 
        {
            StartCoroutine(Attack());
        }

        CheckPhase2();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckPhase2()
    {
        if (!phase2 && currentHealth <= phase2HealthThreshold)
        {
            phase2 = true;
            SoundFXManager.instance.PlaySoundFX(phase2SoundClip, transform, 0.8f);
            teleportCooldown = phase2TeleportCooldown;
        }
    }

    void Die()
    {
        SoundFXManager.instance.PlaySoundFX(dyingSoundClip, transform, 1.2f);
        if (deathEffect != null) 
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        // SequenceManager.instance.ENDINGSEQUENCE()
    }

    IEnumerator Teleport()
    {
        canTeleport = false;

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }
            
        int index = Random.Range(0, teleportPositions.Length);

        while(transform.position == teleportPositions[index].position)
        {
            index = Random.Range(0, teleportPositions.Length);
        }

        transform.position = teleportPositions[index].position;

        SoundFXManager.instance.PlaySoundFX(teleportSoundClip, transform, 0.2f);

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }
        
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
}
