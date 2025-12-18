using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Penting buat Hover

// Script ini otomatis nambahin suara ke tombol tempat dia nempel, kasih ini ke semua button yh king
public class UISound : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private string clickSoundName = "ClickSound";
    [SerializeField] private string hoverSoundName = "Hover";
    [SerializeField] private bool useHoverSound = true;

    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cek kalau tombolnya Interactable (gak mati/grayed out)
        if (btn != null && !btn.interactable) return;

        SoundManager.Instance.PlaySFX("ClickSound");
    }

    // 2. Logic Hover (Pas mouse lewat di atas tombol)
    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     if (useHoverSound && btn != null && btn.interactable)
    //     {
    //         // Pake PlaySFX biasa (atau mau PlaySFXRandomPitch juga boleh)
    //         SoundManager.Instance.PlaySFX(hoverSoundName);
    //     }
    // }
}