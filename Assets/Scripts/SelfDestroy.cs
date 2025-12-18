using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Berapa detik sebelum object ini hancur?")]
    [SerializeField] private float lifeTime = 1f;

    void Start()
    {
        // Fungsi bawaan Unity: Destroy(object, delay)
        Destroy(gameObject, lifeTime);
    }
}