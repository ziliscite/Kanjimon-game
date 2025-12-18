using UnityEngine;
using System.Collections;

public class testkeprocgen : MonoBehaviour
{
    public void awawaw()
    {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("ProcgenScene");
        yield return new WaitForSeconds(1f);
        // PlayerInstance.instance.Disabler();
    }
}
