using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private Animator anim;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();    
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerMovement.IsMoving)
        {
            if (playerMovement.Speed == playerMovement.InitialSpeed)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isRunning", true);
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
    }
}
