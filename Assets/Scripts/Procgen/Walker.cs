using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Walker : MonoBehaviour
{
    // Create the variables - header to keep it organized
    [Header("Tilemap References")]
    // References for tilemaps - serialized to keep it private (?)
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap backgroundTilemap;
    
    [Header("Tile References")] 
    // Reference for the tile sprites
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase[] groundTiles;
    
    [SerializeField] private TileBase backgroundTile;
    
    [Header("Generation Parameters")]
    // Variables for generating the map
    [SerializeField] private int maxIterations = 100;
    [SerializeField] private int walkersCount = 2; // numbers of walker to use - will impact the size
    [SerializeField] [Range(0f, 1f)] private float directionChangeChance = 1.0f;
    [SerializeField] private Vector2Int startPositionOffset = new(0, 0); // where worker will spawn
    [SerializeField] private Vector2Int customGridSize = new (20, 20); // what defines the size of our levels 
    [SerializeField] private int borderSize = 3; // the size of the border (inaccessible area)
    
    [Header("Level Persistence")]
    [SerializeField] private string playerId = "player1";
    [SerializeField] private int currentLevel = 1;
    
    [Header("Objects References")]
    // reference to another class
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private TeleporterHandler teleporterHandler;
    [SerializeField] private PotionSpawner potionPlacer;
    [SerializeField] private MonsterSpawner enemyPlacer;
    [SerializeField] private BossSpawner bossPlacer;
    
    // Grid and walker management variable
    private bool[,] _visitedTiles; // keep track of visited tiles
    
    private Vector2Int _gridSize; 
    // borders
    private Vector2Int _gridMin; // bottom left corner of the grid
    private Vector2Int _gridMax; // top right corner of the grid
    private Vector2Int _walkerMin; // bottom left corner of walkable area (inside border)
    private Vector2Int _walkerMax; // top right corner of walkable area (inside border)

    // buat restart pake player input (debug), tapi ntar bisa dipasang di tele trigger kl player harus pencet key buat pindah level
    public void OnRestart(InputAction.CallbackContext context)
    {
        // only regen if action is performed
        if (context.performed)
        {
            TriggerRegenerate();
        }
    }

    // defaultnya mati le, nanti bikin trigger aja ketika pindah dari level 1 ke dungeon, panggil block of code dibawah
    void Start()
    {
        // restart all level data, ntar apus kl dah beneer
        LevelRepository.Instance.DeleteAllLevelData();
        CalculateGridBounds();
        TriggerRegenerate();
        
        // disini dibikin cek, akses player manager, kalau player punya save file, load dari last position
    }
    
    // Calculate grid and walker bounds based on girdSize and borderSize
    private void CalculateGridBounds()
    {
        // ensure grid size is large enough to include borders
        var sizeX = Mathf.Max(2 * borderSize + 1, customGridSize.x);
        var sizeY = Mathf.Max(2 * borderSize + 1, customGridSize.y);
        _gridSize = new Vector2Int(sizeX, sizeY);

        // center the grid around (0,0) in tilemap coordinate
        var minX = -_gridSize.x / 2;
        var minY = -_gridSize.y / 2; 
        _gridMin = new Vector2Int(minX, minY);
        var maxX = _gridMin.x + _gridSize.x -1;
        var maxY =  _gridMin.y + _gridSize.y -1;
        _gridMax = new Vector2Int(maxX, maxY);
        
        // define walkable area (in the borderSize) by shrinking the grid by borderSize
        var walkMinX = _gridMin.x + borderSize;
        var walkMinY = _gridMin.y + borderSize;
        _walkerMin  = new Vector2Int(walkMinX, walkMinY);
        var walkMaxX = _gridMax.x - borderSize;
        var walkMaxY = _gridMax.y - borderSize;
        _walkerMax = new Vector2Int(walkMaxX, walkMaxY);
        
        // ensure walker bounds are valid (at least 1x1)
        _walkerMin.x = Mathf.Min(_walkerMin.x, _walkerMax.x);
        _walkerMin.y = Mathf.Min(_walkerMin.y, _walkerMax.y);
        _walkerMax.x = Mathf.Max(_walkerMin.x, _walkerMax.x);
        _walkerMax.y = Mathf.Max(_walkerMin.y, _walkerMax.y);
    }

    // public accessor
    public Vector2Int GetGridMin() => _gridMin;
    public Vector2Int GetGridMax() => _gridMax;
    
    // Check if position is within walkable bounds (borders)
    private bool IsWithinWalkerBounds(Vector2Int pos)
    {
        return pos.x >= _walkerMin.x && pos.x <= _walkerMax.x && 
               pos.y >= _walkerMin.y && pos.y <= _walkerMax.y;
    }
    
    private void MarkAsVisited(Vector2Int curPos, Vector2Int boundPos, Action<int, int> marker)
    {
        if (IsWithinWalkerBounds(curPos))
        {
            var arrX = curPos.x - boundPos.x;
            var arrY = curPos.y - boundPos.y;
            
            if (arrX >= 0 && arrX < _gridSize.x && arrY >= 0 && arrY < _gridSize.y)
            {
                marker(arrX, arrY);
            }
        }
    }
    
    private static Vector2Int GetRandomDirection()
    {
        var directions = new []{
            new Vector2Int(0, 1), // up
            new Vector2Int(0, -1), // down
            new Vector2Int(1, 0), // right
            new Vector2Int(-1, 0) // left
        };
        
        return directions[Random.Range(0, directions.Length)];
    }
    
    private void RandomWalker(Vector2Int startPos)
    {
        var curPos = startPos;
        MarkAsVisited(curPos, _gridMin, (arrX, arrY) => 
        {
            _visitedTiles[arrX, arrY] = true;
        });
        
        // initial random dir
        var curDirection = GetRandomDirection();
        
        // walk the walker through the walkable grid
        for (var i = 0; i < maxIterations; i++)
        {
            // move next pos
            curPos += curDirection;
            
            // clamp pos to walkable bounds
            curPos.x = Mathf.Clamp(curPos.x, _walkerMin.x, _walkerMax.x);
            curPos.y = Mathf.Clamp(curPos.y, _walkerMin.y, _walkerMax.y);
            
            // mark visited
            MarkAsVisited(curPos, _gridMin, (arrX, arrY) => 
            {
                if (!_visitedTiles[arrX, arrY])
                {
                    _visitedTiles[arrX, arrY] = true;
                }
            });
            
            // if rand smaller than chance, change direction
            if (Random.value < directionChangeChance)
            {
                curDirection = GetRandomDirection();
            }
        }
        
        // guaranteed 5x5 at the end dan start buat teleporter
        for (int ox = 0; ox <= 4; ox++)
        {
            for (int oy = 0; oy <= 4; oy++)
            {
                var p = new Vector2Int(curPos.x + ox, curPos.y + oy);

                // masih dalam batas walker
                if (p.x < _walkerMin.x || p.x > _walkerMax.x || p.y < _walkerMin.y || p.y > _walkerMax.y) continue;
                
                MarkAsVisited(p, _gridMin, (arrX, arrY) =>
                {
                    _visitedTiles[arrX, arrY] = true;
                });
            }
        }
    }
    
    private int WeightedIndex(int[] weights)
    {
        int randomValue = Random.Range(0, weights.Sum());
    
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i]) return i;
            randomValue -= weights[i];
        }

        return weights.Length - 1; 
    }
    
    private void GetRandomTiles(Vector3Int tilePos)
    {
        int[] weights = {55, 5, 5, 5, 5, 5, 5, 5, 5, 5};
        int randomIndex = WeightedIndex(weights);

        switch (randomIndex)
        {
            case 0: // 55% chance
                groundTilemap.SetTile(tilePos, groundTile);
                break;
            default: // 5% each
                groundTilemap.SetTile(tilePos, groundTiles[randomIndex - 1]);
                break;
        }
    }
    
    // populate tilemaps (like the sprite into the game map)
    private void FillTilemaps()
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                // map array indices to tilemap coordinate
                var tilePos = new Vector3Int(i + _gridMin.x, j + _gridMin.y, 0);
                if (_visitedTiles[i, j])
                {
                    // place the floor tile on the ground
                    GetRandomTiles(tilePos);
                }
                else
                {
                    backgroundTilemap.SetTile(tilePos, backgroundTile);
                }
            }
        }
    }
    
    private void GenerateLevel()
    {
        // clear maps
        groundTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();
        
        // init visited tiles array
        _visitedTiles = new bool[_gridSize.x, _gridSize.y];
        
        // calculate walker's starting position
        var startX = _walkerMin.x + (_walkerMax.x - _walkerMin.x) / 2 + startPositionOffset.x;
        var startY  = _walkerMin.y + (_walkerMax.y - _walkerMin.y) / 2  + startPositionOffset.y;
        var startPos = new Vector2Int(startX, startY);
        
        // clamp startPos to walkable bound
        startPos.x = Mathf.Clamp(startPos.x, _walkerMin.x, _walkerMax.x);
        startPos.y = Mathf.Clamp(startPos.y, _walkerMin.y, _walkerMax.y);
        
        // run the algorithm for each worker
        for (int i = 0; i < walkersCount; i++)
        {
            RandomWalker(startPos);
        }
        
        // populate tilemaps
        FillTilemaps();
    }
    
    private void SaveCurrentLevel()
    {
        if (teleporterHandler == null) return;
        
        LevelData levelData = new LevelData
        {
            levelId = LevelRepository.Instance.GetLevelId(playerId, currentLevel),
            playerId = playerId,
            level = currentLevel,
            groundTiles = GetGroundTilesList(),
            entryDoor = teleporterHandler.EntryDoor,
            exitDoor = teleporterHandler.ExitDoor
        };
        
        LevelRepository.Instance.SaveLevel(levelData);
    }
    
    private List<TileData> GetGroundTilesList()
    {
        List<TileData> tiles = new List<TileData>();
        
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                if (_visitedTiles[i, j])
                {
                    tiles.Add(new TileData
                    {
                        x = i + _gridMin.x,
                        y = j + _gridMin.y,
                        tileId = GetTileIdAt(new Vector3Int(i + _gridMin.x, j + _gridMin.y, 0))
                    });
                }
            }
        }
        
        return tiles;
    }
    
    private int GetTileIdAt(Vector3Int pos)
    {
        TileBase tile = groundTilemap.GetTile(pos);
        if (tile == groundTile) return 0;
        
        for (int i = 0; i < groundTiles.Length; i++)
        {
            if (groundTiles[i] == tile) return i + 1;
        }
        
        return 0;
    }
    
    private void GenerateAndSaveWhenDoorsReady()
    {
        // local handler
        void OnPlaced()
        {
            teleporterHandler.OnTeleporterPlaced -= OnPlaced; // unsubscribe once
            SaveCurrentLevel();
        }

        teleporterHandler.OnTeleporterPlaced += OnPlaced;
        teleporterHandler.GenerateDoor();
    }
    
    // Load existing level
    private void LoadLevel(int levelNumber, int levelIncrement)
    {
        LevelData data = LevelRepository.Instance.LoadOrNull(playerId, levelNumber);
        Debug.Log($"[Walker] LoadLevel called for level {levelNumber}, dataNull={data==null}");

        if (data != null && data.groundTiles.Count > 0)
        {
            Debug.Log("[Walker] Loading from saved data");
            LoadFromLevelData(data, levelIncrement > 0);
        }
        else
        {
            Debug.Log("[Walker] No saved data -> generating new level");
            TriggerRegenerate();
        }
    }

    private TileBase GetTileFromId(int id)
    {
        if (id == 0) return groundTile;
        if (id > 0 && id <= groundTiles.Length) return groundTiles[id - 1];
        return groundTile;
    }
    
    private void LoadFromLevelData(LevelData data, bool isExit)
    {
        groundTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();
        
        foreach (TileData tileData in data.groundTiles)
        {
            Vector3Int pos = new Vector3Int(tileData.x, tileData.y, 0);

            // place ground tile
            groundTilemap.SetTile(pos, GetTileFromId(tileData.tileId));
        }
        
        // place background tile
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                var pos = new Vector3Int(i + _gridMin.x, j + _gridMin.y, 0);
                if (!groundTilemap.HasTile(pos))
                {
                    // place the background tile on the ground
                    backgroundTilemap.SetTile(pos , backgroundTile);
                }
            }
        }
        
        // restore doors, player position, etc.
        if (teleporterHandler != null)
        {
            teleporterHandler.LoadDoor(data.entryDoor, data.exitDoor);
        }
        
        if (enemyPlacer != null)
        {
            enemyPlacer.PlaceMonstersFromData(data.enemyPositions);
        }

        if (bossPlacer != null)
        {
            bossPlacer.PlaceBossFromData(data.bossPosition);
        }
        
        if (potionPlacer != null)
        {
            potionPlacer.PlacePotionsFromData(data.potionPositions);
        }

        if (playerHandler != null)
        {
            // kalau exit (atas ke bawah), player di kanan entry
            // kalau entry (bawah ke atas), player di kiri exit
            if (isExit)
            {
                playerHandler.LoadPlayer(new Vector2Int(data.entryDoor.x, data.entryDoor.y) + Vector2Int.right);
            }
            else
            {
                playerHandler.LoadPlayer(new Vector2Int(data.exitDoor.x, data.exitDoor.y) + Vector2Int.left);
            }
        }
    }
    
    private void TriggerRegenerate()
    {
        GenerateLevel();
        Debug.Log($"[Walker] enemyPlacer null? {enemyPlacer == null}");
        Debug.Log($"[Walker] potionPlacer null? {potionPlacer == null}");
        Debug.Log($"[Walker] Trigger regeneration for level {currentLevel}");
        Debug.Log($"[Walker] bossPlacer null? {bossPlacer == null}");
        
        // trigger placements for the first time
        if (enemyPlacer != null)
        {
            Debug.Log($"[Walker] Triggering enemy placement for level {currentLevel}");
            enemyPlacer.OnLevelGenerated();
        }

        if (potionPlacer != null)
        {
            Debug.Log($"[Walker] Triggering potion placement for level {currentLevel}");
            potionPlacer.OnLevelGenerated();
        }
        
        if (playerHandler != null)
        {
            playerHandler.RegeneratePlayer();
        }

        if (teleporterHandler != null)
        {
            GenerateAndSaveWhenDoorsReady();
        }

        Debug.Log("[Walker] Is this called?");
        if (bossPlacer != null)
        {
            bossPlacer.OnLevelGenerated();
        }
    }
    
    private void SaveObjects()
    {
        Debug.Log($"[Walker] Saving objects for level {currentLevel}");
        if (potionPlacer != null && enemyPlacer != null)
        {
            var potionData = potionPlacer.GetPotionData();
            var monsterData = enemyPlacer.GetMonsterData();
            var bossData = bossPlacer.GetBossPosition();
            
            Debug.Log($"[Walker] Saving level {currentLevel} with {potionData.Count} potions and {monsterData.Count} monsters");
            LevelRepository.Instance.SaveLevelObjects(LevelRepository.Instance.GetLevelId(playerId, currentLevel), potionData, monsterData, bossData);
        }
    }
    
    public void TriggerLoad(int levelIncrement)
    {
        CalculateGridBounds();
        
        // Save the current level's objects before loading the next level
        SaveObjects();
        
        var level = currentLevel + levelIncrement;
        if (level < 1)
        {
            return;
        }

        currentLevel = level;
        LoadLevel(currentLevel, levelIncrement);
    }
}