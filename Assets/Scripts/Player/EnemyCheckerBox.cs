using UnityEngine;

public class EnemyCheckerBox : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    public bool enemyInsideRange;
    public GameObject targetEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInsideRange = true;
            targetEnemy = other.gameObject;
            playerAttack.enemyOnSight = targetEnemy; // set enemy in PlayerAttack
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInsideRange = false;
            targetEnemy = null;
            playerAttack.enemyOnSight = null; // reset enemy in PlayerAttack
        }
    }
}
