using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    private EnemyCombat enemyCombat;
    private Animator anim;
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyCombat = GetComponent<EnemyCombat>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (enemyMovement.IsMoving)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
