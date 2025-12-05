using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
  public GameObject mainMenu;

  void Start()
  {
    mainMenu.SetActive(false);
  }
  // Update is called once per frame
  void Update()
  {
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
      mainMenu.SetActive(!mainMenu.activeSelf);
    }
  }

  public void CloseMenu()
  {
    mainMenu.SetActive(false);
  }
}
