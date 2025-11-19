using UnityEngine;

public class PoitonSpawner : MonoBehaviour
{

    [SerializeField] private GameObject potionPrefab;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        SpawnPotion();
    }
    
    void SpawnPotion()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        Instantiate(potionPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
