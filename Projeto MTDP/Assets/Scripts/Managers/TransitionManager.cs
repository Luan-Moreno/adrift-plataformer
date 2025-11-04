using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;
    private bool isTransitioning = false;

    private UIManager uiM;
    private Image fadePanel;
    public float fadeDuration = 1f;
    private string nextScene;

    private void Start()
    {
        uiM = FindAnyObjectByType<UIManager>();
        FindFadePanel();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void FindFadePanel()
    {
        GameObject fade = GameObject.FindWithTag("Fade");
        if (fade != null)
        {
            Debug.Log("Achou Fade");
            fadePanel = fade.transform.Find("FadePanel")?.GetComponent<Image>();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        uiM = FindAnyObjectByType<UIManager>();
        FindFadePanel();

        if (fadePanel != null)
        {
            Debug.Log("Achou FadePanel");
            StartCoroutine(uiM.Fade(1, 0, 0.5f));
        }
    }

    public void SetNextScene(string sceneName)
    {
        nextScene = sceneName;
    }

    public void StartTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToNextScene());
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        if (fadePanel != null)
        {
            yield return StartCoroutine(uiM.Fade(0, 1, 0.5f));
        }

        Debug.Log("Carregando cena: " + nextScene);
        SceneManager.LoadScene(nextScene);

        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }
}
