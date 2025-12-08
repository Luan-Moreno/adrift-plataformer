using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    private bool playerInRange;
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private Sprite npcSprite;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        visualCue.SetActive(false);
        playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && !DialogueManager.instance.IsDialoguePlaying)
        {
            visualCue.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E) && playerMovement.IsGrounded)
            {
                Debug.Log("Dialogo Iniciado");
                DialogueManager.instance.EnterDialogueMode(inkJSON, npcSprite);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
