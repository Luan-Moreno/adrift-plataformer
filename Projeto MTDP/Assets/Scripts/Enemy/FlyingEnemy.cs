using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    private GameObject player;
    private Vector3 facingRight;
    private Vector3 facingLeft;
    public bool chase;
    public Transform startingPoint;
    public float speed = 5f;
    public float stoppingDistance = 1.5f;
    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        facingRight = Vector3.zero;
        facingLeft = new Vector3(0f, 180f, 0f);
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (startingPoint == null) startingPoint = transform;
        chase = false;
    }

    void Update()
    {
        if (player == null) 
        {
            return;
        }

        Vector3 target = chase ? player.transform.position : startingPoint.position;
        float distance = Vector2.Distance(transform.position, target);
        
        if(target == startingPoint.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        else
        {
            if (distance > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            } 
        }

        if (distance > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
            
        Flip();
    }

    private void Flip()
    {
        Vector3 target = chase ? player.transform.position : startingPoint.position;

        if (transform.position.x > target.x)
        {
            transform.eulerAngles = facingRight;
        }
            
        else
        {
            transform.eulerAngles = facingLeft;
        }
    }


}
