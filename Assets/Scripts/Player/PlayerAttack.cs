using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private EnemyCheckerBox enemyCheckerBox;
    public GameObject enemyOnSight; 
    public int targetEnemyID;
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        // alergi kurung kurawal kah bos
        if (context.performed) AttackAction();
    }

    private void AttackAction()
    {
        if (enemyCheckerBox != null && enemyCheckerBox.enemyInsideRange)
        {
            var enemyData = enemyOnSight.GetComponent<EnemyData>().enemyDataSO;
            PlayerManager.instance.enemyBattledID = enemyData.enemyID;

            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("Battle Scene");
        yield return new WaitForSeconds(1f);
    }
}
