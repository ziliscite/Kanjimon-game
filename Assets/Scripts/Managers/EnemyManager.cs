using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    
    private readonly List<GameObject> _enemies = new();
    
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

    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null && !_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        _enemies.Clear();
    }

    public GameObject GetEnemyAt(int index)
    {
        return index >= 0 && index < _enemies.Count ? _enemies[index] : null;
    }

    public int GetEnemyCount()
    {
        return _enemies.Count;
    }
}