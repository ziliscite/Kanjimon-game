using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject potionPrefab;
    [SerializeField] private int prefabNumber = 5;
    [SerializeField] private ObjectPlacer objectPlacer;
    
    private readonly List<GameObject> _objectInstances = new();
    
    // clear all previously placed potions
    private void ClearPlacedObjects()
    {
        foreach (var instance in _objectInstances)
        {
            if (instance != null) Destroy(instance);
        }
        _objectInstances.Clear();
    }
    
    // place potion prefab randomly
    private void PlacePotions()
    {
        ClearPlacedObjects();
        if (potionPrefab == null || objectPlacer == null) return;
        
        // get all tile pos
        var floorPositions = objectPlacer.GetFloorPositions();
        if (floorPositions.Count == 0) return;
        
        // place potion prefab randomly
        for (int i = 0; i < Mathf.Min(prefabNumber, floorPositions.Count); i++)
        {
            // pick random floor pos
            var randomIndex = Random.Range(0, floorPositions.Count);
            var tilePos = floorPositions[randomIndex];
            
            // convert to world pos, also center it on tile
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(potionPrefab, worldPos, Quaternion.identity);
            _objectInstances.Add(instance);
            
            // make sure no overlap, remove used pos
            floorPositions.RemoveAt(randomIndex);
        }
    }
    
    // place potions from save data
    public void PlacePotionsFromData(List<PotionPosition> potionData)
    {
        ClearPlacedObjects();
        if (potionPrefab == null || objectPlacer == null || potionData == null) return;
        
        foreach (var data in potionData)
        {
            var tilePos = new Vector3Int(data.x, data.y, 0);
            var worldPos = objectPlacer.TileToWorldPosition(tilePos);
            
            var instance = Instantiate(potionPrefab, worldPos, Quaternion.identity);
            _objectInstances.Add(instance);
        }
    }
    
    // wait till level gen is done
    private IEnumerator PlacePotionsCoroutine()
    {
        yield return new WaitUntil(() => objectPlacer.IsGenerationCompleted());
        PlacePotions();
    }
    
    // pub method to regen potions, called by walker on new level gen
    public void OnLevelGenerated()
    {
        StartCoroutine(PlacePotionsCoroutine());
    }
    
    // get potion data for saving
    public List<PotionPosition> GetPotionData()
    {
        var potionData = new List<PotionPosition>();
        var tilemap = objectPlacer.GetGroundTilemap();
        
        if (tilemap == null) return potionData;
        
        foreach (var instance in _objectInstances)
        {
            if (instance != null)
            {
                var pos = tilemap.WorldToCell(instance.transform.position);
                potionData.Add(new PotionPosition { x = pos.x, y = pos.y });
            }
        }
        
        return potionData;
    }
}