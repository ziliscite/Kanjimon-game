using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
  [Header ("Movement Settings")]
  [SerializeField] private float moveSpeed = 5f;
  private Rigidbody2D rb;
  [SerializeField] private Vector2 movement;
  private Animator animator;
  public Transform coord;
  public float charX;
  public float charY;

  [Header ("Attack Box")]
  [SerializeField] private GameObject attackBox;
  [SerializeField] private float boxOffset;

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
  }

  void Update()
  {
    rb.linearVelocity = movement * moveSpeed;
    coord.position = rb.position;
    charX = coord.position.x;
    charY = coord.position.y;

    Vector2 dir = new Vector2(animator.GetFloat("lastX"), animator.GetFloat("lastY"));
    if (dir != Vector2.zero)
      attackBox.transform.localPosition = dir.normalized * boxOffset;
  }

  public void Move(InputAction.CallbackContext context)
  {
    movement = context.ReadValue<Vector2>();
    
    if (movement != Vector2.zero)
    {
        animator.SetFloat("lastX", movement.x);
        animator.SetFloat("lastY", movement.y);
    }

    animator.SetFloat("X", movement.x);
    animator.SetFloat("Y", movement.y);

    animator.SetBool("isMove", !context.canceled);
    if (context.performed)
        SoundManager.Instance.PlaySFXLoop("WalkSound");

    if (context.canceled)
        SoundManager.Instance.StopSFXLoop();
  }
}
