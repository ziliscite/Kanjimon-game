using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private QuestionManager questionManager;
    [SerializeField] private PopupUI popUpUI;
    [SerializeField] private GameObject slashVFX;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PotionGetter potionGetter;
    
    [Header("UI References")]
    [SerializeField] private GameObject UIButtons; // Attack, Defend, Potion buttons
    [SerializeField] private GameObject UIBattle;  // The typing input area
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Slider playerShieldSlider;
    [SerializeField] private Button potionButton;

    [Header("Delays")]
    [SerializeField] private float postEvaluationDelay = 2f; 
    // Tambahkan di bagian [Header("Battle Data")]
    private int currentSessionScore = 0;

    IEnumerator Start()
    {
        if (PlayerManager.Instance.potionsLeft <= 0) {
            potionButton.interactable = false;
        }

        UIButtons.SetActive(false);
        UIBattle.SetActive(false);

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
popUpUI.ShowYourTurn(() => {
    UIButtons.SetActive(true);
});

        
        // Reset skor untuk battle baru
        currentSessionScore = 0;
        BattleDataManager.Instance.ResetSessionScore();
    }

    public void PlayerAttack() { 
        if(state == BattleState.PlayerTurn) { 
            state = BattleState.WaitingLLM; 
            questionManager.ResetReviewUI(); 
            questionManager.AskQuestionForAction("attack"); 
            UIButtons.SetActive(false); 
            UIBattle.SetActive(true); 
        } 
    }

    public void PlayerDefend() { 
        if(state == BattleState.PlayerTurn) { 
            state = BattleState.WaitingLLM; 
            questionManager.ResetReviewUI(); 
            questionManager.AskQuestionForAction("defend"); 
            UIButtons.SetActive(false); 
            UIBattle.SetActive(true); 
        } 
    }

    public void PlayerUsePotion() {
        
        if (state != BattleState.PlayerTurn) return;
        if (PlayerManager.Instance.potionsLeft <= 0) return;

        playerHealth = Mathf.Clamp(playerHealth + 40, 0, 100);
        PlayerManager.Instance.potionsLeft -= 1;
        potionGetter.RefreshPotionUI();
        UpdatePlayerHPBar();
        
        UIButtons.SetActive(false);
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
    bool success = score > 10f;

    if (success)
    {
        // Akumulasi skor ketika jawaban benar
        int scoreToAdd = Mathf.RoundToInt(score);
        currentSessionScore += scoreToAdd;
        BattleDataManager.Instance.AddScore(scoreToAdd);
        
        Debug.Log($"[Battle] Correct answer! Score added: {scoreToAdd}. Session total: {currentSessionScore}");

        if (action == "attack")
        {
            enemyHealth -= scoreToAdd;
            UpdateEnemyHPBar();

            if (enemyHealth <= 0)
            {
                PlayerManager.Instance.isWinningBattle = true;
                PlayerManager.Instance.playerHP = playerHealth;

                // Simpan data battle ke JSON
                bool isBossBattle = PlayerManager.Instance.isEnemyBoss;
                BattleDataManager.Instance.SaveBattleResult(playerHealth, isBossBattle);

                if (isBossBattle && BossManager.Instance != null)
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
            playerShield += scoreToAdd;
            playerShield = Mathf.Clamp(playerShield, 0, 100);
            UpdateShieldBar();
        }
    }
    else
    {
        Debug.Log("[Battle] Incorrect answer - no score added");
    }

    StartCoroutine(DelayedEnemyTurn());
}

    private void HandleEnemyDeath()
    {
        if (enemySpawner.isEnemyBoss) BossManager.Instance.SetBossDead(true);
        PlayerManager.Instance.playerHP = playerHealth;
        state = BattleState.End;
        StartCoroutine(ChangeScene());
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

        UIBattle.SetActive(false);
        UIButtons.SetActive(false);
        questionManager.ResetReviewUI(); 
        // ------------------------

        bool animComplete = false;
        popUpUI.ShowEnemyTurn(() => { 
            animComplete = true; 
        });

        while(!animComplete) yield return null;


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

        yield return new WaitForSeconds(0.5f); // Small impact pause

        state = BattleState.PlayerTurn;

        popUpUI.ShowYourTurn(() => {
            UIButtons.SetActive(true);
        });
    }
    
    private void UpdatePlayerHPBar() { 
        if (playerHPCoroutine != null) StopCoroutine(playerHPCoroutine);
        playerHPCoroutine = StartCoroutine(AnimateSlider(playerHealthSlider, playerHealth));
    }
    
    private void UpdateEnemyHPBar() {
        if (enemyHPCoroutine != null) StopCoroutine(enemyHPCoroutine);
        enemyHPCoroutine = StartCoroutine(AnimateSlider(enemyHealthSlider, enemyHealth));
    }

    private void UpdateShieldBar() {
        if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);
        shieldCoroutine = StartCoroutine(AnimateSlider(playerShieldSlider, playerShield, 8f));
    }

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
        state = BattleState.PlayerTurn;
        UIBattle.SetActive(false);
        popUpUI.ShowYourTurn(() => { UIButtons.SetActive(true); });
    }

    private IEnumerator ChangeScene()
    {
        ScreenFader.Instance.FadeToScene("ProcgenScene");
        yield return new WaitForSeconds(1f);
    }
}