using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Life/HP")]
    [SerializeField] private int playerHp;
    [SerializeField] private bool isColliding;
    [SerializeField] private bool isImmortal;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isCharging;

    [Header("Damage/Attack")]
    [SerializeField] private bool isStrongAttack;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.5f;
    private float damageCooldown = 1f;
    private float lastDamageTime;
    private float chargeStartTime;
    [SerializeField] private float strongAttackHoldTime;
    private float lastAttackTime;
    private int playerDamage;
    
    private Animator anim;
    private UIManager uiM;

    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsCharging { get => isCharging; set => isCharging = value; }
    public bool IsStrongAttack { get => isStrongAttack; set => isStrongAttack = value; }

    void Start()
    {
        playerHp = 5;
        playerDamage = 1;
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        isColliding = false;
        IsDead = false;
    }
    void Update()
    {
        if (!uiM.PauseState && !isDead)
        { 
        if (Input.GetMouseButtonDown(0) && Time.time > lastAttackTime + attackCooldown)
        {
            IsCharging = true;
            chargeStartTime = Time.time;
            anim.SetBool("isCharging", true);
        }

        if (Input.GetMouseButtonUp(0) && IsCharging)
        {
            IsCharging = false;
            float heldTime = Time.time - chargeStartTime;

            if (heldTime < strongAttackHoldTime)
            {
                IsStrongAttack = false;
                anim.SetBool("isCharging", false);
                anim.SetTrigger("isAttacking");
                Debug.Log("Ataque Normal!");
            }
            else
            {
                IsStrongAttack = true;
                anim.SetBool("isCharging", false);
                anim.SetTrigger("isStrongAttacking");
                Debug.Log("Ataque Forte!");
            }

            lastAttackTime = Time.time;
        }

            if (isColliding)
            {
                if (Time.time > lastDamageTime + damageCooldown && !isImmortal)
                {
                    TakeDamage(1);
                    lastDamageTime = Time.time;
                }
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
            int damage = IsStrongAttack ? 2 : 1;
            Debug.Log("Acertou o inimigo! - Causou " + damage + " de dano!");
            //hit.GetComponentInChildren<SkeletonAnim>().OnHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
    
}
