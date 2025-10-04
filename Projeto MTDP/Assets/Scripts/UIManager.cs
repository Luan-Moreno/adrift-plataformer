using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settings;
    [SerializeField] private bool settingsState;
    public GameObject pauseMenu;
    [SerializeField] private bool pauseState;

    public bool PauseState { get => pauseState; set => pauseState = value; }

    void Start()
    {
        if (settings != null)
        {
            settingsState = false;
            settings.SetActive(settingsState);
        }
        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
            PauseState = false;
            pauseMenu.SetActive(PauseState);
            Time.timeScale = PauseState ? 0f : 1f;
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
}
