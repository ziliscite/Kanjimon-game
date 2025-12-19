using System.Collections.Generic;
using UnityEngine;

public class LevelInstanceManager : MonoBehaviour
{
    public static LevelInstanceManager Instance;

    [Header("Spawners")]
    public PotionSpawner potionSpawner;
    public MonsterSpawner monsterSpawner;
    // public BossSpawner bossSpawner; // Tambahkan nanti jika ada

    private void Awake()
    {
        // Singleton sederhana khusus untuk Scene ini saja
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Fungsi wrapper untuk mengambil data Potion yang masih ada di scene
    public List<PotionPosition> GetCurrentPotionData()
    {
        if (potionSpawner != null)
        {
            return potionSpawner.GetPotionData();
        }
        return new List<PotionPosition>();
    }

    // Fungsi wrapper untuk mengambil data Monster yang masih hidup
    public List<EnemyPosition> GetCurrentMonsterData()
    {
        if (monsterSpawner != null)
        {
            return monsterSpawner.GetMonsterData();
        }
        return new List<EnemyPosition>();
    }
    
    // Fungsi wrapper untuk Boss (Contoh)
    public BossPosition GetCurrentBossData()
    {
        // Implementasi nanti sesuai script boss kamu
        return new BossPosition(); 
    }
}