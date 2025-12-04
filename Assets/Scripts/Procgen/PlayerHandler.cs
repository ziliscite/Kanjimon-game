using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private Walker walker;
    // access to tilemap for the player to be placed on
    [SerializeField] private Tilemap groundTilemap;
    // for the player game object
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private CinemachineCamera virtualCamera;
    
    // another game object, we used prefab as the game object to bring to the level
    // the instance is to make distinction if there's already player being instanced
    // then we need to delete and re-instance the object
    private GameObject _playerInstance;

    void Start()
    {
        if (walker == null) return;
        
        // start coroutine
        StartCoroutine(PlacePlayerCoroutine());
    }
    
    private bool IsGenerationCompleted()
    {
        if (walker == null || groundTilemap == null) return false;

        var gridMin = walker.GetGridMin();
        var gridMax = walker.GetGridMax();
        
        for (int i = gridMin.x; i <= gridMax.x; i++)
        {
            for (int j = gridMin.y; j <= gridMax.y; j++)
            {
                // check if there are any tile generated
                if (groundTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private Vector2Int? GetLeftmostFloorPosition()
    {
        Vector2Int? leftmost = null;
    
        // scan the ground tilemap (all of it) within grid bounds
        for (int i = walker.GetGridMin().x; i <= walker.GetGridMax().x; i++)
        {
            for (int j = walker.GetGridMin().y; j <= walker.GetGridMax().y; j++)
            {
                // check for the floor tiles, if not, continue
                if (!groundTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    continue;
                }
            
                // check if there's a tile to the left of this position
                if (!groundTilemap.HasTile(new Vector3Int(i - 1, j, 0)))
                {
                    continue;
                }
            
                var pos = new Vector2Int(i, j);
                // check if the floor tile position is the leftmost (that has a left neighbor)
                if (!leftmost.HasValue || pos.x < leftmost.Value.x || (pos.x == leftmost.Value.x && pos.y < leftmost.Value.y))
                {
                    leftmost = pos;
                }
            }
        }
    
        return leftmost;
    }

    private void GeneratePlayer(Vector2Int? pos)
    {
        // destroy if instance exists
        if (_playerInstance != null)
        {
            Destroy(_playerInstance);
            _playerInstance = null;
        }
        
        // find leftmost tile
        if (pos.HasValue)
        {
            // convert tile pos to coordinate
            var worldPos = groundTilemap.CellToWorld(new Vector3Int(pos.Value.x, pos.Value.y, 0));
            // adjust player position to center tile (1x1 size)
            worldPos += new Vector3(0.5f, 0.5f, 0);
            
            // instantiate player
            _playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        }
        
        // set virtual camera target
        if (virtualCamera != null && _playerInstance != null)
        {
            virtualCamera.Follow = _playerInstance.transform;
        }
    }
    
    private void PlacePlayer()
    {
        // find leftmost tile
        var leftmostPos = GetLeftmostFloorPosition();
        GeneratePlayer(leftmostPos);
    }

    public void LoadPlayer(Vector2Int pos)
    {
        GeneratePlayer(pos);
    }
    
    // will wait until generation is done in the background before placing player
    private IEnumerator PlacePlayerCoroutine()
    {
        yield return new WaitUntil(IsGenerationCompleted);
        PlacePlayer();
    }
    
    public void RegeneratePlayer()
    {
        StartCoroutine(PlacePlayerCoroutine());
    }

    public GameObject GetPlayerInstance()
    {
        return _playerInstance;
    }
}
