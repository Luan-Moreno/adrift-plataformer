using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private UIManager uiM;
    private GameManager gameManager;

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
}
