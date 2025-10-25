using UnityEngine;
using System.Collections;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Dialogue Manager in the scene!");
        }

        instance = this;
    }
    #endregion
    #region Variables
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image speakerImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private Story currentStory;
    private bool isDialoguePlaying;
    private PlayerMovement playerMovement;
    private Animator anim;

    public bool IsDialoguePlaying { get => isDialoguePlaying; set => isDialoguePlaying = value; }
    public GameObject DialoguePanel { get => dialoguePanel; set => dialoguePanel = value; }

    #endregion
    void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        anim = playerMovement.GetComponent<Animator>();
        IsDialoguePlaying = false;
        DialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialoguePlaying) { return; }


        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, Sprite speakerSprite)
    {
        anim.Rebind();
        anim.Update(0f);
        UIManager.instance.PauseState = true;
        currentStory = new Story(inkJSON.text);
        IsDialoguePlaying = true;
        speakerImage.sprite = speakerSprite;
        DialoguePanel.SetActive(true);
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        DialoguePanel.SetActive(false);
        dialogueText.text = "";
        StartCoroutine(EndDialogue());
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    IEnumerator EndDialogue()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        UIManager.instance.PauseState = false;
        IsDialoguePlaying = false;
    }
}
