using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ObjectPlacer : MonoBehaviour
{
    // prefab to object we want to place
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int prefabNumber = 5;
    
    [SerializeField] private Walker walker;
    // access to tilemap for the player to be placed on
    [SerializeField] private Tilemap groundTilemap;
    
    // the array aint changing, the child is, so readonly
    private readonly List<GameObject> _objectInstances = new();

    // check if ground tile generation is complete
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
    
    // clear all previously placed objects
    private void ClearPlacedObjects()
    {
        foreach (var instance in _objectInstances)
        {
            if (instance != null) Destroy(instance);
        }
        _objectInstances.Clear();
    }
    
    // retrieve all floor tile position for ground tilemap
    private List<Vector3Int> GetFloorPositions()
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

    // place object prefab randomly
    private void PlaceObjects()
    {
        ClearPlacedObjects();
        if (objectPrefab == null || walker == null || groundTilemap == null) return;
        
        // get all tile pos
        var floorPositions = GetFloorPositions();
        if (floorPositions.Count == 0) return;
        
        // place object prefab randomly
        for (int i = 0; i < Mathf.Min(prefabNumber, floorPositions.Count); i++)
        {
            // pick random floor pos
            var randomIndex = Random.Range(0, floorPositions.Count);
            var tilePos = floorPositions[randomIndex];
            
            // convert to world pos, also center it on tile
            var worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
            
            var instance = Instantiate(objectPrefab, worldPos, Quaternion.identity);
            _objectInstances.Add(instance);
            
            // make sure no overlap, remove used pos
            floorPositions.RemoveAt(randomIndex);
        }
    }
    
    // wait till level gen is done (like other skrip ye)
    private IEnumerator PlaceObjectsCoroutine()
    {
        yield return new WaitUntil(IsGenerationCompleted);
        PlaceObjects();
    }
    
    // pub method to regen potions, called by walker on new level gen
    public void OnLevelGenerated()
    {
        StartCoroutine(PlaceObjectsCoroutine());
    }

    // start in the beninging
    private void Start()
    {
        OnLevelGenerated();
    }
}
