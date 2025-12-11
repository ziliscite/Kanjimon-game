using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    // whether it'll teleport up (-1) or down (1)
    private int _direction;
    private Walker _walker;

    public void SetWalker(Walker walkerRef)
    {
        _walker = walkerRef;
    }
    
    public void SetDirection(int direction)
    {
        _direction = direction;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (_walker != null)
            {
                _walker.TriggerLoad(_direction);
            }
        }
    }
}
