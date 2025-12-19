using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTabController : MonoBehaviour
{
  public Image[] tabIcons;
  public GameObject[] tabContents;
  public GameObject mainMenu;
  public GameObject mainTab;
  public bool isTabActive = false;
  private int currentTabIndex;

  void Update()
  {
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
      if (!isTabActive)
        mainMenu.SetActive(!mainMenu.activeSelf);
      else
        CloseTab(currentTabIndex);
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
        isTabActive = true;
        currentTabIndex = tabIndex;
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
    isTabActive = false;
    currentTabIndex = -1;
  }

  public void CloseMenu()
  {
    mainMenu.SetActive(false);
  }

    public void QuitFromButton()
  {
      Application.Quit();
  }
}
