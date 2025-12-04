using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
    public void EnterBattle()
    {
        SceneManager.LoadScene("Battle Scene");
    }
}
