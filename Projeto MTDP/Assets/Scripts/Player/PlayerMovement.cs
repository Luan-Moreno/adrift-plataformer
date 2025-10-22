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
    [SerializeField] private float dashVelocity;
    [SerializeField] private float dashDuration;
    private Vector2 direction;
    private Vector2 dashDirection;

    [Header("Checks")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash;
    private float facing;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float Speed { get => speed; set => speed = value; }
    public float InitialSpeed { get => initialSpeed; set => initialSpeed = value; }

    #endregion

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        playerCombat = GetComponent<PlayerCombat>();
        trailRenderer = GetComponent<TrailRenderer>();
        IsMoving = false;
        isGrounded = false;
        isJumping = false;
        isDashing = false;
        canDash = false;
        Speed = InitialSpeed;
    }
    void Update()
    {
        if (!uiM.PauseState && !playerCombat.IsDead && speed != 0)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            IsMoving = direction.x != 0;
            Rotate();
            Run();
            Jump();
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
            if (isJumping && isGrounded)
            {
                isJumping = false;
                isGrounded = false;
                rig.linearVelocity = new Vector2(rig.linearVelocity.x, jumpForce);
            }
        }
    }

    #region Movement
    void Rotate()
    {
        if (direction.x > 0)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        if (direction.x < 0)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            Speed = InitialSpeed * 1.75f;
        }
        else
        {
            Speed = InitialSpeed;
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
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

            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), (Input.GetAxisRaw("Vertical") / 3));
            if (dashDirection == Vector2.zero)
            {
                facing = transform.eulerAngles.y == 0f ? 1f : -1f;
                dashDirection = new Vector2(facing, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rig.linearVelocity = dashDirection.normalized * dashVelocity;
        }

        if (isGrounded && !isDashing)
        {
            canDash = true;
        }
    }

    IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDuration);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    #endregion
    
    #region Collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
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
