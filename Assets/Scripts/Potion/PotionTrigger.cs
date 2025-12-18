using UnityEngine;

public class PotionTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.potionsLeft++;
            Potion.PotionCount++;
            Destroy(gameObject);
        }
        
        Debug.Log($"Potion count: {Potion.PotionCount}");
    }
}