using UnityEngine.UI;
using UnityEngine;

public class IndexTabController : MonoBehaviour
{
  public Image[] tabIcons;
  public GameObject[] tabContents;

  void Start()
  {
    ActivateTab(0);
  }

  public void ActivateTab(int tabIndex)
  {
    for (int i = 0; i < tabIcons.Length; i++)
    {
      if (i == tabIndex)
      {
        tabContents[i].SetActive(true);
        tabIcons[i].color = Color.white;
      }
      else
      {
        tabContents[i].SetActive(false);
        tabIcons[i].color = Color.gray;
      }
    }
  }
}
