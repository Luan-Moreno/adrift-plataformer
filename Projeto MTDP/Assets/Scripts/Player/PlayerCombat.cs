using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, IDataPersistence
{
    #region Variables
    private PlayerMovement playerMovement;
    private Animator anim;
    private UIManager uiM;
    private SequenceManager sequenceManager;
    private Collider2D hit;

    [Header("Life/HP")]
    [SerializeField] private int currentHp;
    [SerializeField] private int maxHp;
    [SerializeField] private bool isImmortal;
    [SerializeField] private float invulnerableTime = 1f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color flashColor;
    
    [SerializeField] private bool isDead;
    [SerializeField] private bool nearBonfire;
    private GameObject bonfire;
    private bool isColliding;

    [Header("Damage/Attack")]
    [SerializeField] private bool isStrongAttack;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float strongAttackHoldTime;

    [Header("Attack Points")]
    [SerializeField] private Transform currentAttackPoint;
    [SerializeField] private Transform attackPointForward;
    [SerializeField] private Transform attackPointUp;
    [SerializeField] private Transform attackPointDown;

    private bool isCharging;
    private int playerDamage;
    private float damageCooldown = 1f;
    private float lastDamageTime;
    private float chargeStartTime;
    private float lastAttackTime;
    [SerializeField] private bool meleeAttack;
    [SerializeField] private bool isRespawning;
    
    [Header("Attack Movement")]
    public float defaultForce = 30;
    public float upwardsForce = 5;
    public float movementTime = 0.1f;
    private Vector2 direction;
    private bool collided;
    private bool downwardStrike;
    private Rigidbody2D rig;

    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsImmortal { get => isImmortal; set => isImmortal = value; }
    public bool IsCharging { get => isCharging; set => isCharging = value; }
    public bool IsStrongAttack { get => isStrongAttack; set => isStrongAttack = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int CurrentHp { get => currentHp; set => currentHp = value; }
    public GameObject Bonfire { get => bonfire; set => bonfire = value; }
    public bool IsRespawning { get => isRespawning; set => isRespawning = value; }


    #endregion
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        sequenceManager = FindAnyObjectByType<SequenceManager>();
        Bonfire = gameObject;
        isColliding = false;
        IsDead = false;
        uiM.gameOver.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = Instantiate(spriteRenderer.material);
        normalColor = spriteRenderer.material.GetColor("_Color");
        flashColor = Color.red;
    }
    void Update()
    {
        if (!uiM.PauseState && !isDead)
        {
            Attack();
            CollisionDamage();
        }

        BonfireDetector();
    }

    void FixedUpdate()
    {
        HandleAttackMovement();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            meleeAttack = true;
        }
        else
        {
            meleeAttack = false;
        }

        if (meleeAttack && Input.GetAxis("Vertical") > 0)
        {
            currentAttackPoint = attackPointUp;
            anim.SetTrigger("isUpwardAttacking");
        }

        if (meleeAttack && Input.GetAxis("Vertical") < 0 && !playerMovement.IsGrounded)
        {
            currentAttackPoint = attackPointDown;
            anim.SetTrigger("isDownwardAttacking");
        }

        if ((meleeAttack && Input.GetAxis("Vertical") == 0) ||
        meleeAttack && Input.GetAxis("Vertical") < 0 && playerMovement.IsGrounded)
        {
            currentAttackPoint = attackPointForward;
            anim.SetTrigger("isForwardAttacking");
        }

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
            anim.SetBool("isCharging", false);

            if (heldTime < strongAttackHoldTime)
            {
                IsStrongAttack = false;
            }
            else
            {
                IsStrongAttack = true;
                anim.SetTrigger("isStrongAttacking");
            }

            lastAttackTime = Time.time;
        }
    }

    #region Damage/Healing

    public void TakeDamage(int damage)
    {
        if (isImmortal || IsDead || UIManager.instance.PauseState) return;

        currentHp -= damage;
        uiM.UpdateHearts();
        if (currentHp <= 0)
        {
            currentHp = 0;
            uiM.fade.SetActive(false);
            uiM.gameOver.SetActive(true);
            IsDead = true;
            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(Invulnerability());
        }
    }

    private IEnumerator Invulnerability()
    {
        isImmortal = true;

        float elapsed = 0f;
        float flashSpeed = 0.3f;

        while (elapsed < invulnerableTime)
        {
            spriteRenderer.material.SetColor("_Color", flashColor);
            yield return new WaitForSeconds(flashSpeed);
            spriteRenderer.material.SetColor("_Color", normalColor);
            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed * 2f;
        }

        spriteRenderer.material.SetColor("_Color", normalColor);
        isImmortal = false;
    }

    void GiveDamage(GameObject target, int damage)
    {
        EnemyCombat enemyCombat = target.GetComponent<EnemyCombat>();
        if (IsStrongAttack)
        {
            enemyCombat.TakeHighDamage(damage);
        }
        else
        {
            enemyCombat.TakeDamage(damage);
        }
        
    }

    public void ReceiveHealing(int healing)
    {
        currentHp += healing;
        uiM.UpdateHearts();
        anim.SetTrigger("isHealing");
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    void CollisionDamage()
    {
        if (isColliding)
        {
            if (Time.time > lastDamageTime + damageCooldown && !IsImmortal)
            {
                TakeDamage(1);
                lastDamageTime = Time.time;
            }
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
        if (collider.gameObject.CompareTag("Bonfire"))
        {
            nearBonfire = true;
            Bonfire = collider.gameObject;
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
        hit = Physics2D.OverlapCircle(currentAttackPoint.position, radius, enemyLayer);
        if (hit != null && !UIManager.instance.PauseState)
        {
            if (hit.CompareTag("Hazard"))
            {
                upwardsForce = 6;
                HandleAttackCollision(hit.GetComponentInChildren<HazardBehaviour>());
                StartCoroutine(HitPause());
            }

            if (hit.CompareTag("Enemy"))
            {
                upwardsForce = 3;
                HandleAttackCollision(hit.GetComponentInChildren<HazardBehaviour>());

                playerDamage = IsStrongAttack ? 2 : 1;
                GiveDamage(hit.gameObject, playerDamage);
                StartCoroutine(HitPause());
            }

            BreakableWall wall = hit.GetComponent<BreakableWall>();
            if (wall != null)
            {
                wall.TakeHit();
                StartCoroutine(HitPause());
            }
        }
    }
    
    private IEnumerator HitPause()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1f;
    }

    private void HandleAttackCollision(HazardBehaviour hazardObj)
    {
        if (hazardObj.giveUpwardForce && Input.GetAxis("Vertical") < 0 && !playerMovement.IsGrounded)
        {
            direction = Vector2.up;
            downwardStrike = true;
            collided = true;
        }

        if (Input.GetAxis("Vertical") > 0 && !playerMovement.IsGrounded)
        {
            direction = Vector2.down;
            downwardStrike = true;
            collided = true;
        }

        if ((Input.GetAxis("Vertical") <= 0 && playerMovement.IsGrounded) || Input.GetAxis("Vertical") == 0)
        {
            if (playerMovement.IsFacingLeft)
            {
                direction = Vector2.right;
            }
            else
            {
                direction = Vector2.left;
            }
            collided = true;
        }
        if(upwardsForce == 5)
        {
            hazardObj.Damage(playerDamage);
        }
        StartCoroutine(NoLongerColliding());
    }

    private void HandleAttackMovement()
    {
        if (collided && !UIManager.instance.PauseState)
        {
            if (downwardStrike)
            {
                rig.linearVelocity = new Vector2(rig.linearVelocityX, 0f);
                rig.AddForce(direction * upwardsForce, ForceMode2D.Impulse);
            }
            else
            {
                rig.AddForce(direction * defaultForce);
            }
        }
    }
    
    private IEnumerator NoLongerColliding()
    {
        yield return new WaitForSeconds(movementTime);
        collided = false;
        downwardStrike = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPointForward.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPointUp.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPointDown.position, radius);
    }

    #endregion

    #region Load/Save

    void BonfireDetector()
    {
        if (nearBonfire && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(sequenceManager.RestSequence());
        }
    }

    public void SaveData(ref GameData data)
    {
        data.maxHp = this.maxHp;
        data.playerDamage = this.playerDamage;
    }
    
    public void LoadData(GameData data)
    {
        this.currentHp = data.maxHp;
        this.maxHp = data.maxHp;
        this.playerDamage = data.playerDamage;
    }

    #endregion
    
}
