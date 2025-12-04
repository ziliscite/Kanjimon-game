using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Kanjimon/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Attributes")]
    public string enemyName;
    public int hp;
    public int attackMultiplier = 2;
    public int exp = 5; // buat exp ke player setelah kill
    
    [Header("Misscellaneous")]
    public Sprite portrait; // buat icon in battle
    public GameObject enemyPrefab;
}
