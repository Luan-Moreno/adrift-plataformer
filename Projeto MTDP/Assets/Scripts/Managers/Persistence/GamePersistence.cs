using UnityEngine;

public class GamePersistence : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.Find("PersistentManagers") != null)
        {
            Destroy(gameObject);
        }        

        LoadAndInstantiate("PersistentManagers");
        LoadAndInstantiate("PersistentCameras");
        LoadAndInstantiate("PersistentUI");
        LoadAndInstantiate("Player");
    }

    private void LoadAndInstantiate(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Persistent/{prefabName}");

        if (prefab == null)
        {
            Debug.Log(prefabName);
            Debug.LogError("Não encontrado em Resources/Prefabs/Persistent");
            return;
        }

        GameObject instance = Instantiate(prefab);
        DontDestroyOnLoad(instance);
    }
}
