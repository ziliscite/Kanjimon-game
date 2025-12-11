using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private Transform spawnPoint;
    public int currentEnemyID;


    void Start()
    {
        EnemyDataSO data = GameManager.instance.currentEnemy;
        currentEnemyID = data.enemyID;

        Instantiate(data.enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    // public void UpdateHealth()
    // {
    //     if 
    // }

    // void SpawnEnemy()
    // {
    //     switch (currentEnemyID)
    //     {
    //         case 1:
    //             Instantiate(enemyList[0], spawnPoint.position, Quaternion.identity);
    //             break;
    //         default:
    //             Debug.Log("No enemy spawned");
    //             break;
    //     }
    // }
}

