using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class EnemyCombat : MonoBehaviour
{
    #region Variables
    public Transform point;
    public float radius;
    public LayerMask layer;
    [SerializeField] protected bool isAttacking;
    [SerializeField] private int damage;
    [SerializeField] private int enemyHp;
    [SerializeField] private int maxEnemyHp;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;
    [SerializeField] private Color flashColor;
    [SerializeField] private bool isDead;
    private Animator anim;
    private PlayerCombat playerCombat;
    private EnemyMovement enemyMovement;
    private Collider2D hit;
    private CinemachineImpulseSource impulseSource;
    private Vector3 initialPosition;

    public bool IsDead { get => isDead; set => isDead = value; }
    #endregion

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        enemyHp = maxEnemyHp;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
        initialPosition = gameObject.transform.position;
    }

    protected void FixedUpdate()
    {
        OnCollision();
    }

    private IEnumerator Attack()
    {
        //TEMPORÁRIO
        GiveDamage();
        yield return new WaitForSeconds(0.25f);
        isAttacking = true;
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        if(enemyMovement != null)
        {
            enemyMovement.Speed = enemyMovement.InitialSpeed;
            enemyMovement.IsMoving = true;
        }
    }

    #region Damage
    public void GiveDamage()
    {
        if (hit != null)
        {
            if(!playerCombat.IsImmortal)
            {
                playerCombat.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHp -= damage;
        if (CameraManager.instance.CanShake)
        {
            CameraManager.instance.CameraShake(impulseSource);
        }
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            isDead = true;
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
    }
    
    public void TakeHighDamage(int damage)
    {
        enemyHp -= damage;
        if(CameraManager.instance.CanShake)
        {
            CameraManager.instance.StrongCameraShake(impulseSource); 
        }
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            isDead = true;
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = normalColor;
    }

    #endregion

    public void Respawn()
    {
        isDead = false;
        enemyHp = maxEnemyHp;
        gameObject.transform.position = initialPosition;
        gameObject.SetActive(true);
    }
    
    #region Collision
    void OnCollision()
    {
        hit = Physics2D.OverlapCircle(point.position, radius, layer);
        if (hit != null && hit.CompareTag("Player") && !isAttacking)
        {
            if(enemyMovement != null)
            {
                enemyMovement.IsMoving = false;
                enemyMovement.Speed = 0f;
            }
            StartCoroutine(Attack());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }
    #endregion
}
