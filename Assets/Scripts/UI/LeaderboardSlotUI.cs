using TMPro;
using UnityEngine;

public class LeaderboardSlotUI : MonoBehaviour
{
  [SerializeField] public TMP_Text rankText;
  [SerializeField] private TMP_Text playerNameText;
  [SerializeField] private TMP_Text scoreText;

  public void Bind(LeaderboardData.LeaderboardEntry data)
  {
    playerNameText.text = data.playerName;
    scoreText.text = $"{data.score} Pts";
    rankText.text = $"{transform.GetSiblingIndex() + 1}.";
  }
}
