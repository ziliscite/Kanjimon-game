using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private LargeLanguageService llm;
    [SerializeField] private TMP_Text questionText; 
    [SerializeField] private TMP_InputField answerField;

    [SerializeField] private float enemyHP;
    [SerializeField] private float playerHP;

    private string currentJapanese;
    

    void Start()
    {
        enemyHP = GameManager.instance.currentEnemy.hp;
        playerHP = 100; // eksampel
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        // sementara contoh random kata manual
        string[] words = { "水", "猫", "友達", "学校" };
        currentJapanese = words[Random.Range(0, words.Length)];
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

        if (resp.score >= 80f)
        {
            enemyHP -= resp.score;
        }
        else
        {
            playerHP -= (100f - resp.score);
        }

        // clear input & next word
        answerField.text = "";
        GenerateQuestion();
    }

    private void OnEvaluateError(string message)
    {
        Debug.LogError(message);
    }
}
