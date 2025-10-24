using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private float globalShakeForce = 1f;
    [SerializeField] private bool canShake;

    public bool CanShake { get => canShake; set => canShake = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        canShake = true;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        StartCoroutine(CanShakeDelay(impulseSource, globalShakeForce));
    }

    public void StrongCameraShake(CinemachineImpulseSource impulseSource)
    {
        StartCoroutine(CanShakeDelay(impulseSource, globalShakeForce * 3));
    }

    private IEnumerator CanShakeDelay(CinemachineImpulseSource impulseSource, float shakeForce)
    {
        CanShake = false;
        impulseSource.GenerateImpulseWithForce(shakeForce);
        yield return new WaitForSeconds(0.1f);
        CanShake = true;
    }
}
