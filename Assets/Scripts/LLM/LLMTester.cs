using System;
using UnityEngine;

public class LLMTester : MonoBehaviour
{
    [SerializeField] private LargeLanguageService service;

    public void TestSend()
    {
        var req = new SubmitQuestionRequest {
            question = "私は学生です。",
            answer = "Saya murid."
        };

        service.SubmitQuestion(req, res =>
        {
            Debug.Log("Success: " + res.correctAnswer + "|" + res.score + "|" + res.explanation);
        }, err =>
        {
            Debug.LogError(err);
        });
    }

    private void Start()
    {
        TestSend();
    }
}
