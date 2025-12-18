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

    
    public BattleState state;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private QuestionManager questionManager;
    
    [Header("UI References")]
    [SerializeField] private GameObject UIButtons;
    [SerializeField] private GameObject UIBattle;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Slider playerShieldSlider;

    [Header("Delay")]
    [SerializeField] private float postEvaluationDelay = 5f;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerData>().playerHealth;
        enemyHealth = enemySpawner.enemyHealthSpawned;
        enemyHealthSlider = FindFirstObjectByType<EnemyData>().healthSlider;

        playerShield = 0;
        playerShieldSlider.maxValue = 100;
        playerShieldSlider.value = playerShield;

        playerHealthSlider.maxValue = playerHealth;
        playerHealthSlider.value = playerHealth;

        enemyHealthSlider.maxValue = enemyHealth;
        enemyHealthSlider.value = enemyHealth;

        state = BattleState.PlayerTurn;
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

        playerHealth += 20; // sementara 10 hp, nanti bisa diganti
    
        questionManager.ResetReviewUI();
        UIButtons.SetActive(false);
        UIBattle.SetActive(true);

        EnemyTurn();
    }

    // callback kalau LLM berhasil
    // private void OnLLMSuccess(LargeLanguageResponse response)
    // {
    //     Debug.Log("LLM Score: " + response.score);
    //     Debug.Log("LLM Says: " + response.explanation);

    //     // setelah dapet jawaban → musuh nyerang
    //     EnemyTurn();
    // }

    // private void OnLLMError(string msg)
    // {
    //     Debug.LogError("LLM ERROR: " + msg);
    //     // fallback: tetap lanjut musuh
    //     EnemyTurn();
    // }

    public void OnActionEvaluated(string action, float score)
    {
        if (action == "attack")
        {
            int dmg = Mathf.RoundToInt(score);
            enemyHealth -= dmg;
            UpdateEnemyHPBar();

            if (enemyHealth <= 0)
            {
                if (enemySpawner.isEnemyBoss)
                {
                    BossManager.Instance.SetBossDead(true);
                }
                
                state = BattleState.End;
                StartCoroutine(ChangeScene());
                return;
            }
        }
        else if (action == "defend")
        {
            int shieldGain = Mathf.RoundToInt(score);
            playerShield += shieldGain;

            playerShield = Mathf.Clamp(playerShield, 0, 100);
            UpdateShieldValue();
        }

        // delay sebelum turn musuh (buat proses jawaban dulu)
        StartCoroutine(DelayedEnemyTurn());
    }

    private IEnumerator DelayedEnemyTurn()
    {
        yield return new WaitForSeconds(postEvaluationDelay);
        EnemyTurn();
    }

    private void EnemyTurn()
    {
        state = BattleState.EnemyTurn;

        int enemyDamage = 5;

        int finalDamage = Mathf.Max(0, enemyDamage - playerShield);

        playerHealth -= finalDamage;
        UpdatePlayerHPBar();

        Debug.Log("Enemy attacks! Player HP: " + playerHealth);

        if (playerHealth <= 0)
        {
            state = BattleState.End;
            Debug.Log("Player kalah.");
            StartCoroutine(ChangeScene());
            return;
        }

        state = BattleState.PlayerTurn;
        Debug.Log("Player turn again.");

        UIBattle.SetActive(false);
        UIButtons.SetActive(true);
    }

    private void UpdatePlayerHPBar()
    {
        playerHealthSlider.value = playerHealth;
        Debug.Log("Updating Player HP Bar: " + playerHealth);
    }

    private void UpdateEnemyHPBar()
    {
        enemyHealthSlider.value = enemyHealth;
        Debug.Log("Updating Enemy HP Bar: " + enemyHealth);
    }

    private void UpdateShieldValue()
    {
        playerShieldSlider.value = playerShield;
        Debug.Log("Updating Player Shield: " + playerShield);
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("Test");
        yield return new WaitForSeconds(1f);
        PlayerInstance.instance.Disabler();
    }
}
