using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class BattleData
{
    public string playerId;
    public int totalScore;
    public int battlesWon;
    public string timestamp;
    public int finalPlayerHP;
    public bool wasBossBattle;
}

public class BattleDataManager : MonoBehaviour
{
    private static BattleDataManager _instance;
    public static BattleDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("BattleDataManager");
                _instance = go.AddComponent<BattleDataManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private string saveFilePath;
    private BattleData currentBattleData;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        saveFilePath = Path.Combine(Application.persistentDataPath, "battleData.json");
        LoadData();
    }

    public void AddScore(int score)
    {
        currentBattleData.totalScore += score;
        Debug.Log($"[BattleDataManager] Score added: {score}. Total: {currentBattleData.totalScore}");
    }

    public void ResetSessionScore()
    {
        // Reset skor untuk battle baru, tapi jangan reset total battles won
        currentBattleData.totalScore = 0;
    }

    public void SaveBattleResult(int finalPlayerHP, bool isBossBattle)
    {
        currentBattleData.battlesWon++;
        currentBattleData.finalPlayerHP = finalPlayerHP;
        currentBattleData.wasBossBattle = isBossBattle;
        currentBattleData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        SaveData();
        
        Debug.Log($"[BattleDataManager] Battle saved - Score: {currentBattleData.totalScore}, " +
                  $"Battles Won: {currentBattleData.battlesWon}, Boss: {isBossBattle}");
    }

    private void SaveData()
    {
        try
        {
            string json = JsonUtility.ToJson(currentBattleData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"[BattleDataManager] Data saved to: {saveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[BattleDataManager] Failed to save: {e.Message}");
        }
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                currentBattleData = JsonUtility.FromJson<BattleData>(json);
                Debug.Log($"[BattleDataManager] Data loaded - Total Battles Won: {currentBattleData.battlesWon}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BattleDataManager] Failed to load: {e.Message}");
                InitializeNewData();
            }
        }
        else
        {
            InitializeNewData();
        }
    }

    private void InitializeNewData()
    {
        currentBattleData = new BattleData
        {
            playerId = SystemInfo.deviceUniqueIdentifier,
            totalScore = 0,
            battlesWon = 0,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            finalPlayerHP = 100,
            wasBossBattle = false
        };
        
        Debug.Log($"[BattleDataManager] New data initialized for Player ID: {currentBattleData.playerId}");
    }

    public BattleData GetCurrentData()
    {
        return currentBattleData;
    }

    public int GetCurrentScore()
    {
        return currentBattleData.totalScore;
    }
}