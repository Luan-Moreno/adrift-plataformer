using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip boatSFX;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f;
    public TMP_Text dialogueText;
    public float textFadeDuration = 2f;
    public float textStayDuration = 4f;

    void Start()
    {
        dialogueText.canvasRenderer.SetAlpha(0f);
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        AudioManager.instance.PlayTemporaryBGM(boatSFX, sfxVolume);

        yield return StartCoroutine(ShowText("O mar engole tudo que ousa cruzar suas águas..."));
        yield return StartCoroutine(ShowText("E naquela noite, um navio lutava contra seu destino."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowText("Mas nem todos que afundam estão destinados a morrer."));
        
        yield return AudioManager.instance.FadeVolume(AudioManager.instance.audioSource.volume, 0f, 0.5f);

        DataPersistenceManager.instance.ResetSaveData();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("AP01"); 
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
