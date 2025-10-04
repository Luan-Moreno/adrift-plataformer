using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator anim;

    private UIManager uiM;
    
    [SerializeField] private int initialSpeed;
    [SerializeField] private int speed;
    [SerializeField] private int jumpForce;
    private Vector2 direction;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isGrounded;
    private bool isJumping;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public int Speed { get => speed; set => speed = value; }
    public int InitialSpeed { get => initialSpeed; set => initialSpeed = value; }

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        IsMoving = false;
        isGrounded = false;
        isJumping = false;
        Speed = InitialSpeed;
    }
    void Update()
    {
        if (!uiM.PauseState)
        { 
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            IsMoving = direction.x != 0;
            Rotate();
            Run();
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!uiM.PauseState)
        {
            rig.linearVelocity = new Vector2(direction.x * Speed, rig.linearVelocity.y);
            if (isJumping)
            {
                rig.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                isJumping = false;
            }
        }
    }

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
            Speed = 15;
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
    }

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
}
