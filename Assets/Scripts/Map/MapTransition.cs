using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
  [SerializeField] PolygonCollider2D mapBoundary;
  [SerializeField] Direction transitionDirection;
  [SerializeField] float transitionDistance = 2f;
  CinemachineConfiner2D confiner;

  enum Direction
  {
    Up,
    Down,
    Left,
    Right
  }

  private void Awake()
  {
    confiner = FindAnyObjectByType<CinemachineConfiner2D>();
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      confiner.BoundingShape2D = mapBoundary;
      UpdatePlayerPosition(collision.gameObject);
    }
  }

  private void UpdatePlayerPosition(GameObject player)
  {
    Vector3 newPosition = player.transform.position;

    switch (transitionDirection)
    {
      case Direction.Up:
        newPosition.y += transitionDistance;
        break;
      case Direction.Down:
        newPosition.y -= transitionDistance;
        break;
      case Direction.Left:
        newPosition.x -= transitionDistance;
        break;
      case Direction.Right:
        newPosition.x += transitionDistance;
        break;
    }
    player.transform.position = newPosition;
  }
}
