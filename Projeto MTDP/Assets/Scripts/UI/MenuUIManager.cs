using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    #region Variables
    public GameObject fade;
    private Image fadePanel;
    public float fadeDuration = 1f;
    public GameObject settings;
    [SerializeField] private bool settingsState = false;

    private SequenceManager sequenceManager;

    #endregion

    void Start()
    {
        sequenceManager = FindAnyObjectByType<SequenceManager>();
        fadePanel = fade.transform.Find("FadePanel").GetComponent<Image>();
        fade.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(FadeToGame(0, 1));
    }

    public void GoToSettings()
    {
        settingsState = !settingsState;
        settings.SetActive(settingsState);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public IEnumerator FadeToGame(float startAlpha, float endAlpha, string sceneName = "InitialScene")
    {
        fade.SetActive(true);
        float time = 0;
        Color color = fadePanel.color;
        color.a = startAlpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }
        color.a = endAlpha;
        fadePanel.color = color;
        yield return new WaitForSeconds(fadeDuration / 2);
        ChangeScene(sceneName);
    }
    
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
