using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField] private float moveSpeed = 5f;
  private Rigidbody2D rb;
  [SerializeField] private Vector2 movement;
  private Animator animator;
  public GameObject character;
  public Transform coord;
  public float charX;
  public float charY;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
  }

  void Awake()
  {
    character = this.gameObject;
  }
  // Update is called once per frame
  void Update()
  {
    rb.linearVelocity = movement * moveSpeed;
    coord.position = rb.position;
    charX = coord.position.x;
    charY = coord.position.y;
  }

  public void Move(InputAction.CallbackContext context)
  {
    animator.SetBool("isMove", true);

    if (context.canceled)
    {
      animator.SetBool("isMove", false);
      animator.SetFloat("lastX", movement.x);
      animator.SetFloat("lastY", movement.y);
    }
    movement = context.ReadValue<Vector2>();
    animator.SetFloat("X", movement.x);
    animator.SetFloat("Y", movement.y);
  }
}
