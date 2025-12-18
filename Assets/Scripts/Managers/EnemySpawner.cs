using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerData playerData;
    public int currentEnemyID;
    public bool isEnemyBoss;
    public int enemyHealthSpawned;
    
    void Awake()
    {
        playerData = FindFirstObjectByType<PlayerData>();
        currentEnemyID = playerData.enemyBattledID;
    }
    
    void Start()
    {
        SpawnEnemy(currentEnemyID);
    }

    private void SpawnEnemy(int enemyID)
    {
        switch (enemyID)
        {
            case 1:
                Instantiate(enemyList[0], spawnPoint.position, Quaternion.identity);
                break;
            case 2:
                Instantiate(enemyList[1], spawnPoint.position, Quaternion.identity);
                break;
            case 3:
                Instantiate(enemyList[2], spawnPoint.position, Quaternion.identity);
                break;
            default:
                Debug.LogError("Invalid enemy ID: " + enemyID);
                break;
        }
        enemyHealthSpawned = enemyList[enemyID - 1].GetComponent<EnemyData>().enemyDataSO.hp;
        isEnemyBoss = enemyList[enemyID - 1].GetComponent<EnemyData>().enemyDataSO.isBoss;
        Debug.Log("Spawned Enemy ID: " + enemyID);
    }
}

