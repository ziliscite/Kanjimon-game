using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabs;
    [SerializeField] private int prefabNumber = 5;
    [SerializeField] private ObjectPlacer objectPlacer;
    
    private readonly List<GameObject> _objectInstances = new();
    
    // clear all previously placed monsters
    private void ClearPlacedObjects()
    {
        foreach (var instance in _objectInstances)
        {
            if (instance != null) Destroy(instance);
        }
        _objectInstances.Clear();
    }
    
    // place monster prefabs randomly
    private void PlaceMonsters()
    {
        ClearPlacedObjects();
        if (objectPlacer == null || monsterPrefabs == null || monsterPrefabs.Count == 0) return;
        
        Debug.LogError($"[MonsterSpawner] Placing {prefabNumber} monsters");
        
        // get all tile pos
        var floorPositions = objectPlacer.GetFloorPositions();
        if (floorPositions.Count == 0) return;
        
        // place monster prefabs randomly
        for (int i = 0; i < Mathf.Min(prefabNumber, floorPositions.Count); i++)
        {
            // pick random floor pos
            var randomIndex = Random.Range(0, floorPositions.Count);
            var tilePos = floorPositions[randomIndex];
            
            // pick random monster from available pool
            var randomMonsterIndex = Random.Range(0, monsterPrefabs.Count);
            var selectedMonster = monsterPrefabs[randomMonsterIndex];
            
            // convert to world pos, also center it on tile
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(selectedMonster, worldPos, Quaternion.identity);
            _objectInstances.Add(instance);
            
            // make sure no overlap, remove used pos
            floorPositions.RemoveAt(randomIndex);
        }
    }
    
    // place monsters from save data
    public void PlaceMonstersFromData(List<EnemyPosition> monsterData)
    {
        ClearPlacedObjects();
        if (objectPlacer == null || monsterPrefabs == null || monsterData == null) return;
        
        Debug.LogError($"[MonsterSpawner] Placing {monsterData.Count} monsters from save data");
        foreach (var data in monsterData)
        {
            // validate monster id
            if (data.enemyId < 0 || data.enemyId >= monsterPrefabs.Count) continue;
            
            var tilePos = new Vector3Int(data.x, data.y, 0);
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(monsterPrefabs[data.enemyId], worldPos, Quaternion.identity);
            _objectInstances.Add(instance);
        }
    }
    
    // wait till level gen is done
    private IEnumerator PlaceMonstersCoroutine()
    {
        yield return new WaitUntil(() => objectPlacer.IsGenerationCompleted());
        PlaceMonsters();
    }
    
    // pub method to regen monsters, called by walker on new level gen
    public void OnLevelGenerated()
    {
        StartCoroutine(PlaceMonstersCoroutine());
    }
    
    // get monster data for saving
    public List<EnemyPosition> GetMonsterData()
    {
        var monsterData = new List<EnemyPosition>();
        var tilemap = objectPlacer.GetGroundTilemap();
        
        if (tilemap == null) return monsterData;
        
        foreach (var instance in _objectInstances)
        {
            if (instance != null)
            {
                var pos = tilemap.WorldToCell(instance.transform.position);
                
                // find monster id by comparing prefab name
                int monsterId = -1;
                var instanceName = instance.name.Replace("(Clone)", "").Trim();
                
                for (int i = 0; i < monsterPrefabs.Count; i++)
                {
                    if (monsterPrefabs[i] != null && monsterPrefabs[i].name == instanceName)
                    {
                        monsterId = i;
                        break;
                    }
                }
                
                if (monsterId != -1)
                {
                    monsterData.Add(new EnemyPosition { enemyId = monsterId, x = pos.x, y = pos.y });
                }
            }
        }
        
        return monsterData;
    }
}