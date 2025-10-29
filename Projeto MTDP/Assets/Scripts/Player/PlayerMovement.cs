using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Variables
    private Rigidbody2D rig;
    private Animator anim;
    private UIManager uiM;
    private PlayerCombat playerCombat;
    private TrailRenderer trailRenderer;

    [Header("Movimentation Variables")]
    [SerializeField] private float initialSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float coyoteTime;
    [SerializeField] private bool isJumpBuffer;
    [SerializeField] private float dashVelocity;
    [SerializeField] private float dashDuration;
    private Vector2 direction;
    private Vector2 dashDirection;

    [Header("Checks")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool wallHit;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isCoyoteGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash;
    private float facing;
    private bool isFacingRight;
    private bool isFacingLeft;
    private Coroutine coyoteCoroutine;

    [Header("Ground Checks")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform groundCheckerPoint;
    [SerializeField] private float groundCheckerRadius;
    [SerializeField] private Transform jumpBufferPoint;
    [SerializeField] private float jumpBufferRadius;

    private Collider2D detectGround;
    private Collider2D detectGroundBuffer;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float Speed { get => speed; set => speed = value; }
    public float InitialSpeed { get => initialSpeed; set => initialSpeed = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsFacingRight { get => isFacingRight; set => isFacingRight = value; }
    public bool IsFacingLeft { get => isFacingLeft; set => isFacingLeft = value; }

    #endregion

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        playerCombat = GetComponent<PlayerCombat>();
        trailRenderer = GetComponent<TrailRenderer>();
        IsMoving = false;
        IsGrounded = false;
        isJumping = false;
        isDashing = false;
        canDash = false;
        Speed = InitialSpeed;
        facing = 1f;
    }
    void Update()
    {
        if (!uiM.PauseState && !playerCombat.IsDead && speed != 0)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            IsMoving = direction.x != 0;
            GroundCheck();
            Rotate();
            Run();
            Jump();
            JumpBuffer();
            Dash();
        }
    }
    void FixedUpdate()
    {
        if (!uiM.PauseState)
        {
            if (!isDashing)
            {
                rig.linearVelocity = new Vector2(direction.x * Speed, rig.linearVelocity.y);
            }
            if (isJumping && isCoyoteGrounded)
            {
                isJumping = false;
                IsGrounded = false;
                isCoyoteGrounded = false;
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
            }
        }
        else
        {
            rig.linearVelocity = new Vector2(0f, 0f);
        }
    }

    #region Movement
    void Rotate()
    {
        if (direction.x > 0)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            IsFacingRight = true;
            IsFacingLeft = false;
            facing = 1f;
            
        }
        if (direction.x < 0)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            IsFacingRight = false;
            IsFacingLeft = true;
            facing = -1f;
        }
    }

    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && IsGrounded)
        {
            Speed = InitialSpeed * 1.5f;
        }
        else
        {
            Speed = InitialSpeed;
        }
    }

    void Jump()
    {
        if ((isCoyoteGrounded || isJumpBuffer) && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            isJumpBuffer = false;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rig.linearVelocity.y > 0)
        {
            rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0f);
        }
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;

            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashDirection == Vector2.zero)
            {
                dashDirection = new Vector2(facing, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            isCoyoteGrounded = false;
            dashDirection = dashDirection.normalized;
            dashDirection.x *= 0.9f;
            dashDirection.y *= 0.75f;
            rig.linearVelocity = dashDirection * dashVelocity;
        }

        if (IsGrounded && !isDashing)
        {
            isCoyoteGrounded = true;
            canDash = true;
        }
    }

    IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDuration);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        isCoyoteGrounded = false;
    }

    #endregion

    #region Collision

    void GroundCheck()
    {
        detectGround = Physics2D.OverlapCircle(groundCheckerPoint.position, groundCheckerRadius, groundLayer);
        if (detectGround != null)
        {
            IsGrounded = true;
            isCoyoteGrounded = IsGrounded;
            
            if (coyoteCoroutine != null)
            {
                StopCoroutine(coyoteCoroutine);
                coyoteCoroutine = null;
            }
        }
        else
        {
            IsGrounded = false;

            if(coyoteCoroutine == null)
            {
                coyoteCoroutine = StartCoroutine(CoyoteTime());
            }
        }
    }

    void JumpBuffer()
    {
        detectGroundBuffer = Physics2D.OverlapCircle(jumpBufferPoint.position, jumpBufferRadius, groundLayer);
        isJumpBuffer = detectGroundBuffer != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckerPoint.position, groundCheckerRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(jumpBufferPoint.position, jumpBufferRadius);
    }

    #endregion

    public void SaveData(ref GameData data)
    {
        if(!data.firstBonfire)
        {
            data.playerPosition = data.firstSpawnpoint;
        }
    }
    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }
}
