using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }
    private bool _isBossDead;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetBossDead(bool isDead)
    {
        _isBossDead = isDead;
    }

    public bool IsBossDead()
    {
        return _isBossDead;
    }

    public void ResetFloor()
    {
        _isBossDead = false;
    }
}