using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rig;
    [SerializeField] private float initialSpeed;
    private float speed;
    [SerializeField] private Transform point;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layer;
    private Vector3 facingRight;
    private Vector3 facingLeft;
    private bool isRotating;
    private Animator anim;
    private UIManager uiM;
    private EnemyCombat enemyCombat;
    private bool isMoving;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float Speed { get => speed; set => speed = value; }
    public float InitialSpeed { get => initialSpeed; set => initialSpeed = value; }

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        enemyCombat = GetComponent<EnemyCombat>();
        facingRight = new Vector3(0f, 0f, 0f);
        facingLeft = new Vector3(0f, 180f, 0f);
        isRotating = false;
        isMoving = true;
        speed = initialSpeed;
    }

    void Update()
    {
        if (!uiM.PauseState && !enemyCombat.IsDead)
        {
            //isMoving = rig.linearVelocity.magnitude > 0.1f;
        }
    }

    void FixedUpdate()
    {
        if (!uiM.PauseState && !enemyCombat.IsDead)
        { 
            rig.linearVelocity = new Vector2(Speed, rig.linearVelocityY);
            OnCollision();
        }
    }

    void OnCollision()
    {
        Collider2D hit = Physics2D.OverlapCircle(point.position, radius, layer);
        if (hit != null && !isRotating)
        {
            StartCoroutine("Rotate");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }

    private IEnumerator Rotate()
    {
        isRotating = true;
        if (transform.eulerAngles.y == 0)
        {
            transform.eulerAngles = facingLeft;
            Speed = -Mathf.Abs(Speed);
            InitialSpeed = -Mathf.Abs(InitialSpeed);
        }
        else
        {
            transform.eulerAngles = facingRight;
            Speed = Mathf.Abs(Speed);
            InitialSpeed = Mathf.Abs(InitialSpeed);
        }
        yield return new WaitForSeconds(1);
        isRotating = false;
    }
}
