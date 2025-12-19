using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabs;
    [SerializeField] private int prefabNumber = 5;
    [SerializeField] private ObjectPlacer objectPlacer;
    
    private void ClearPlacedObjects()
    {
        EnemyManager.Instance.ClearAllEnemies();
    }
    
    private void PlaceMonsters()
    {
        ClearPlacedObjects();
        if (objectPlacer == null || monsterPrefabs == null || monsterPrefabs.Count == 0) return;
        
        Debug.Log($"[MonsterSpawner] Placing {prefabNumber} monsters");
        
        var floorPositions = objectPlacer.GetFloorPositions();
        if (floorPositions.Count == 0) return;
        
        for (int i = 0; i < Mathf.Min(prefabNumber, floorPositions.Count); i++)
        {
            var randomIndex = Random.Range(0, floorPositions.Count);
            var tilePos = floorPositions[randomIndex];
            
            var randomMonsterIndex = Random.Range(0, monsterPrefabs.Count);
            var selectedMonster = monsterPrefabs[randomMonsterIndex];
            
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(selectedMonster, worldPos, Quaternion.identity);
            instance.name = i.ToString();
            EnemyManager.Instance.AddEnemy(instance);
            
            floorPositions.RemoveAt(randomIndex);
        }
    }
    
    public void PlaceMonstersFromData(List<EnemyPosition> monsterData)
    {
        ClearPlacedObjects();
        if (objectPlacer == null || monsterPrefabs == null || monsterData == null) return;
        
        Debug.Log($"[MonsterSpawner] Original monster data count: {monsterData.Count}");
        
        // Filter out the defeated enemy if returning from a won battle
        List<EnemyPosition> enemiesToPlace = monsterData;
        if (PlayerManager.Instance != null && PlayerManager.Instance.isWinningBattle)
        {
            int defeatedIndex = PlayerManager.Instance.enemyInstanceIndex;
            if (defeatedIndex == -1)
            {
                Debug.Log("[MonsterSpawner] Defeated enemy was a boss (index -1), handled by BossManager");
            }
            else
            {
                enemiesToPlace = monsterData.Where(e => e.enemyInstanceIndex != defeatedIndex).ToList();
                Debug.Log($"[MonsterSpawner] Filtered out defeated enemy at index {defeatedIndex}");
            }
        }
        
        Debug.Log($"[MonsterSpawner] Placing {enemiesToPlace.Count} monsters from save data");
        
        int newIndex = 0;
        foreach (var data in enemiesToPlace)
        {
            if (data.enemyId < 0)
            {
                Debug.LogWarning($"[MonsterSpawner] Invalid enemy ID: {data.enemyId - 1}");
                continue;
            }
            
            var tilePos = new Vector3Int(data.x, data.y, 0);
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(monsterPrefabs[data.enemyId - 1], worldPos, Quaternion.identity);
            
            // Use sequential index instead of original index
            instance.name = newIndex.ToString();
            
            Debug.Log($"[MonsterSpawner] Spawned enemy {newIndex} (original index: {data.enemyInstanceIndex}) at position {worldPos}");
            
            EnemyManager.Instance.AddEnemy(instance);
            newIndex++;
        }
    }
    
    private IEnumerator PlaceMonstersCoroutine()
    {
        yield return new WaitUntil(() => objectPlacer.IsGenerationCompleted());
        PlaceMonsters();
    }
    
    public void OnLevelGenerated()
    {
        StartCoroutine(PlaceMonstersCoroutine());
    }
    
    public List<EnemyPosition> GetMonsterData()
    {
        var monsterData = new List<EnemyPosition>();
        var tilemap = objectPlacer.GetGroundTilemap();
        
        if (tilemap == null)
        {
            Debug.LogWarning("[MonsterSpawner] Tilemap is null in GetMonsterData");
            return monsterData;
        }
        
        Debug.Log($"[MonsterSpawner] GetMonsterData: EnemyManager has {EnemyManager.Instance.GetEnemyCount()} enemies");
        
        for (int i = 0; i < EnemyManager.Instance.GetEnemyCount(); i++)
        {
            var instance = EnemyManager.Instance.GetEnemyAt(i);
            if (instance != null)
            {
                var pos = tilemap.WorldToCell(instance.transform.position);
                var enemy = instance.GetComponent<EnemyData>();
                
                if (enemy != null && enemy.enemyDataSO != null)
                {
                    monsterData.Add(new EnemyPosition 
                    { 
                        enemyId = enemy.enemyDataSO.enemyID, 
                        x = pos.x, 
                        y = pos.y, 
                        enemyInstanceIndex = i  // Use current index, not old index
                    });
                    Debug.Log($"[MonsterSpawner] Saved enemy {i} at tile position ({pos.x}, {pos.y})");
                }
                else
                {
                    Debug.LogWarning($"[MonsterSpawner] Enemy at index {i} has no EnemyData component");
                }
            }
            else
            {
                Debug.LogWarning($"[MonsterSpawner] Enemy at index {i} is null");
            }
        }
        
        Debug.Log($"[MonsterSpawner] GetMonsterData returning {monsterData.Count} enemy positions");
        return monsterData;
    }
}