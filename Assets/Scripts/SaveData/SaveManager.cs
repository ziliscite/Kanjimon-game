using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    private static string savePath =>
        Application.persistentDataPath + "/kanjimon_save.json";

    public static void SaveGame()
    {
        PlayerManager pm = PlayerManager.instance;

        SaveData data = new SaveData
        {
            playerHP = pm.playerHP,
            potionsLeft = pm.potionsLeft,
            expPlayer = pm.expPlayer,
            playerLevel = pm.playerLevel,

            posX = pm.lastPosition.x,
            posY = pm.lastPosition.y,
            posZ = pm.lastPosition.z,

            lastFloor = pm.lastFloor,
            enemyBattledID = pm.enemyBattledID,

            sceneName = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved Succesfully");
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found");
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
