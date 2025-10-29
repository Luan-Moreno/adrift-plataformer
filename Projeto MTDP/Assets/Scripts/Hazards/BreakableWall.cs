using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BreakableWall : MonoBehaviour
{
    public int hitsToBreak = 3;
    public float knockbackDistance = 0.1f;
    public float knockbackDuration = 0.05f;
    public ParticleSystem hitParticles;
    private int currentHits = 0;
    private Vector3 originalPosition;
    public GameObject room;
    private SecretRoom secretRoom;

    void Awake()
    {
        originalPosition = transform.position;
    }

    void Start()
    {
        secretRoom = room.GetComponent<SecretRoom>();
    }
    
    public void TakeHit()
    {
        currentHits++;

        if (hitParticles != null)
        {
            hitParticles.Play();
        }

        StopAllCoroutines();
        StartCoroutine(Knockback());

        if (currentHits >= hitsToBreak)
        {
            secretRoom.RevealRoom();
            Destroy(gameObject);
        }
            
    }

    private IEnumerator Knockback()
    {
        Vector3 targetPos = originalPosition - new Vector3(knockbackDistance, 0, 0);
        float timer = 0f;
        while (timer < knockbackDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPos, timer / knockbackDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
