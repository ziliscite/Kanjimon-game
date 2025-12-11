using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Kanjimon/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Attributes")]
    public int enemyID;
    public string enemyName;
    public int hp;
    public int attack;
    public int exp; // buat exp ke player setelah kill
    // public GameObject enemyPrefab;
}
