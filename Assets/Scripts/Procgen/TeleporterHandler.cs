using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleporterHandler : MonoBehaviour
{
    [SerializeField] private Walker walker;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private GameObject[] doorSurroundPrefabs;
    
    private GameObject _entryInstance;
    private Vector2Int _entryPosition;
    
    private GameObject _exitInstance;
    private Vector2Int _exitPosition;
    
    private List<GameObject> _surroundInstances = new ();
    
    public event Action OnTeleporterPlaced;

    void Start()
    {
        StartCoroutine(PlaceTeleporterCoroutine());
    }

    private bool IsGenerationCompleted()
    {
        if (walker == null || groundTilemap == null || playerHandler == null || playerHandler.GetPlayerInstance() == null) return false;
        
        var min = walker.GetGridMin();
        var max = walker.GetGridMax();

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                if (groundTilemap.HasTile(new Vector3Int(x, y, 0)))
                    return true;
            }
        }

        return false;
    }
    
    // check if the farthest point has 3x3 free area 
    private bool IsTilemapValid(Vector2Int position)
    {
        var gridMin = walker.GetGridMin();
        var gridMax = walker.GetGridMax();
        
        // check if the position is within tilemap bounds
        if (position.x < gridMin.x || position.x > gridMax.x || position.y < gridMin.y || position.y > gridMax.y) return false;
        
        // check if the 3x3 surrounding area has a walkable area
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                var pos = new Vector2Int(position.x + dx, position.y + dy);
                
                // check if the position is within tilemap bounds
                if (pos.x < gridMin.x || pos.x > gridMax.x || pos.y < gridMin.y || pos.y > gridMax.y) return false;
                
                // check if the position is walkable
                if (!groundTilemap.HasTile(new Vector3Int(pos.x, pos.y, 0))) return false;
            }
        }
        
        return true;
    }

    private Vector2Int? GetFarthestPosition(Vector3Int playerPos)
    {
        Vector2Int? farthestPos = null;
        var maxDistance = -1f;
        
        // scan ground tilemap within grid bounds
        for (int i = walker.GetGridMin().x; i <= walker.GetGridMax().x; i++)
        {
            for (int j = walker.GetGridMin().y; j <= walker.GetGridMax().y; j++)
            {
                if (!groundTilemap.HasTile(new Vector3Int(i, j, 0))) continue;
                
                // if ground has tile, get new position
                var position = new Vector2Int(i, j);

                // check if the position is valid
                if (!IsTilemapValid(position)) continue;
                
                // calculate Euclidean distance to player
                var distance = Vector2Int.Distance(new Vector2Int(playerPos.x, playerPos.y), position);
                
                // update farthest
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPos = position;
                }
            }
        }
        
        return farthestPos;
    }
    
    private List<GameObject> PlaceSpritesAroundGrid(Vector2Int centerGrid)
    {
        // 8 neighbors
        Vector2Int[] offsets = {
            new(-1,  1), // 0 top-left
            new( 0,  1), // 1 top
            new( 1,  1), // 2 top-right
            new(-1,  0), // 3 left
            new( 1,  0), // 5 right
            new(-1, -1), // 6 bottom-left
            new( 0, -1), // 7 bottom
            new( 1, -1)  // 8 bottom-right
        };

        // center offset to place sprites at tile center
        Vector3 tileCenterOffset = new Vector3(0.5f, 0.5f, 0f);
        var spawns = new List<GameObject>();
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int grid = new Vector2Int(centerGrid.x + offsets[i].x, centerGrid.y + offsets[i].y);
            Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(grid.x, grid.y, 0)) + tileCenterOffset;

            // Parent to the exit instance if it exists, otherwise don't parent
            Transform parentTransform = _exitInstance != null ? _exitInstance.transform : null;
            GameObject inst = Instantiate(doorSurroundPrefabs[i], worldPos, Quaternion.identity, parentTransform);
            spawns.Add(inst);
        }

        return spawns;
    }

    private void PlaceTeleporter(Vector2Int position, bool isExit)
    {
        // convert to world pos using tilemap and center the tile (0.5,0.5)
        var worldPos = groundTilemap.CellToWorld(new Vector3Int(position.x, position.y, 0));
        worldPos += new Vector3(0.5f, 0.5f, 0f);
    
        // instantiate
        if (isExit)
        {
            _exitInstance = Instantiate(exitPrefab, worldPos, Quaternion.identity);
            _surroundInstances = PlaceSpritesAroundGrid(position);
        }
        else
        {
            _entryInstance = Instantiate(entryPrefab, worldPos, Quaternion.identity);
        }
    }

    private void RenderExit(Vector2Int pos)
    {
        _exitPosition = new Vector2Int(pos.x, pos.y);
        PlaceTeleporter(_exitPosition, true);
        
        // put in the trigger
        var exitTrigger = _exitInstance.GetComponent<TeleporterTrigger>();
        if (exitTrigger != null)
        {
            exitTrigger.SetWalker(walker);
            exitTrigger.SetDirection(1);
        }
    }
    
    private void PlaceExit()
    {
        var player = playerHandler.GetPlayerInstance();
        var playerPos = groundTilemap.WorldToCell(player.transform.position);
        
        // find the farthest index
        var farthestIndex = GetFarthestPosition(playerPos);
        if (farthestIndex.HasValue)
        {
            _exitPosition = new Vector2Int(farthestIndex.Value.x, farthestIndex.Value.y);
            RenderExit(_exitPosition);
        }
    }

    private void LoadExit()
    {
        RenderExit(_exitPosition);
    }

    private void RenderEntry(Vector2Int pos)
    {
        _entryPosition = new Vector2Int(pos.x, pos.y);
        PlaceTeleporter(_entryPosition, false);
        
        // put in the trigger
        var entryTrigger = _entryInstance.GetComponent<TeleporterTrigger>();
        if (entryTrigger != null)
        {
            entryTrigger.SetWalker(walker);
            entryTrigger.SetDirection(-1);
        }
    }
    
    private void PlaceEntry()
    {
        var player = playerHandler.GetPlayerInstance();
        var playerPos = groundTilemap.WorldToCell(player.transform.position);
        _entryPosition = new Vector2Int(playerPos.x, playerPos.y) + Vector2Int.left;
        // put 1 tile behind player
        RenderEntry(_entryPosition);
    }

    private void LoadEntry()
    {
        RenderEntry(_entryPosition);
    }

    private void CleanUp()
    {
        if (_entryInstance != null)
        {
            Destroy(_entryInstance);
            _entryInstance = null;
        }

        if (_exitInstance != null)
        {
            Destroy(_exitInstance);
            _exitInstance = null;
        }

        if (_surroundInstances != null && _surroundInstances.Count > 0)
        {
            foreach (var sur in _surroundInstances) Destroy(sur);
            _surroundInstances.Clear();
        }
    }
    
    private IEnumerator PlaceTeleporterCoroutine()
    {
        yield return new WaitUntil(IsGenerationCompleted);

        CleanUp();
        PlaceEntry();
        PlaceExit();
        
        OnTeleporterPlaced?.Invoke();
    }
    
    private IEnumerator LoadTeleporterCoroutine()
    {
        yield return new WaitUntil(IsGenerationCompleted);

        CleanUp();
        LoadEntry();
        LoadExit();
    }
    
    public void LoadDoor(TeleporterPosition entry, TeleporterPosition exit)
    {
        _entryPosition = new Vector2Int(entry.x, entry.y);
        _exitPosition = new Vector2Int(exit.x, exit.y);
        
        StartCoroutine(LoadTeleporterCoroutine());
    }
    
    public void GenerateDoor()
    {
        StartCoroutine(PlaceTeleporterCoroutine());
    }
    
    // getter for entry and exit door
    public TeleporterPosition EntryDoor => new ()
    {
        x = _entryPosition.x, 
        y = _entryPosition.y,
        direction = -1
    };
    
    public TeleporterPosition ExitDoor => new ()
    {
        x = _exitPosition.x, 
        y = _exitPosition.y,
        direction = 1
    };
}