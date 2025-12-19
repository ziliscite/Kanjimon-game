using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartButton()
    {
        SaveManager.SaveGame();   // 🔒 bikin save dulu
        SceneManager.LoadScene("ProcgenScene"); // lanjut main
    }
}
