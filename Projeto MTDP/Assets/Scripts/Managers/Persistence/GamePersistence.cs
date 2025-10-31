using UnityEngine;

public class GamePersistence : MonoBehaviour
{
    private void Awake()
    {
        if (FindAnyObjectByType<PersistentRoot>() == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/PersistentRoot");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
            else
            {
                Debug.LogError("Prefab PersistentRoot não encontrado!");
            }
        }
    }
}
