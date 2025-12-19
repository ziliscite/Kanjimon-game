using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class SaveManager
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "kanjimon_player.json");

    public static void SaveGame()
    {
        // 1. AMBIL DATA DARI PLAYER MANAGER
        PlayerManager pm = PlayerManager.Instance;
        
        if (pm == null)
        {
            Debug.LogError("[SaveManager] PlayerManager Instance is null! Cannot save.");
            return;
        }

        // 2. SAVE STATE LEVEL TERLEBIH DAHULU (PENTING!)
        // Ini akan menyimpan sisa potion/monster ke file JSON level
        SaveCurrentLevelState(pm);

        // 3. SUSUN DATA PLAYER
        SaveData data = new SaveData
        {
            playerHP = pm.playerHP,
            potionsLeft = pm.potionsLeft,
            expPlayer = pm.expPlayer,
            playerLevel = pm.playerLevel,

            posX = pm.transform.position.x,
            posY = pm.transform.position.y,
            posZ = pm.transform.position.z,

            // Pastikan variabel ini ada di PlayerManager kamu
            currentFloor = pm.currentLevelIndex, 
            
            enemyBattledID = pm.enemyBattledID,
            sceneName = SceneManager.GetActiveScene().name,
            timestamp = System.DateTime.Now.ToString()
        };

        // 4. TULIS KE FILE PLAYER
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("[SaveManager] Player & World State Saved Successfully");
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("[SaveManager] No save file found");
            return null;
        }

        try 
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Error loading save: {e.Message}");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
        // Opsional: Reset Level Data juga agar bersih total
        if (LevelRepository.Instance != null)
        {
             LevelRepository.Instance.DeleteAllLevelData();
        }
    }

    // --- PRIVATE HELPER METHODS ---

    private static void SaveCurrentLevelState(PlayerManager pm)
    {
        // 1. Cek apakah kita di scene Procgen (Gameplay)
        // Jika di Main Menu, jangan jalankan logic ini karena LevelInstanceManager tidak ada
        if (LevelInstanceManager.Instance == null) 
        {
            // Debug Log ini berguna untuk info, bukan Error
            Debug.Log("[SaveManager] LevelInstanceManager not found (Maybe in Menu?). Skipping level object save.");
            return;
        }

        Debug.Log("[SaveManager] Gathering level objects data...");

        // 2. Ambil data real-time dari scene lewat LevelInstanceManager (Script Jembatan)
        List<PotionPosition> currentPotions = LevelInstanceManager.Instance.GetCurrentPotionData();
        List<EnemyPosition> currentEnemies = LevelInstanceManager.Instance.GetCurrentMonsterData();
        BossPosition currentBoss = LevelInstanceManager.Instance.GetCurrentBossData();

        // 3. Generate ID Level yang unik (Contoh: "PlayerUUID_level_5")
        string currentLevelId = LevelRepository.Instance.GetLevelId(pm.playerId, pm.currentLevelIndex);
        
        // 4. Kirim ke LevelRepository untuk update file JSON level tersebut
        LevelRepository.Instance.SaveLevelObjects(
            currentLevelId, 
            currentPotions, 
            currentEnemies, 
            currentBoss
        );
    }
}