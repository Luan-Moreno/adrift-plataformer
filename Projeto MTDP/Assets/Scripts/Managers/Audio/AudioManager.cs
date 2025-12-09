using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

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

    public void SetMasterVolume(float db)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(db) * 20f);
    }

    public void SetMusicVolume(float db)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(db) * 20f);
    }

    public void SetSFXVolume(float db)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(db) * 20f);
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
}
