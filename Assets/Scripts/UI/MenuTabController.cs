using UnityEngine.UI;
using UnityEngine;

public class MenuTabController : MonoBehaviour
{
  public Image[] tabIcons;
  public GameObject[] tabContents;
  public GameObject mainMenu;
  public GameObject mainTab;

  void Start()
  {
    for (int i = 0; i < tabContents.Length; i++)
    {
      tabContents[i].SetActive(false);
      mainTab.SetActive(false);
    }
  }

  public void ActivateTab(int tabIndex)
  {
    mainTab.SetActive(true);
    for (int i = 0; i < tabIcons.Length; i++)
    {
      if (i == tabIndex)
      {
        tabContents[i].SetActive(true);
      }
      else
      {
        tabContents[i].SetActive(false);
      }
    }
    mainMenu.SetActive(false);
  }

  public void CloseTab(int tabIndex)
  {
    tabContents[tabIndex].SetActive(false);
    mainMenu.SetActive(true);
    mainTab.SetActive(false);
  }
}
