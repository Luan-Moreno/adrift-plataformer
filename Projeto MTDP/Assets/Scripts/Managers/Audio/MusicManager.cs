using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public AudioSource audioSource;
    public AudioMixer audioMixer;
    [SerializeField] private AudioClip currentClip;
    private AudioClip previousClip;

    public void PlayBGM(AudioClip audio, bool loop = true)
    {
        audioSource.clip = audio;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void PlayTemporaryBGM(AudioClip newClip, float fadeDuration = 0.5f)
    {
        if (newClip == null) return;

        previousClip = audioSource.clip;

        StopAllCoroutines();
        StartCoroutine(SwapToTemporary(newClip, fadeDuration));
    }

    private IEnumerator SwapToTemporary(AudioClip newClip, float fadeDuration)
    {
        audioMixer.GetFloat("MusicVolume", out float current);

        // fade out
        yield return AudioManager.instance.FadeMixer("MusicVolume", current, -80f, fadeDuration);

        PlayBGM(newClip);

        // fade in
        yield return AudioManager.instance.FadeMixer("MusicVolume", -80f, current, fadeDuration);
    }

    public void RestorePreviousBGM(float fadeDuration = 0.5f)
    {
        if (previousClip == null) return;

        StopAllCoroutines();
        StartCoroutine(SwapBackToPrevious(fadeDuration));
    }

    private IEnumerator SwapBackToPrevious(float fadeDuration)
    {
        audioMixer.GetFloat("MusicVolume", out float current);

        // fade out
        yield return AudioManager.instance.FadeMixer("MusicVolume", current, -80f, fadeDuration);

        PlayBGM(previousClip);

        // fade in
        yield return AudioManager.instance.FadeMixer("MusicVolume", -80f, current, fadeDuration);
    }
}
