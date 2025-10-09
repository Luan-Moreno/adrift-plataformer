using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private Rigidbody2D rig;
    public float speed;
    public Transform point;
    public float radius;
    public LayerMask layer;
    private Vector3 facingRight;
    private Vector3 facingLeft;
    private bool isRotating;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        facingRight = new Vector3(0f, 0f, 0f);
        facingLeft = new Vector3(0f, 180f, 0f);
        isRotating = false;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(speed, rig.linearVelocityY);
        OnCollision();
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
            speed = -Mathf.Abs(speed);
        }
        else
        {
            transform.eulerAngles = facingRight;
            speed = Mathf.Abs(speed);
        }
        yield return new WaitForSeconds(1);
        isRotating = false;
    }
}
