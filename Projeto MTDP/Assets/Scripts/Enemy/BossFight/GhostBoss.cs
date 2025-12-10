using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostBoss : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float currentHealth = 1;
    
    public float maxHealth = 100f;

    [Header("Movement")]
    [SerializeField] private bool canTeleport = true;
    public float teleportCooldown = 2f;
    
    [Header("Attack")]
    public GameObject knifePrefab;
    public Transform attackPoint;
    public float attackCooldown = 2.2f;


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
    private Vector3 facingRight;
    private Vector3 facingLeft;
    private CinemachineCollisionImpulseSource impulseSource;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
        enemyCombat = GetComponent<EnemyCombat>();
        currentHealth = enemyCombat.enemyHp + 1;

        facingRight = Vector3.zero;
        facingLeft = new Vector3(0f, 180f, 0f);
        impulseSource = GetComponent<CinemachineCollisionImpulseSource>();

        canTeleport = false;
        Invoke(nameof(AllowTeleport), 1.5f);
    }

    void AllowTeleport() => canTeleport = true;

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
            Flip();
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
            CameraManager.instance.StrongCameraShake(impulseSource);
            Instantiate(deathEffect, transform.parent.position, Quaternion.identity);
        }
        SequenceManager.instance.StartCoroutine(SequenceManager.instance.Ending());
        Destroy(gameObject, 0.1f);
    }

    private void Flip()
    {
        Vector3 target = player.transform.position;

        if (transform.position.x > target.x)
        {
            
            transform.eulerAngles = facingRight;
        }
            
        else
        {
            transform.eulerAngles = facingLeft;
        }
    }

    IEnumerator Teleport()
    {
        canTeleport = false;

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.parent.position, Quaternion.identity);
        }
            
        int index = Random.Range(0, teleportPositions.Length);

        while(transform.parent.position == teleportPositions[index].position)
        {
            index = Random.Range(0, teleportPositions.Length);
        }

        transform.parent.position = teleportPositions[index].position;

        SoundFXManager.instance.PlaySoundFX(teleportSoundClip, transform, 0.2f);

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.parent.position, Quaternion.identity);
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
            knife.GetComponent<Rigidbody2D>().linearVelocity = dir * 5f;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
