using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraTargetAssigner : MonoBehaviour
{
    private CinemachineCamera virtualCam;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        AssignTarget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignTarget();
    }

    private void AssignTarget()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            virtualCam.Follow = player.transform;
            virtualCam.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("Nenhum player encontrado!");
        }
    }
}
