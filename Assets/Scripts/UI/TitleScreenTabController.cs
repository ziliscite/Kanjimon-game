using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreenTabController : MonoBehaviour
{
  public Image[] tabIcons;
  public GameObject[] tabContents;
  public GameObject mainTab;
  private int currentTabIndex;

  public void ActivateTab(int tabIndex)
  {
    mainTab.SetActive(true);
    currentTabIndex = tabIndex;
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
    
  }

  void Update()
  {
    if (Keyboard.current.escapeKey.wasPressedThisFrame && currentTabIndex != -1)
    {
      CloseTab(currentTabIndex);
    }
  }
  public void CloseTab(int tabIndex)
  {
    tabContents[tabIndex].SetActive(false);
    mainTab.SetActive(false);
    currentTabIndex = -1;
  }
  
  public void QuitFromButton()
  {
      Application.Quit();
  }
}
