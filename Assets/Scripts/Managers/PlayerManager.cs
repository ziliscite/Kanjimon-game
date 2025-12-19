using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    [Header("Player Data")]
    public int playerHP;
    public int potionsLeft;
    public int expPlayer;
    public int playerLevel;
    public Vector3 lastPosition;
    public int lastFloor = 1;
    public string sceneName;
    public int currentLevelIndex;
    public string playerId;
    
    [Header("Battle Data")]
    public bool isReturningFromBattle = false;
    public bool isWinningBattle = false;
    
    [Header("Enemy Data")]
    public int enemyBattledID;
    public int enemyInstanceIndex;
    public bool isEnemyBoss;
    

    void Awake()
    {
        if (Instance!= null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetFloor(int level)
    {
        lastFloor = level;
    }
    
    public void SetEnemyData(int enemyID, int instanceIndex, bool isBoss)
    {
        enemyBattledID = enemyID;
        enemyInstanceIndex = instanceIndex;
        isEnemyBoss = isBoss;
    }

    public void CleanUp()
    {
        isWinningBattle = false;
        isReturningFromBattle = false;
        Debug.Log("[PlayerManager] Battle cleanup completed");
    }
}