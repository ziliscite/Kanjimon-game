using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Enabler()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<PlayerAttack>().enabled = true;
        gameObject.GetComponent<PlayerMovement>().enabled = true;
    }

    public void Disabler()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<PlayerAttack>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
    }
}
