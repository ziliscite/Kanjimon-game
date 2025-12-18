using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private Walker walker;
    [SerializeField] private Tilemap groundTilemap;
    
    // check if ground tile generation is complete
    public bool IsGenerationCompleted()
    {
        if (walker == null || groundTilemap == null) return false;

        var gridMin = walker.GetGridMin();
        var gridMax = walker.GetGridMax();
        
        for (int i = gridMin.x; i <= gridMax.x; i++)
        {
            for (int j = gridMin.y; j <= gridMax.y; j++)
            {
                if (groundTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    // retrieve all floor tile position for ground tilemap
    public List<Vector3Int> GetFloorPositions()
    {
        var floorPositions = new List<Vector3Int>();
        
        var gridMin = walker.GetGridMin();
        var gridMax = walker.GetGridMax();
        
        // scan ground tilemap within grid bounds
        for (int i = gridMin.x; i <= gridMax.x; i++)
        {
            for (int j = gridMin.y; j <= gridMax.y; j++)
            {
                if (groundTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    floorPositions.Add(new Vector3Int(i, j, 0));
                }
            }
        }
        
        return floorPositions;
    }
    
    // convert tile position to world position (centered)
    public Vector3 TileToWorldPosition(Vector3Int tilePos)
    {
        if (groundTilemap == null) return Vector3.zero;
        return groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
    }
    
    // getters
    public Walker GetWalker()
    {
        return walker;
    }
    
    public Tilemap GetGroundTilemap()
    {
        return groundTilemap;
    }
}