using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    // [SerializeField] private Walker walker; -> nanti bisa pake class ke3 buat track level, kl sementara mau pake walker, bebas
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private TMP_Text questionText; 
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private GameObject boxExplain;
    [SerializeField] private Button submitButton;


    private bool isEvaluating = false;
    private string pendingAction; // "attack" / "defend"

    private string currentJapanese;
    
    void Start()
    {
        // enemyHP = GameManager.instance.currentEnemy.hp;
        // playerHP = 100; // eksampel
        GenerateQuestion();
    }
    
    // weighted scaling, biar di awal gk digenjreng soal N kelaz atas
    public string GetDungeonDifficulty(int dungeon)
    {
        float r = Random.value;

        if (dungeon <= 2) return "N5";
        if (dungeon <= 5) return (r < 0.7f) ? "N5" : "N4";
        if (dungeon <= 6) return (r < 0.5f) ? "N4" : "N3";
        if (dungeon <= 7) return (r < 0.3f) ? "N4" : "N3";
        if (dungeon <= 8) return (r < 0.6f) ? "N3" : "N2";
        if (dungeon <= 9) return (r < 0.4f) ? "N2" : "N1";

        return "N1"; // Dungeon 10 final boss
    }
    
    public void AskQuestionForAction(string action)
    {
        pendingAction = action;
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        // ini nanti kategori sesuai enemy type, dimap tuh ke question type
        currentJapanese = QuestionRepository.instance.GetRandomQuestion("", GetDungeonDifficulty(1)).sentence;
        questionText.text = currentJapanese;
    }

    public void OnSubmit()
    {
        if (isEvaluating) return;
        isEvaluating = true;

        answerField.interactable = false;
        submitButton.interactable = false;

        var request = new SubmitQuestionRequest
        {
            question = currentJapanese,
            answer = answerField.text
        };

        answerField.text = "";

        llm.SubmitQuestion(request, OnEvaluateSuccess, OnEvaluateError);
    }

    private void OnEvaluateSuccess(LargeLanguageResponse resp)
    {
        isEvaluating = false;

        resultText.text = $"Score: {resp.score:0}";
        
        boxExplain.SetActive(true);
        explanationText.text = resp.explanation;

        // input OFF saat review
        answerField.interactable = false;

        battleManager.OnActionEvaluated(pendingAction, resp.score);

        submitButton.GetComponentInChildren<TMP_Text>().text = "Lanjut";
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(NextQuestion);

        pendingAction = null;

        // auto-hide boxExplain setelah 3detik
        StartCoroutine(HideExplanationAfterSeconds(3f));
    }

    private void OnEvaluateError(string message)
    {
        isEvaluating = false;

        explanationText.text = message;
        Debug.LogError(message);

        ResetReviewUI();

        // balikin turn ke player
        battleManager.CancelActionAndReturnToPlayer();

        pendingAction = null;
    }

    // lanjut setelah aksi
    private void NextQuestion()
    {
        boxExplain.SetActive(false);

        submitButton.GetComponentInChildren<TMP_Text>().text = "Submit";
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmit);
        submitButton.interactable = true;

        answerField.interactable = true;

        GenerateQuestion();
    }

    // buat ngosongin UI
    public void ResetReviewUI()
    {
        Debug.Log("Resetting Review UI");
        resultText.text = "";
        explanationText.text = "";

        boxExplain.SetActive(false);

        answerField.text = "";
        answerField.interactable = true;

        submitButton.GetComponentInChildren<TMP_Text>().text = "Submit";
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmit);
        submitButton.interactable = true;
    }

    private IEnumerator HideExplanationAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        boxExplain.SetActive(false);
    }
}
