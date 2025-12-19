using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    WaitingLLM,
    End
}

public class BattleManager : MonoBehaviour
{
    [Header("Battle Data")]
    public int playerHealth;
    public int enemyHealth;
    public int playerShield;

    private Coroutine playerHPCoroutine;
    private Coroutine enemyHPCoroutine;
    private Coroutine shieldCoroutine;
    
    public BattleState state;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private QuestionManager questionManager;
    [SerializeField] private PopupUI popUpUI;
    // [SerializeField] private TMP_Text explanationText;
    // [SerializeField] private GameObject boxExplain;

    
    [Header("UI References")]
    [SerializeField] private GameObject UIButtons;
    [SerializeField] private GameObject UIBattle;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Slider playerShieldSlider;

    [Header("Delay")]
    [SerializeField] private float postEvaluationDelay = 5f;

    IEnumerator Start()
    {
        // tunggu sampe enemy beneran ada
        while (enemySpawner.SpawnedEnemy == null)
            yield return null;

        playerHealth = PlayerManager.Instance.playerHP;

        enemyHealth = enemySpawner.enemyHealthSpawned;
        enemyHealthSlider = enemySpawner.SpawnedEnemy.healthSlider;

        playerShield = 0;

        playerHealthSlider.maxValue = playerHealth;
        playerHealthSlider.value = playerHealth;

        enemyHealthSlider.maxValue = enemyHealth;
        enemyHealthSlider.value = enemyHealth;

        playerShieldSlider.maxValue = 100;
        playerShieldSlider.value = 0;

        state = BattleState.PlayerTurn;
        popUpUI.ShowYourTurn();
    }

    public void PlayerAttack()
    {
        if (state != BattleState.PlayerTurn) return;

        state = BattleState.WaitingLLM;

        questionManager.ResetReviewUI();
        questionManager.AskQuestionForAction("attack");

        UIButtons.SetActive(false);
        UIBattle.SetActive(true);
    }

    public void PlayerDefend()
    {
        if (state != BattleState.PlayerTurn) return;

        state = BattleState.WaitingLLM;

        questionManager.ResetReviewUI();
        questionManager.AskQuestionForAction("defend");
        
        UIButtons.SetActive(false);
        UIBattle.SetActive(true);
    }

    public void PlayerUsePotion()
    {
        if (state != BattleState.PlayerTurn) return;

        playerHealth += 40;
    
        questionManager.ResetReviewUI();
        UIButtons.SetActive(false);
        UIBattle.SetActive(true);

        EnemyTurn();
    }

    public void OnActionEvaluated(string action, float score)
    {
        if (state != BattleState.WaitingLLM)
        {
            Debug.Log("Ignored duplicate evaluation");
            return;
        }

        Debug.Log($"[Battle] Action: {action}, Score: {score}");
        bool success = score > 10f; // threshold bebas

        if (success)
        {
            if (action == "attack")
            {
                enemyHealth -= Mathf.RoundToInt(score);
                UpdateEnemyHPBar();

                if (enemyHealth <= 0)
                {
                    PlayerManager.Instance.isWinningBattle = true;
                    PlayerManager.Instance.playerHP = playerHealth; // update data HP player ke global
                    
                    if (PlayerManager.Instance.isEnemyBoss && BossManager.Instance != null)
                    {
                        BossManager.Instance.SetBossDead(true);
                        Debug.Log("[BattleManager] Boss defeated - setting boss dead flag");
                    }
                    
                    state = BattleState.End;
                    StartCoroutine(ChangeScene());
                    return;
                }
            }
            else if (action == "defend")
            {
                playerShield += Mathf.RoundToInt(score);
                playerShield = Mathf.Clamp(playerShield, 0, 100);
                UpdateShieldBar();
            }
        }

        StartCoroutine(DelayedEnemyTurn());
    }

    private IEnumerator DelayedEnemyTurn()
    {
        yield return new WaitForSeconds(postEvaluationDelay);
        EnemyTurn();
    }

    private void EnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        state = BattleState.EnemyTurn;
        popUpUI.ShowEnemyTurn();

        // BIAR POPUP KEBACA
        yield return new WaitForSeconds(1.2f);

        EnemyData enemy = enemySpawner.SpawnedEnemy.GetComponent<EnemyData>();
        int enemyDamage = enemy.GetAttackDamage();

        int finalDamage = Mathf.Max(0, enemyDamage - playerShield);

        playerHealth -= finalDamage;
        UpdatePlayerHPBar();

        if (playerHealth <= 0)
        {
            state = BattleState.End;
            StartCoroutine(ChangeScene());
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        state = BattleState.PlayerTurn;
        popUpUI.ShowYourTurn();

        UIBattle.SetActive(false);
        UIButtons.SetActive(true);
    }

    private void UpdatePlayerHPBar()
    {
        if (playerHPCoroutine != null)
            StopCoroutine(playerHPCoroutine);

        playerHPCoroutine = StartCoroutine(
            AnimateSlider(playerHealthSlider, playerHealth)
        );
    }

    private void UpdateEnemyHPBar()
    {
        if (enemyHPCoroutine != null)
            StopCoroutine(enemyHPCoroutine);

        enemyHPCoroutine = StartCoroutine(
            AnimateSlider(enemyHealthSlider, enemyHealth)
        );
    }

    private void UpdateShieldBar()
    {
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(
            AnimateSlider(playerShieldSlider, playerShield, 8f)
        );
    }


    // Animasi bars
    private IEnumerator AnimateSlider(Slider slider, float target, float speed = 5f)
    {
        while (Mathf.Abs(slider.value - target) > 0.1f)
        {
            slider.value = Mathf.Lerp(slider.value, target, speed * Time.deltaTime);
            yield return null;
        }

        slider.value = target;
    }

    public void CancelActionAndReturnToPlayer()
    {
        Debug.Log("Cancelling action and returning to player's turn");
        state = BattleState.PlayerTurn;

        popUpUI.ShowYourTurn();
        UIBattle.SetActive(false);
        UIButtons.SetActive(true);
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("ProcgenScene");
        yield return new WaitForSeconds(1f);
    }
}
