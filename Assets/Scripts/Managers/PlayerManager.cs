using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    // public EnemyDataSO currentEnemy;
    
    [Header("Player Data")]
    public int playerHP;
    public int potionsLeft;

    [Header("Enemy Data")]
    public int enemyBattledID;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
