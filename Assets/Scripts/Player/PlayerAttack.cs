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
            SoundManager.Instance.PlaySFXRandomPitch("SlashSound");

            var enemyData = enemyOnSight.GetComponent<EnemyData>().enemyDataSO;
            var enemyInstanceIndex = -1;
            if (int.TryParse(enemyOnSight.name, out int number))
            {
                enemyInstanceIndex = number;
            }
            
            PlayerManager.Instance.SetEnemyData(enemyData.enemyID, enemyInstanceIndex, enemyData.isBoss);
            PlayerManager.Instance.lastPosition = transform.position;
            
            Walker walkerScript = FindFirstObjectByType<Walker>();
            if (walkerScript != null)
            {
                walkerScript.SaveCurrentLevelState();
            }
        
            PlayerManager.Instance.isReturningFromBattle = true;
            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("Battle Scene");
        yield return new WaitForSeconds(1f);
    }
}
