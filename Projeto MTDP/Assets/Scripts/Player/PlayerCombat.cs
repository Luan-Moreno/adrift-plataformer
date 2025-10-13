using UnityEngine;

public class PlayerCombat : MonoBehaviour, IDataPersistence
{
    #region Variables

    [Header("Life/HP")]
    [SerializeField] private int currentHp;
    [SerializeField] private int maxHp;
    private bool isColliding;
    [SerializeField] private bool isImmortal;
    [SerializeField] private bool isDead;
    private bool isCharging;
    [SerializeField] private bool nearBonfire;

    [Header("Damage/Attack")]
    [SerializeField] private bool isStrongAttack;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float strongAttackHoldTime;
    private int playerDamage;
    private float damageCooldown = 1f;
    private float lastDamageTime;
    private float chargeStartTime;
    private float lastAttackTime;
    private GameObject bonfire;

    private Animator anim;
    private UIManager uiM;
    private SequenceManager sequenceManager;
    private Collider2D hit;

    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsCharging { get => isCharging; set => isCharging = value; }
    public bool IsStrongAttack { get => isStrongAttack; set => isStrongAttack = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int CurrentHp { get => currentHp; set => currentHp = value; }
    public GameObject Bonfire { get => bonfire; set => bonfire = value; }

    #endregion
    void Start()
    {
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        sequenceManager = FindAnyObjectByType<SequenceManager>();
        Bonfire = gameObject;
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
                }
                else
                {
                    IsStrongAttack = true;
                    anim.SetBool("isCharging", false);
                    anim.SetTrigger("isStrongAttacking");
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

        if (nearBonfire && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(sequenceManager.RestSequence());
        }

    }

    #region Damage/Healing

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
            uiM.fade.SetActive(false);
            uiM.gameOver.SetActive(true);
            IsDead = true;
            Time.timeScale = 0f;
        }
    }

    void GiveDamage(GameObject target, int damage)
    {
        EnemyCombat enemyCombat = target.GetComponent<EnemyCombat>();
        Debug.Log("Enemy hit! - Caused " + damage + " damage!");
        enemyCombat.TakeDamage(damage);
    }

    public void ReceiveHealing(int healing)
    {
        currentHp += healing;
        anim.SetTrigger("isHealing");
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    #endregion

    #region Collision/Trigger

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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Healing"))
        {
            ReceiveHealing(1);
            Destroy(collider.gameObject);
        }

        if (collider.gameObject.CompareTag("Bonfire"))
        {
            Bonfire = collider.gameObject;
            nearBonfire = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Bonfire"))
        {
            nearBonfire = false;
            Bonfire = gameObject;
        }
    }

    public void OnAttack()
    {
        hit = Physics2D.OverlapCircle(attackPoint.position, radius, enemyLayer);

        if (hit != null)
        {
            playerDamage = IsStrongAttack ? 2 : 1;
            GiveDamage(hit.gameObject, playerDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }

    #endregion

    #region Load/Save

    public void SaveData(ref GameData data)
    {
        data.maxHp = this.maxHp;
        data.playerDamage = this.playerDamage;
    }
    
    public void LoadData(GameData data)
    {
        this.maxHp = data.maxHp;
        this.playerDamage = data.playerDamage;
    }

    #endregion
    
}
