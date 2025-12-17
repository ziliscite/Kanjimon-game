using UnityEngine;
using UnityEngine.UI;

public class OnChangePlayerStatus : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text text;
  [SerializeField] private Slider slider;

  [SerializeField] private GameObject player;
  [SerializeField] private TMPro.TMP_Text potionCount;

  public void UpdatePlayerStatus()
  {
    // HP
    int maxValue = player.GetComponent<Player>().maxHitPoints;
    int currentValue = player.GetComponent<Player>().hitPoints;
    text.text = (currentValue + " / " + maxValue).ToString();
    slider.value = currentValue;
    // Potions
    potionCount.text = "x" + player.GetComponent<Player>().potions.ToString();
  }

  void Start()
  {
    slider.maxValue = player.GetComponent<Player>().maxHitPoints;
    UpdatePlayerStatus();
  }

  void Update()
  {
    UpdatePlayerStatus();
  }
}
