using UnityEngine;

public class SaveSlotData : MonoBehaviour
{
  [System.Serializable]
  public class SaveData
  {
    public string playerName;
    public string saveDate;
    public int playTimeMinutes;
    public int level;
    public int points;
    public int slotIndex;
  }

  public SaveData[] slotData = new SaveData[3];

  //init data
  void Awake()
  {
    for (int i = 0; i < slotData.Length; i++)
    {
      slotData[i] = new SaveData
      {
        playerName = $"Player{i + 1}",
        saveDate = System.DateTime.Now.ToString("dd-MM-yyyy"),
        playTimeMinutes = 100 * (i + 1),
        level = i + 1,
        points = 1540 * (i + 1),
        slotIndex = i
      };
    }
    slotData[0] = null;
  }
}
