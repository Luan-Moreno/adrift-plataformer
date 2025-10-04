using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Garante que sempre tem AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void EnsureExists()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("AudioManager");
            instance = go.AddComponent<AudioManager>();
        }
    }

    public void PlayBGM(AudioClip audio, float volume, bool loop = true)
    {
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }
}