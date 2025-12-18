using UnityEngine;

public class LoadController : MonoBehaviour
{
  [SerializeField] private GameObject[] loadSlots;
  [SerializeField] private GameObject saveSlotData;
  private SaveSlotData.SaveData[] slotData;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if (saveSlotData != null)
    {
      slotData = saveSlotData.GetComponent<SaveSlotData>().slotData;
      BuildSlots();
    }
  }
  void BuildSlots()
  {
    for (int index = 0; index < slotData.Length; index++)
    {
      if (slotData[index] != null && slotData[index].playerName != null)
      {
        loadSlots[index].GetComponent<SaveSlotUI>().Bind(slotData[index]);
      }
    }
  }


  // Update is called once per frame
  void Update()
  {

  }
}
