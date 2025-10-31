using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one UI Manager in the scene!");
        }

        instance = this;
    }
    #endregion
    
    #region Variables
    public GameObject settings;
    [SerializeField] private bool settingsState = false;

    public GameObject pauseMenu;
    public GameObject gameOver;
    public GameObject inventory;
    [SerializeField] private bool pauseState;
    private Scene currentScene;
    public GameObject fade;
    private Image fadePanel;
    public float fadeDuration = 1f;

    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private SequenceManager sequenceManager;
    private DialogueManager dialogueManager;
    private InventoryManager inventoryManager;

    public bool PauseState { get => pauseState; set => pauseState = value; }

    #endregion

    void Start()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerAnim = FindAnyObjectByType<PlayerAnim>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        anim = playerAnim.GetComponent<Animator>();
        sequenceManager = FindAnyObjectByType<SequenceManager>();
        dialogueManager = FindAnyObjectByType<DialogueManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        fadePanel = fade.transform.Find("FadePanel").GetComponent<Image>();


        if (settings != null)
        {
            settingsState = false;
            settings.SetActive(settingsState);
        }
        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver.activeInHierarchy 
        && !inventory.activeInHierarchy && !dialogueManager.DialoguePanel.activeInHierarchy)
        {
            bool initialPauseState = PauseState;
            PauseState = !PauseState;
            pauseMenu.SetActive(PauseState);
            Time.timeScale = PauseState ? 0f : 1f;
            PauseState = initialPauseState;
        }

        if (Input.GetKeyDown(KeyCode.I) && !gameOver.activeInHierarchy 
        && !pauseMenu.activeInHierarchy && !dialogueManager.DialoguePanel.activeInHierarchy)
        {
            PauseState = !PauseState;
            inventory.SetActive(PauseState);
            Time.timeScale = PauseState ? 0f : 1f;
        }
    }

    public void Pause()
    {
        if (pauseMenu != null)
        {
            if (!sequenceManager.IsResting)
            {
                PauseState = false;
                pauseMenu.SetActive(PauseState);
                Time.timeScale = PauseState ? 0f : 1f;
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        if(sceneName == "MainMenu")
        {
            DataPersistenceManager.instance.SaveGame();
        }
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

    public IEnumerator Fade(float startAlpha, float endAlpha, float fadeDuration = 1f)
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
