using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    // [SerializeField] private Walker walker; -> nanti bisa pake class ke3 buat track level, kl sementara mau pake walker, bebas
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private TMP_Text questionText; 
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private BattleManager battleManager;

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
        var request = new SubmitQuestionRequest
        {
            question = currentJapanese,
            answer = answerField.text
        };

        llm.SubmitQuestion(request, OnEvaluateSuccess, OnEvaluateError);
    }

    private void OnEvaluateSuccess(LargeLanguageResponse resp)
    {
        Debug.Log("Score: " + resp.score);

        // clear input & next word
        answerField.text = "";

        // KASIH SCORE KE BATTLE MANAGER
        battleManager.OnActionEvaluated(pendingAction, resp.score);

        // Reset
        pendingAction = null;
    }

    private void OnEvaluateError(string message)
    {
        Debug.LogError(message);
    }
}
