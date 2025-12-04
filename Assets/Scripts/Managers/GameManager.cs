using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public EnemyDataSO currentEnemy;

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
