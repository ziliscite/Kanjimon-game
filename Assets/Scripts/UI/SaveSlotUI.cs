using UnityEngine;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
  [SerializeField] TMP_Text playerName;
  [SerializeField] TMP_Text saveDate;
  [SerializeField] TMP_Text playTimeMinutes;
  [SerializeField] TMP_Text level;
  [SerializeField] TMP_Text points;
  [SerializeField] TMP_Text progressCount;

  public void Bind(SaveSlotData.SaveData data)
  {
    playerName.text = data.playerName;
    saveDate.text = data.saveDate;
    playTimeMinutes.text = HourMinuteFormat(data.playTimeMinutes);
    level.text = $"Dungeon - {data.level}";
    points.text = $"{data.points} Pts";
    progressCount.text = (data.level / 10f * 100).ToString("F0");
  }
  string HourMinuteFormat(int totalMinutes)
  {
    int hours = totalMinutes / 60;
    int minutes = totalMinutes % 60;
    return $"{hours:D2}:{minutes:D2}";
  }
}


