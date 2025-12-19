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
        var validPositions = new System.Collections.Generic.List<Vector3Int>();
        foreach (var offset in offsets)
        {
            var candidatePos = exitPos + offset;
            
            // check if tile exists at this position
            if (tilemap.HasTile(candidatePos))
            {
                validPositions.Add(candidatePos);
            }
        }
        
        // if no valid positions found at distance 2, try distance 3
        // (distance 1 is occupied by door surrounds)
        if (validPositions.Count == 0)
        {
            Vector3Int[] fallbackOffsets = {
                new Vector3Int(3, 0, 0),   // right
                new Vector3Int(-3, 0, 0),  // left
                new Vector3Int(0, 3, 0),   // up
                new Vector3Int(0, -3, 0)   // down
            };
            
            foreach (var offset in fallbackOffsets)
            {
                var candidatePos = exitPos + offset;
                
                if (tilemap.HasTile(candidatePos))
                {
                    validPositions.Add(candidatePos);
                }
            }
            
            // if still no valid positions, just place at distance 3 to the left regardless
            if (validPositions.Count == 0)
            {
                var lastResortPos = exitPos + new Vector3Int(-3, 0, 0);
                var lastResortWorldPos = objectPlacer.TileToWorldPosition(lastResortPos);
                
                var instance = Instantiate(bossPrefab, lastResortWorldPos, Quaternion.identity);
                _objectInstance = instance;
                return;
            }
        }
        
        // randomly pick one of the valid positions
        var selectedPos = validPositions[Random.Range(0, validPositions.Count)];
        var selectedWorldPos = objectPlacer.TileToWorldPosition(selectedPos);
        
        var bossInstance = Instantiate(bossPrefab, selectedWorldPos, Quaternion.identity);
        _objectInstance = bossInstance;
    }
    
    public void PlaceBossFromData(BossPosition bossPosition, bool bossDefeated)
    {
        ClearPlacedObjects();
        if (bossPrefab == null || objectPlacer == null || bossPosition == null) return;
        
        bool isBossActuallyDead = bossDefeated;
        if (BossManager.Instance != null)
        {
            // Jika BossManager bilang boss mati, override data JSON
            if (BossManager.Instance.IsBossDead())
            {
                isBossActuallyDead = true;
                Debug.Log("[BossSpawner] BossManager says boss is dead, overriding JSON data");
            }

            BossManager.Instance.SetBossDead(isBossActuallyDead);
        }
    
        if (isBossActuallyDead)
        {
            Debug.Log("[BossSpawner] Boss is dead, not spawning");
            return;
        }
        
        var tilePos = new Vector3Int(bossPosition.x, bossPosition.y, 0);
        var worldPos = objectPlacer.TileToWorldPosition(tilePos);
        
        var instance = Instantiate(bossPrefab, worldPos, Quaternion.identity);
        _objectInstance = instance;
    }
    
    private IEnumerator PlaceBossCoroutine()
    {
        yield return new WaitUntil(() => objectPlacer.IsGenerationCompleted());
        PlaceBoss();
    }
    
    public void OnLevelGenerated()
    {
        StartCoroutine(PlaceBossCoroutine());
    }
    
    public BossPosition GetBossPosition()
    {
        // Don't save boss position if boss is dead
        if (BossManager.Instance != null && BossManager.Instance.IsBossDead())
        {
            Debug.Log("[BossSpawner] Boss is dead, not saving position");
            return null;
        }
        
        BossPosition bossPosition = null;
        
        var tilemap = objectPlacer.GetGroundTilemap();
        if (tilemap == null) return null;
        
        if (_objectInstance != null)
        {
            var pos = tilemap.WorldToCell(_objectInstance.transform.position);
            bossPosition = new BossPosition { x = pos.x, y = pos.y, enemyId = 5};
        }
        
        return bossPosition;
    }
}