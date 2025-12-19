using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DungeonDoor"))
            ScreenFader.Instance.FadeToScene("ProcgenScene");
    }
}
