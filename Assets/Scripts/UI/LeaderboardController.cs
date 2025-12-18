using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
  [SerializeField] private GameObject slotPrefab;
  [SerializeField] private Transform slotContainer;
  [SerializeField] private GameObject leaderboardDataObject;
  [SerializeField] private int maxEntries = 6;
  private LeaderboardData.LeaderboardEntry[] entries;


  void Start()
  {
    if (leaderboardDataObject != null)
    {
      entries = leaderboardDataObject.GetComponent<LeaderboardData>().entries;
      System.Array.Sort(entries, (x, y) => y.score.CompareTo(x.score));
    }

    //instantiate
    for (int i = 0; i < maxEntries; i++)
    {
      if (entries[i] != null)
      {
        GameObject newEntry = Instantiate(slotPrefab, slotContainer);
        newEntry.GetComponent<LeaderboardSlotUI>().Bind(entries[i]);
        newEntry.SetActive(true);
        switch (i)
        {
          case 0:
            {
              newEntry.GetComponent<LeaderboardSlotUI>().rankText.color = Color.yellow;
              newEntry.GetComponent<RectTransform>().localScale = Vector3.one * 1.1f;
              break;
            }
          case 1:
            {
              newEntry.GetComponent<LeaderboardSlotUI>().rankText.color = Color.gray;
              break;
            }
          case 2:
            {
              newEntry.GetComponent<LeaderboardSlotUI>().rankText.color = new Color(0.8f, 0.5f, 0.2f);
              break;
            }
        }
      }
    }
  }
}
