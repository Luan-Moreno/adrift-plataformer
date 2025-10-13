using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour
{
    private bool isResting = false;
    private bool canRest = true;
    [SerializeField] private AudioClip bonfireTheme;
    [SerializeField][Range(0f, 1f)] private float bonfireVolume = 0.5f;

    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private UIManager uiM;
    private GameManager gameManager;

    public bool IsResting { get => isResting; set => isResting = value; }

    void Start()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerAnim = FindAnyObjectByType<PlayerAnim>();
        anim = playerAnim.GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void Update()
    {

    }

    public IEnumerator RestSequence()
    {
        if (!canRest) yield break;

        canRest = false;

        if (!IsResting)
        {
            IsResting = true;
            playerMovement.Speed = 0f;
            playerCombat.ReceiveHealing(playerCombat.MaxHp);
            //playerCombat.RechargeItems();
            yield return StartCoroutine(uiM.Fade(0, 1));

            AudioManager.instance.PlayTemporaryBGM(bonfireTheme, bonfireVolume);

            if (playerCombat.Bonfire != null)
            {
                playerCombat.transform.position = playerCombat.Bonfire.transform.position;
            }

            playerCombat.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            anim.SetBool("isResting", true);

            // Salvar
            gameManager.SaveGame();

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(uiM.Fade(1, 0));
            uiM.PauseState = true;
        }
        else
        {
            yield return StartCoroutine(uiM.Fade(0, 1));
            anim.SetBool("isResting", false);
            uiM.PauseState = false;
            yield return new WaitForSeconds(0.5f);
            playerMovement.Speed = playerMovement.InitialSpeed;
            yield return StartCoroutine(uiM.Fade(1, 0));
            IsResting = false;
            AudioManager.instance.RestorePreviousBGM();
        }

        yield return new WaitForSeconds(1);
        canRest = true;
    }
}
