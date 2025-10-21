using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    #region Variables
    public Transform point;
    public float radius;
    public LayerMask layer;
    [SerializeField] private bool isAttacking;
    [SerializeField] private int damage;
    [SerializeField] private int enemyHp;
    [SerializeField] private int maxEnemyHp;
    [SerializeField] private bool isDead;
    private Animator anim;
    private PlayerCombat playerCombat;
    private EnemyMovement enemyMovement;
    private Collider2D hit;

    public bool IsDead { get => isDead; set => isDead = value; }
    #endregion

    void Start()
    {
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        enemyHp = maxEnemyHp;
    }

    void FixedUpdate()
    {
        OnCollision();
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.25f);
        isAttacking = true;
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        enemyMovement.Speed = enemyMovement.InitialSpeed;
        enemyMovement.IsMoving = true;
    }

    #region Damage
    public void GiveDamage()
    {
        if (hit != null)
        {
            Debug.Log("Was Attacked! - Received " + damage + " damage!");
            if(!playerCombat.IsImmortal)
            {
                playerCombat.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHp -= damage;
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            isDead = true;
            gameObject.SetActive(false);
            Debug.Log("Enemy died!");
        }
    }
    #endregion

    public void Respawn()
    {
        isDead = false;
        enemyHp = maxEnemyHp;
        gameObject.SetActive(true);
    }
    
    #region Collision
    void OnCollision()
    {
        hit = Physics2D.OverlapCircle(point.position, radius, layer);
        if (hit != null && hit.CompareTag("Player") && !isAttacking)
        {
            enemyMovement.IsMoving = false;
            enemyMovement.Speed = 0f;
            StartCoroutine(Attack());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }
    #endregion
}
