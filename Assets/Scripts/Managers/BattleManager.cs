using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerData playerData;
    public int currentEnemyID;

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
        Debug.Log("Spawned Enemy ID: " + enemyID);
    }
}

