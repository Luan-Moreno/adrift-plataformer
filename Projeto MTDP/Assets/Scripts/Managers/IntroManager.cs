using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NUnit.Framework.Constraints;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private AudioClip boatSFX;
    private float sfxVolume = -40f;
    public TMP_Text dialogueText;
    public float textFadeDuration = 2f;
    public float textStayDuration = 4f;

    void Start()
    {
        dialogueText.canvasRenderer.SetAlpha(0f);
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        AudioManager.instance.audioMixer.GetFloat("MasterVolume", out float current);
        AudioManager.instance.PlayTemporaryBGM(boatSFX);

        yield return StartCoroutine(ShowText("The sea swallows everything that dares to cross its waters..."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowText("And that night, a ship was fighting against its destiny."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ShowText("But not everyone who sinks is destined to die."));

        yield return AudioManager.instance.FadeMixer("MasterVolume", sfxVolume, -80f, 1f);

        DataPersistenceManager.instance.ResetSaveData();
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.audioSource.clip = null;

        SceneManager.LoadScene("AP01");
        AudioManager.instance.SetBGMVolume(current);
    }

    IEnumerator ShowText(string text)
    {
        dialogueText.text = text;

        dialogueText.CrossFadeAlpha(1f, textFadeDuration, false);
        yield return new WaitForSeconds(textFadeDuration + textStayDuration);

        dialogueText.CrossFadeAlpha(0f, textFadeDuration, false);
        yield return new WaitForSeconds(textFadeDuration);
    }
}
