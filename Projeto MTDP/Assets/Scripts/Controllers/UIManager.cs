using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject settings;
    [SerializeField] private bool settingsState;

    public GameObject pauseMenu;
    public GameObject gameOver;
    [SerializeField] private bool pauseState;
    private Scene currentScene;
    public Image fadePanel;
    public float fadeDuration = 1f;


    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private SequenceManager sequenceManager;

    public bool PauseState { get => pauseState; set => pauseState = value; }

    void Start()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerAnim = FindAnyObjectByType<PlayerAnim>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        anim = playerAnim.GetComponent<Animator>();
        sequenceManager = FindAnyObjectByType<SequenceManager>();

        if (settings != null)
        {
            settingsState = false;
            settings.SetActive(settingsState);
        }
        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver.activeInHierarchy)
        {
            PauseState = !PauseState;
            pauseMenu.SetActive(PauseState);
            Time.timeScale = PauseState ? 0f : 1f;
        }
    }

    public void Pause()
    {
        if (pauseMenu != null)
        {
            if(!sequenceManager.IsResting)
            {
                PauseState = false;
                pauseMenu.SetActive(PauseState);
                Time.timeScale = PauseState ? 0f : 1f;
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void ReloadScene()
    {
        currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void TurnSettings()
    {
        settingsState = !settingsState;
        settings.SetActive(settingsState);
    }

    public void CloseGame()
    {
        Debug.Log("Jogo Fechado");
        Application.Quit();
    }

        public IEnumerator Fade(float startAlpha, float endAlpha)
    {
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
    }
}
