using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private AudioClip boatSFX;
    private float sfxVolume = -40f;
    public TMP_Text dialogueText;
    public float textFadeDuration = 2f;
    public float textStayDuration = 4f;

    void Start()
    {
        dialogueText.canvasRenderer.SetAlpha(0f);
        StartCoroutine(EndingSequence());
    }

    IEnumerator EndingSequence()
    {
        MusicManager.instance.PlayTemporaryBGM(boatSFX);

        yield return StartCoroutine(ShowText("The darkness fades… for now."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowText("Thanks for testing this pre-alpha build."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ShowText("The real journey is still far ahead."));

        yield return AudioManager.instance.FadeMixer("MusicVolume", sfxVolume, -80f, 1f);
        yield return new WaitForSeconds(0.5f);

        yield return AudioManager.instance.FadeMixer("MusicVolume", -80f, sfxVolume, 0.001f);
        SceneManager.LoadScene("MainMenu");
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
