using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private EnemyCheckerBox enemyCheckerBox;
    [SerializeField] private PlayerData playerData;
    public GameObject enemyOnSight; 
    public int targetEnemyID;
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackAction();
    }

    private void AttackAction()
    {
        if (enemyCheckerBox != null && enemyCheckerBox.enemyInsideRange)
        {
            targetEnemyID = enemyOnSight.GetComponent<EnemyData>().enemyDataSO.enemyID;
            playerData.enemyBattledID = targetEnemyID;

            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("Battle Scene");
        yield return new WaitForSeconds(1f);
        PlayerInstance.instance.Disabler();
    }
}
