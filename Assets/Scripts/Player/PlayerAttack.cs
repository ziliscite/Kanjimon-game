using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private EnemyCheckerBox enemyCheckerBox;
    [SerializeField] private GameObject slashVFX;
    public GameObject enemyOnSight; 
    public int targetEnemyID;
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        // alergi kurung kurawal kah bos || biarin wle
        if (context.performed) AttackAction();
    }

    private void AttackAction()
    {
        if (enemyCheckerBox != null && enemyCheckerBox.enemyInsideRange)
        {
            Vector3 centerPos = enemyCheckerBox.transform.position;
            Instantiate(slashVFX, centerPos, Quaternion.identity);

            var enemyData = enemyOnSight.GetComponent<EnemyData>().enemyDataSO;
            PlayerManager.Instance.enemyBattledID = enemyData.enemyID;

            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("Battle Scene");
        yield return new WaitForSeconds(1f);
    }
}
