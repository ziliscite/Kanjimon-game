using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
  [SerializeField] private GameObject settingsValue;

  [SerializeField] private Toggle musicToggle;
  [SerializeField] private Toggle sfxToggle;

  [SerializeField] private Slider musicSlider;
  [SerializeField] private Slider sfxSlider;

  [SerializeField] private GameObject MusicOnIcon;
  [SerializeField] private GameObject MusicOffIcon;
  [SerializeField] private GameObject SfxOnIcon;
  [SerializeField] private GameObject SfxOffIcon;

  private settingsValue sv;
  private bool isInitialized = false;

  void Start()
  {
    sv = settingsValue.GetComponent<settingsValue>();
    musicSlider.value = sv.musicValue;
    sfxSlider.value = sv.sfxValue;
    musicToggle.isOn = sv.isMusicOn;
    sfxToggle.isOn = sv.isSfxOn;
    IconToggle();
    isInitialized = true;
  }
  public void ApplySettings()
  {
    if (!isInitialized) return;
    sv.isMusicOn = musicToggle.isOn;
    sv.isSfxOn = sfxToggle.isOn;
    sv.musicValue = (int)musicSlider.value;
    sv.sfxValue = (int)sfxSlider.value;
    IconToggle();
  }

  public void IconToggle()
  {
    MusicOnIcon.SetActive(musicToggle.isOn);
    MusicOffIcon.SetActive(!musicToggle.isOn);

    SfxOnIcon.SetActive(sfxToggle.isOn);
    SfxOffIcon.SetActive(!sfxToggle.isOn);
  }
}
