using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SpawnPoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SequenceManager.instance.spawnPoint = transform.position;
        }
    }
}
