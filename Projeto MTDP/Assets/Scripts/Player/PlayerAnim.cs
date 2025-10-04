using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator anim;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        anim = playerMovement.GetComponent<Animator>();
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
