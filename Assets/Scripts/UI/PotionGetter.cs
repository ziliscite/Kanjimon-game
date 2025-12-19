using UnityEngine;
using TMPro;

public class PotionGetter : MonoBehaviour
{
    [SerializeField] private TMP_Text potionText;
    private int _potionsLeft;

    void Start()
    {
        // Safe to access PlayerManager here
        _potionsLeft = PlayerManager.Instance.potionsLeft;
        potionText.text = _potionsLeft.ToString();
    }

    // Optional: if potions can change during gameplay, you might want an Update method or a function to refresh the UI:
    public void RefreshPotionUI()
    {
        _potionsLeft = PlayerManager.Instance.potionsLeft;
        potionText.text = _potionsLeft.ToString();
    }
}
