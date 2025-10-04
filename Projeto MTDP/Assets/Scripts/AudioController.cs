using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmMusic;
    [SerializeField] [Range(0f, 1f)] private float volume;
    private AudioManager audioM;
    void Start()
    {
        AudioManager.EnsureExists();
        audioM = FindAnyObjectByType<AudioManager>();
        audioM.PlayBGM(bgmMusic, volume);
    }
}