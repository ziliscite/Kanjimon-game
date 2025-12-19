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
    public int lastFloor;
    public string sceneName;
    public int currentLevelIndex;
    public string playerId;

    [Header("Enemy Data")]
    public int enemyBattledID;

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
}
