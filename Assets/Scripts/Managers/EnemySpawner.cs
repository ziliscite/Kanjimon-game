using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private Transform spawnPoint;
    // [SerializeField] private PlayerData playerData;
    public EnemyData SpawnedEnemy { get; private set; }
    public int currentEnemyID;
    public int enemyHealthSpawned;
    
    void Awake()
    {
        currentEnemyID = PlayerManager.Instance.enemyBattledID;
    }
    
    void Start()
    {
        SpawnEnemy(currentEnemyID);
    }

    private void SpawnEnemy(int enemyID)
    {
        GameObject enemy = Instantiate(enemyList[enemyID - 1], spawnPoint.position, Quaternion.identity);
        SpawnedEnemy = enemy.GetComponent<EnemyData>();
        enemyHealthSpawned = SpawnedEnemy.enemyDataSO.hp;
    }
}

