using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;

    private AudioClip previousClip;

    private void Awake()
{
    if (instance == null)
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (masterGroup != null)
        {
            audioSource.outputAudioMixerGroup = masterGroup;
        }
    }
    else
    {
        Destroy(gameObject);
        return;
    }
}

    public void PlayBGM(AudioClip audio, bool loop = true)
    {
        audioSource.clip = audio;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void SetBGMVolume(float db)
    {
        audioMixer.SetFloat("MasterVolume", db);
    }

    public IEnumerator FadeMixer(string parameter, float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Lerp(from, to, time / duration);
            audioMixer.SetFloat(parameter, value);
            yield return null;
        }
        audioMixer.SetFloat(parameter, to);
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
        audioMixer.GetFloat("MasterVolume", out float current);

        // fade out
        yield return FadeMixer("MasterVolume", current, -80f, fadeDuration);

        PlayBGM(newClip);

        // fade in
        yield return FadeMixer("MasterVolume", -80f, current, fadeDuration);
    }

    public void RestorePreviousBGM(float fadeDuration = 0.5f)
    {
        if (previousClip == null) return;

        StopAllCoroutines();
        StartCoroutine(SwapBackToPrevious(fadeDuration));
    }

    private IEnumerator SwapBackToPrevious(float fadeDuration)
    {
        audioMixer.GetFloat("MasterVolume", out float current);

        // fade out
        yield return FadeMixer("MasterVolume", current, -80f, fadeDuration);

        PlayBGM(previousClip);

        // fade in
        yield return FadeMixer("MasterVolume", -80f, current, fadeDuration);
    }
}
