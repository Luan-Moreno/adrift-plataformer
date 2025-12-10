using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject boss;
    public AudioClip bossMusic;
    public float fadeDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) 
        {
            return;
        }

        if(boss != null)
        {
            boss.SetActive(true);
        }
        MusicManager.instance.PlayTemporaryBGM(bossMusic, fadeDuration);
        Destroy(this);
    }
}
