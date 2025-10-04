using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private bool isColliding;
    [SerializeField] private int playerHp;
    [SerializeField] private int playerDamage;
    private float damageCooldown = 1f;
    private float lastDamageTime;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayer;
    private PlayerAnim playerAnim;
    private Animator anim;
    private UIManager uiM;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool isDead;

    public bool IsDead { get => isDead; set => isDead = value; }

    void Start()
    {
        playerHp = 5;
        playerDamage = 1;
        playerAnim = GetComponent<PlayerAnim>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        isColliding = false;
        IsDead = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("isAttacking");
            lastAttackTime = Time.time;
        }

        if (isColliding)
        {
            if (Time.time > lastDamageTime + damageCooldown)
            {
                TakeDamage(1);
                lastDamageTime = Time.time;
            }
        }
    }

    void TakeDamage(int damage)
    {
        playerHp -= damage;
        if (playerHp < 0)
        {
            playerHp = 0;
            uiM.gameOver.SetActive(true);
            IsDead = true;
            Time.timeScale = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isColliding = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }


    public void OnAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, radius, enemyLayer);

        if (hit != null)
        {
            Debug.Log("Acertou o inimigo!");
            //hit.GetComponentInChildren<SkeletonAnim>().OnHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
    
}
