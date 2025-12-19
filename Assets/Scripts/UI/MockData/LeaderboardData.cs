using UnityEngine;

public class LeaderboardData : MonoBehaviour
{
  [System.Serializable]
  public class LeaderboardEntry
  {
    public string playerName;
    public int score;
  }

  public LeaderboardEntry[] entries = new LeaderboardEntry[8]
  {
    new LeaderboardEntry { playerName = "fufufafa", score = 1500 },
    new LeaderboardEntry { playerName = "omkegas", score = 1200 },
    new LeaderboardEntry { playerName = "diddy", score = 10700 },
    new LeaderboardEntry { playerName = "etanol", score = 2300 },
    new LeaderboardEntry { playerName = "ni", score = 900 },
    new LeaderboardEntry { playerName = "cukurukuk", score = 67 },
    new LeaderboardEntry { playerName = "skibidi", score = 1620 },
    new LeaderboardEntry { playerName = "masjuki", score = 4240 },
  };
}
