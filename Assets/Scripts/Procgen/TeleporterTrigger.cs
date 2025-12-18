using UnityEngine;
using UnityEngine.InputSystem;

public class TeleporterTrigger : MonoBehaviour
{
    private Walker _walker;
    private int _direction;
    private bool _playerInTrigger;
    private string _triggerId; // Add this!
    private PlayerInput _playerInput;
    private InputAction _interactAction;
    
    void Start()
    {
        // Find player input
        _playerInput = FindFirstObjectByType<PlayerInput>();
        if (_playerInput != null)
        {
            _interactAction = _playerInput.actions["Interact"];
        }
    }
    
    void Awake()
    {
        _triggerId = gameObject.GetInstanceID().ToString();
    }
    
    public void SetDirection(int direction)
    {
        Debug.Log($"[{_triggerId}] SetDirection: {direction}");
        _direction = direction;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{_triggerId}] Player ENTERED - _playerInTrigger = true");
            _playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{_triggerId}] Player EXITED - _playerInTrigger = false");
            _playerInTrigger = false;
        }
    }

    void Update()
    {
        if (_playerInTrigger && _interactAction != null && _interactAction.WasPressedThisFrame())
        {
            if (_direction == 1 && BossManager.Instance != null && !BossManager.Instance.IsBossDead())
            {
                return;
            }
            
            _walker = FindFirstObjectByType<Walker>();
            if (_walker != null)
            {
                Debug.Log($"[{gameObject.name}] TELEPORTING");
                _walker.TriggerLoad(_direction);
            }
        }
    }
}