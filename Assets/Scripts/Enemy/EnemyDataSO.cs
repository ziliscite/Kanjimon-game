using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Kanjimon/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Attributes")]
    public int enemyID;
    public string enemyName;
    public int hp;
    public int attackPower;
}
