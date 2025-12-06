using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmMusic;

    void Start()
    {
        AudioManager audioM = AudioManager.instance;
        MusicManager musicM = MusicManager.instance;
        if (audioM == null)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Audio/AudioManager"));
            audioM = prefab.GetComponent<AudioManager>();
        }
        if (musicM == null)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Audio/MusicManager"));
            musicM = prefab.GetComponent<MusicManager>();
        }
        musicM.PlayBGM(bgmMusic);
    }
}
