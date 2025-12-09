using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePersistence : MonoBehaviour
{
    Scene m_Scene;
    string sceneName;
    private bool persistentLoaded = false;

    #region Singleton
    public static GamePersistence instance { get; private set; }

    private void Awake() 
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainMenu")
        {
            UnloadPersistent();
            persistentLoaded = false;
            return;
        }

        if(!persistentLoaded)
        {
            if(scene.name != "IntroScene")
            {
                Unload("TestLight (Disable on Play)");
                LoadPersistent();
                persistentLoaded = true;
            }
        }
    }

    private void LoadPersistent()
    {
        LoadAndInstantiate("PersistentManagers");
        LoadAndInstantiate("PersistentCameras");
        LoadAndInstantiate("PersistentUI");
        LoadAndInstantiate("Player");
    }

    private void UnloadPersistent()
    {
        Unload("PersistentManagers");
        Unload("PersistentCameras");
        Unload("PersistentUI");
        Unload("Player");
    }

    private void LoadAndInstantiate(string prefabName, string folderName="Persistent")
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{folderName}/{prefabName}");

        if (prefab == null)
        {
            Debug.Log(prefabName);
            Debug.LogError("Não encontrado em Resources/Prefabs/Persistent");
            return;
        }

        GameObject instance = Instantiate(prefab);
        instance.name = prefabName;
        DontDestroyOnLoad(instance);
    }

    private void Unload(string prefabName)
    {
        GameObject obj = GameObject.Find(prefabName);
        Destroy(obj);
    }
}