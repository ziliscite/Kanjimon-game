using UnityEngine;
using UnityEngine.UI;

public class OnChangeValue : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text text;

  public void UpdateHP(GameObject valueObject)
  {
    int maxValue = 100;
    float value = valueObject.GetComponent<Slider>().value;
    text.text = (value + " / " + maxValue).ToString();
  }
}
