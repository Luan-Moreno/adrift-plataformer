using System.Collections;
using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Configuração do Portal")]
    public string sceneToLoad;
    public string portalID;
    public string destinationPortalID;
    private bool canTeleport = true;
    public bool spawnRight = true;
    public float offset = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTeleport)
        {
            SequenceManager.lastUsedPortalID = destinationPortalID;
            SequenceManager.spawnRight = spawnRight;

            TransitionManager transition = FindAnyObjectByType<TransitionManager>();
            if (transition != null)
            {
                transition.SetNextScene(sceneToLoad);
                transition.StartTransition();
            }
        }
    }

    public IEnumerator WaitTeleport()
    {
        canTeleport = false;
        yield return new WaitForSeconds(0.5f);
        canTeleport = true;
    }
}
