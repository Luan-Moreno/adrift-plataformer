using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    private AudioClip previousClip;
    private float previousVolume;

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

    public IEnumerator FadeVolume(float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }
        audioSource.volume = end;
    }

    public void PlayTemporaryBGM(AudioClip newClip, float volume, float fadeDuration = 0.5f)
    {
        if (newClip == null) return;

        previousClip = audioSource.clip;
        previousVolume = audioSource.volume;

        StopAllCoroutines();
        StartCoroutine(SwapToTemporary(newClip, volume, fadeDuration));
    }

    private IEnumerator SwapToTemporary(AudioClip newClip, float volume, float fadeDuration)
    {
        yield return FadeVolume(audioSource.volume, 0f, fadeDuration);
        PlayBGM(newClip, volume);
        yield return FadeVolume(0f, volume, fadeDuration);
    }

    public void RestorePreviousBGM(float fadeDuration = 0.5f)
    {
        if (previousClip == null) return;

        StopAllCoroutines();
        StartCoroutine(SwapBackToPrevious(fadeDuration));
    }

    private IEnumerator SwapBackToPrevious(float fadeDuration)
    {
        yield return FadeVolume(audioSource.volume, 0f, fadeDuration);
        PlayBGM(previousClip, previousVolume);
        yield return FadeVolume(0f, previousVolume, fadeDuration);
    }
}