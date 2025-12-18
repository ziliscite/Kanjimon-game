using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    
    [Header("Player Data")]
    public int playerHP;
    public int potionsLeft;
    public int expPlayer;
    public int playerLevel;
    public Vector3 lastPosition;
    public int lastFloor;
    public string sceneName;

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
