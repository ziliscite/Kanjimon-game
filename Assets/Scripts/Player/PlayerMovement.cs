using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
  public GameObject character;
  public Transform coord;
  public float charX;
  public float charY;

  [Header ("Attack Box")]
  [SerializeField] private GameObject attackBox;
  [SerializeField] private float boxOffset;

  [Header ("Other References")]
  [SerializeField] private TriggerBattle triggerBattle;
  [SerializeField] private EnemyChecker enemyChecker;

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
  }

  void Awake()
  {
    character = this.gameObject;
    triggerBattle = this.GetComponent<TriggerBattle>();
    enemyChecker = attackBox.GetComponent<EnemyChecker>();
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
  }

  public void OnAttack(InputAction.CallbackContext context)
  {
      if (context.performed)
      {
          AttackAction();
      }
  }

  private void AttackAction()
  {
      if (enemyChecker != null && enemyChecker.enemyInsideRange)
      {
        EnemyData enemyData = enemyChecker.targetEnemy.GetComponent<EnemyData>();

        GameManager.instance.currentEnemy = enemyData.enemyDataSO;

          triggerBattle?.EnterBattle();
          gameObject.GetComponent<SpriteRenderer>().enabled = false;
          this.enabled = false;
      }
  }
}
