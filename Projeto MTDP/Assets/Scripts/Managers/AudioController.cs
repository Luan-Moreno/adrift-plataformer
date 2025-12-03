using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmMusic;

    void Start()
    {
        AudioManager audioM = FindAnyObjectByType<AudioManager>();
        if (audioM == null)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Managers/AudioManager"));
            audioM = prefab.GetComponent<AudioManager>();
        }
        audioM.PlayBGM(bgmMusic);
    }
}
