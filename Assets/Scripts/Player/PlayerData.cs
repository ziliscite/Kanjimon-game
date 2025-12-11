using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header ("Player Stats")]
    public int playerHealth = 100;

    [Header ("Enemy Battled")]
    public int enemyBattledID;

    [Header ("References")]
    [SerializeField] private EnemyCheckerBox enemyCheckerBox;
    [SerializeField] private PlayerAttack playerAttack;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
