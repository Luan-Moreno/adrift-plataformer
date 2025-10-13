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
            playerCombat.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHp -= damage;
        if (enemyHp <= 0)
        {
            enemyHp = 0;
            isDead = true;
            Debug.Log("The enemy died!");
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Collision
    void OnCollision()
    {
        hit = Physics2D.OverlapCircle(point.position, radius, layer);
        if (hit != null & !isAttacking)
        {
            enemyMovement.IsMoving = false;
            enemyMovement.Speed = 0f;
            StartCoroutine("Attack");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }
    #endregion
}
