using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

[Serializable] public class TileData {
    public int x;
    public int y;
    public int tileId;
}

[Serializable] public class TeleporterPosition
{
    public int x;
    public int y;
    public int direction; // 1 = down / exit, -1 = up / entry
}

[Serializable] public class PotionPosition {
    public int x;
    public int y;
}

[Serializable] public class EnemyPosition {
    public int x;
    public int y;
    public int enemyId;
    public int enemyInstanceIndex;
}

[Serializable] public class BossPosition
{
    public int x;
    public int y;
    public int enemyId;
}

[Serializable] public class LevelData
{
    public string levelId;
    public string playerId; // uuid
    public int level;
    public List<TileData> groundTiles;
    public TeleporterPosition entryDoor;
    public TeleporterPosition exitDoor;
    public List<PotionPosition> potionPositions;
    public List<EnemyPosition> enemyPositions;
    public BossPosition bossPosition;
}

public class LevelRepository : MonoBehaviour
{
    public static LevelRepository Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    // Check if a level file exists
    private bool LevelExists(string levelId)
    {
        string filePath = GetFilePath(levelId);
        bool exists = File.Exists(filePath);
        Debug.Log($"[LevelManager] Checking if level exists - ID: {levelId}, Path: {filePath}, Exists: {exists}");
        return exists;
    }

    // Generate consistent level ID
    public string GetLevelId(string playerId, int levelNumber)
    {
        string levelId = $"{playerId}_level_{levelNumber}";
        Debug.Log($"[LevelManager] Generated Level ID: {levelId} for Player: {playerId}, Level: {levelNumber}");
        return levelId;
    }
    
    public void SaveLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("[LevelManager] Cannot save level: LevelData is null");
            return;
        }

        try
        {
            string filePath = GetFilePath(levelData.levelId);
            string directory = Path.GetDirectoryName(filePath);
            
            if (string.IsNullOrEmpty(directory))
            {
                Debug.LogError($"[LevelManager] Invalid directory path for level {levelData.levelId}");
                return;
            }
            
            Debug.Log($"[LevelManager] Saving level {levelData.levelId} to {filePath}");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(directory))
            {
                Debug.Log($"[LevelManager] Directory {directory} does not exist, creating...");
                Directory.CreateDirectory(directory);
                Debug.Log($"[LevelManager] Created directory: {directory}");
            }
            
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"[LevelManager] Successfully saved level: {levelData.levelId} to {filePath}");
        }
        catch (Exception e)
        {
            Debug.Log($"[LevelManager] Failed to save level {levelData.levelId}: {e.Message}");
            Debug.LogException(e);
        }
    }

    // load old level by id, modify potionData and monsterData, then save
    public void SaveLevelObjects(string levelId, List<PotionPosition> potionData, List<EnemyPosition> monsterData, BossPosition bossData)
    {
        LevelData levelData = LoadLevel(levelId);
        if (levelData == null)
        {
            Debug.Log($"[LevelManager] Level {levelId} not found");
            return;
        }

        levelData.potionPositions = potionData;
        levelData.enemyPositions = monsterData;
        levelData.bossPosition = bossData;
        
        Debug.Log($"[LevelManager] Saving level {levelId} with {potionData.Count} potions and {monsterData.Count} monsters");
        SaveLevel(levelData);
    }
    
    private LevelData LoadLevel(string levelId)
    {
        try
        {
            string filePath = GetFilePath(levelId);
            if (!File.Exists(filePath))
            {
                Debug.Log($"[LevelManager] Level file not found: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            Debug.Log($"[LevelManager] Loaded level {levelId}: tiles={levelData.groundTiles?.Count ?? 0}, entry=({levelData.entryDoor.x},{levelData.entryDoor.y}), exit=({levelData.exitDoor.x},{levelData.exitDoor.y}), potions={levelData.potionPositions?.Count ?? 0}, monsters={levelData.enemyPositions?.Count ?? 0}");
            return levelData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[LevelManager] Failed to load level: {e.Message}");
            return null;
        }
    }

    
    // Load or create new level
    public LevelData LoadOrNull(string playerId, int levelNumber)
    {
        string levelId = GetLevelId(playerId, levelNumber);
        if (LevelExists(levelId))
        {
            return LoadLevel(levelId);
        }

        return null;
    }
    
    private string GetFilePath(string levelId)
    {
        return Path.Combine(Application.persistentDataPath, $"map222/level_{levelId}.json");
    }
    
    public void DeleteAllLevelData()
    {
        string directory = Path.Combine(Application.persistentDataPath, "map222");
        Debug.Log($"[LevelManager] Attempting to delete all level data in: {directory}");
        
        if (!Directory.Exists(directory))
        {
            Debug.Log($"[LevelManager] Directory does not exist, nothing to delete: {directory}");
            return;
        }
        
        string[] files = Directory.GetFiles(directory, "level_*.json");
        Debug.Log($"[LevelManager] Found {files.Length} level files to delete");
        
        int deletedCount = 0;
        foreach (string file in files)
        {
            try
            {
                Debug.Log($"[LevelManager] Deleting file: {file}");
                File.Delete(file);
                deletedCount++;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LevelManager] Failed to delete file {file}: {e.Message}");
            }
        }
        
        Debug.Log($"[LevelManager] Deleted {deletedCount} out of {files.Length} level files");
    }
}
