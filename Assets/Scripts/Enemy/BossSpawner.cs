using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private TeleporterHandler teleporterHandler;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private ObjectPlacer objectPlacer;
    
    private GameObject _objectInstance;
    
    // clear all previously placed potions
    private void ClearPlacedObjects()
    {
        if (_objectInstance != null) Destroy(_objectInstance);
    }
    
    private void PlaceBoss()
    {
        ClearPlacedObjects();
        if (bossPrefab == null || objectPlacer == null || teleporterHandler == null) return;
        
        // get exit teleporter position
        var exitDoor = teleporterHandler.ExitDoor;
        var exitPos = new Vector3Int(exitDoor.x, exitDoor.y, 0);
        
        // define 4 positions that are 2 tiles away in cardinal directions
        Vector3Int[] offsets = {
            new Vector3Int(2, 0, 0),   // right
            new Vector3Int(-2, 0, 0),  // left
            new Vector3Int(0, 2, 0),   // up
            new Vector3Int(0, -2, 0)   // down
        };
        
        var tilemap = objectPlacer.GetGroundTilemap();
        if (tilemap == null) return;
        
        // collect all valid positions
        var validPositions = new List<Vector3Int>();
        foreach (var offset in offsets)
        {
            var candidatePos = exitPos + offset;
            
            // check if tile exists at this position
            if (tilemap.HasTile(candidatePos))
            {
                validPositions.Add(candidatePos);
            }
        }
        
        // if no valid positions found, fallback to random placement
        if (validPositions.Count == 0)
        {
            var fallbackPos = exitPos + new Vector3Int(-1, 0, 0);
            var fallbackWorldPos = objectPlacer.TileToWorldPosition(fallbackPos);
            
            var instance = Instantiate(bossPrefab, fallbackWorldPos, Quaternion.identity);
            _objectInstance = instance;
            return;
        }
        
        // randomly pick one of the valid positions
        var selectedPos = validPositions[Random.Range(0, validPositions.Count)];
        var selectedWorldPos = objectPlacer.TileToWorldPosition(selectedPos);
        
        var bossInstance = Instantiate(bossPrefab, selectedWorldPos, Quaternion.identity);
        _objectInstance = bossInstance;
    }
    
    public void PlaceBossFromData(BossPosition bossPosition)
    {
        ClearPlacedObjects();
        if (bossPrefab == null || objectPlacer == null || bossPosition == null) return;
        
        var tilePos = new Vector3Int(bossPosition.x, bossPosition.y, 0);
        var worldPos = objectPlacer.TileToWorldPosition(tilePos);
        
        var instance = Instantiate(bossPrefab, worldPos, Quaternion.identity);
        _objectInstance = instance;
    }
    
    private IEnumerator PlaceBossCoroutine()
    {
        yield return new WaitUntil(() => objectPlacer.IsGenerationCompleted());
        PlaceBoss();
        if (BossManager.Instance != null)
        {
            BossManager.Instance.ResetFloor();
        }
    }
    
    public void OnLevelGenerated()
    {
        StartCoroutine(PlaceBossCoroutine());
    }
    
    public BossPosition GetBossPosition()
    {
        var bossPosition = new BossPosition();
        var tilemap = objectPlacer.GetGroundTilemap();
        
        if (tilemap == null) return bossPosition;
        
        if (_objectInstance != null)
        {
            var pos = tilemap.WorldToCell(_objectInstance.transform.position);
            bossPosition = new BossPosition { x = pos.x, y = pos.y, enemyId = 5};
        }
        
        return bossPosition;
    }
}