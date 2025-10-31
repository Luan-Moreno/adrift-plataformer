using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SpawnPoint : MonoBehaviour
{
    private SequenceManager sequenceManager;

    void Start()
    {
        sequenceManager = FindAnyObjectByType<SequenceManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sequenceManager.spawnPoint = transform.position;
        }
    }
}
