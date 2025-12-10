using UnityEngine;

public class KnifeBehaviour : MonoBehaviour
{
    public int damage = 1;
    public float speed = 5f;
    private Vector2 direction;

    private void Start()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        direction = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, 1.5f); 
    }

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            if (playerCombat != null)
            {
                playerCombat.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
