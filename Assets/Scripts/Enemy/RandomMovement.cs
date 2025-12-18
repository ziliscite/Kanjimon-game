using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    [SerializeField] private float maxDistance = 1.5f; // maximum distance from origin
    [SerializeField] private float speed = 1f; // movement speed
    [SerializeField] private float directionChangeTime = 0.5f; // time before randomly changing direction
    
    private Vector2 _originalPosition;
    private Vector2 _currentDirection;
    private Rigidbody2D _rb;
    private float _directionTimer;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;

    
    // Cardinal directions
    private readonly Vector2[] _cardinalDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        _originalPosition = _rb.position;
        
        // Start with random direction
        ChooseNewDirection();
        _directionTimer = directionChangeTime;
    }

    private void FixedUpdate()
    {
        // Timer for periodic direction changes
        _directionTimer -= Time.fixedDeltaTime;
        if (_directionTimer <= 0f)
        {
            ChooseNewDirection();
            _directionTimer = directionChangeTime;
        }

        // Calculate parent offset
        var parentPosition = transform.parent != null ? (Vector2)transform.parent.position : Vector2.zero;
        var targetOrigin = _originalPosition + parentPosition;
        
        // Check if we're at max distance from origin
        float distanceFromOrigin = Vector2.Distance(_rb.position, targetOrigin);
        if (distanceFromOrigin >= maxDistance)
        {
            // Turn back towards origin
            Vector2 directionToOrigin = (targetOrigin - _rb.position).normalized;
            _currentDirection = GetClosestCardinalDirection(directionToOrigin);
        }
        
        // Move in current direction
        _rb.linearVelocity = _currentDirection * speed;

        UpdateAnimator();
        UpdateFlip();
    }

    private void UpdateAnimator()
    {
        // bool isMoving = _currentDirection != Vector2.zero;
        bool isMoving = _rb.linearVelocity.sqrMagnitude > 0.01f;

        _animator.SetBool("isMoving", isMoving);
        _animator.SetFloat("moveX", _currentDirection.x);
        // _animator.SetFloat("moveY", _currentDirection.y);
    }

    private void UpdateFlip()
    {
        if (_currentDirection.x != 0) 
            _sprite.flipX = _currentDirection.x < 0;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stop briefly
        _rb.linearVelocity = Vector2.zero;
        
        // Choose a new direction away from the collision
        Vector2 collisionNormal = collision.contacts[0].normal;
        ChooseNewDirectionAwayFrom(-collisionNormal);
        
        // Reset timer
        _directionTimer = directionChangeTime;
    }
    
    private void ChooseNewDirection()
    {
        // Pick a random cardinal direction
        _currentDirection = _cardinalDirections[Random.Range(0, _cardinalDirections.Length)];
    }
    
    private void ChooseNewDirectionAwayFrom(Vector2 avoid)
    {
        // Find cardinal directions that are not towards the collision
        var validDirections = new System.Collections.Generic.List<Vector2>();
        
        foreach (var dir in _cardinalDirections)
        {
            if (Vector2.Dot(dir, avoid) <= 0) // Not pointing towards collision
            {
                validDirections.Add(dir);
            }
        }
        
        if (validDirections.Count > 0)
        {
            _currentDirection = validDirections[Random.Range(0, validDirections.Count)];
        }
        else
        {
            // Fallback: choose opposite direction
            _currentDirection = -avoid.normalized;
            _currentDirection = GetClosestCardinalDirection(_currentDirection);
        }
    }
    
    private Vector2 GetClosestCardinalDirection(Vector2 direction)
    {
        Vector2 closest = _cardinalDirections[0];
        float maxDot = Vector2.Dot(direction.normalized, closest);
        
        for (int i = 1; i < _cardinalDirections.Length; i++)
        {
            float dot = Vector2.Dot(direction.normalized, _cardinalDirections[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = _cardinalDirections[i];
            }
        }
        
        return closest;
    }
}