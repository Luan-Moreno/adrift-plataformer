using System.Collections;
using System.Collections.Generic;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    #endregion
    
    #region Variables
    public GameObject settings;
    [SerializeField] private bool settingsState = false;
    [SerializeField] private List<Image> hearts;

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

        if (gameOver != null) gameOver.SetActive(false);
        if (fade != null) fade.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (inventory != null) inventory.SetActive(false);

        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver.activeInHierarchy
        && !inventory.activeInHierarchy && !dialogueManager.DialoguePanel.activeInHierarchy 
        && !settings.activeInHierarchy)
        {
            bool initialPauseState = PauseState;
            PauseState = !PauseState;
            pauseMenu.SetActive(PauseState);
            Time.timeScale = PauseState ? 0f : 1f;
            PauseState = initialPauseState;
        }

        if (Input.GetKeyDown(KeyCode.I) && !gameOver.activeInHierarchy
        && !pauseMenu.activeInHierarchy && !dialogueManager.DialoguePanel.activeInHierarchy
        && !settings.activeInHierarchy)
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
        Time.timeScale = 1f;
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
        Application.Quit();
    }

    public void UpdateHearts()
    {
        float maxHp = playerCombat.MaxHp; //10
        float hpLeft = playerCombat.CurrentHp; //10
        float perHeart = (float) maxHp / hearts.Count; //2
        
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hpLeft >= perHeart)
            {
                hearts[i].fillAmount = 1f;
            }
            else if (hpLeft > 0)
            {
                hearts[i].fillAmount = hpLeft / perHeart;
            }
            else
            {
                hearts[i].fillAmount = 0f;
            }

            hpLeft -= perHeart;
        }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fade.SetActive(false);
        gameOver.SetActive(false);
        pauseMenu.SetActive(false);
        inventory.SetActive(false);
        settings.SetActive(false);
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

