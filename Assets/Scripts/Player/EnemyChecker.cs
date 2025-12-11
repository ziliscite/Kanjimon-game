using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    public bool enemyInsideRange;
    public GameObject targetEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Bahlil detected");
            enemyInsideRange = true;
            targetEnemy = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInsideRange = false;
            targetEnemy = null;
        }
    }
}
