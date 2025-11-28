using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SubmitQuestionRequest 
{
    public string question;
    public string answer;
}

[Serializable]
public class LargeLanguageMessage 
{
    public string role;
    public string content;
}

// singleton
[Serializable]
public class ResponseFormat
{
    public string type = "json_object";
    private ResponseFormat() {}
    private static ResponseFormat _instance;
    public static ResponseFormat Instance { get
        {
            return _instance ??= new ResponseFormat();
        }
    }
}

[Serializable]
public class DeepSeekRequest
{
    public string model = "deepseek-chat";
    public List<LargeLanguageMessage> messages = new List<LargeLanguageMessage>();
    [JsonProperty("response_format")] public ResponseFormat responseFormat;
}

[Serializable]
public class LargeLanguageResponse
{
    public float score;
    [JsonProperty("correct_answer")] public string correctAnswer;
    public string explanation;
}

[Serializable]
public class WrapperChoice {
    public int index;
    public LargeLanguageMessage message;
}

[Serializable]
public class DeepSeekResponse {
    public string id;
    [JsonProperty("object")] public string obj;
    public List<WrapperChoice> choices;
}

public class LargeLanguageService : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://api.deepseek.com/chat/completions";
    [SerializeField] private RestClient client;

    [SerializeField, TextArea(3,6)] private string systemPrompt = "You are an automated evaluator for Japanese → Indonesian translations of short sentences. ALWAYS respond with a single valid JSON object and NOTHING else. The JSON object must have exactly these fields: \n- \"score\": number (0.0 to 100.0), \n- \"correct_answer\": string (the best natural Indonesian translation), \n- \"explanation\": string (short explanation of errors/partial credit; 15-70 chars in Indonesian).\n\nEvaluation rules:\n1) Score reflects semantic and grammatical correctness. 100 = perfect, 0 = entirely wrong or unrelated.\n2) Allow natural variations and synonyms. If meaning is preserved with colloquial phrasing, give high score (90–100).\n3) Penalize for missing crucial information, wrong case/tense, wrong subject, or meaning reversal.\n4) Partial credit: if some components are correct (e.g., verb correct but object wrong), score proportionally (e.g., 60–80).\n5) Prefer concise, natural Indonesian for \"correct_answer\". Do not include explanatory notes or alternatives inside \"correct_answer\".\n6) Use \"explanation\" to list main mistake(s) and why the score was reduced (one-sentence, objective).\n7) Do NOT output any surrounding markdown, commentary, or extra fields. Output must be parseable JSON.\n\nScoring guideline (for consistency):\n- 95–100: correct, natural, and fluent (minor punctuation or vocabulary variants ok).\n- 80–94: mostly correct, minor omissions/wording differences.\n- 60–79: partial correctness; important parts present but errors affecting meaning.\n- 30–59: significant errors or omissions, partial relevance.\n- 1–29: mostly incorrect but some related fragments.\n- 0: unrelated or nonsense.\n\nInput format expected in user message: JSON string with fields:\n{ \"question\": \"<japanese sentence>\", \"submitted_translation\": \"<indonesian text>\" }\n\nWhen you receive the user message, parse that JSON and evaluate. Output example (JSON):\n{\"score\": 95.0, \"correct_answer\":\"Ini adalah sebuah pulpen.\", \"explanation\":\"Varian frasa minor dengan makna yang tepat.\"}\n";
    
    // nanti kirim callback disini buat nampilin hasil ke game objek
    public void SubmitQuestion(SubmitQuestionRequest question, Action<LargeLanguageResponse> onSuccess,
        Action<string> onError)
    {
        // kl dijadiin json lama gk ya?
        string userContentJson = JsonConvert.SerializeObject(new
        {
            question.question,
            submitted_translation = question.answer
        });

        var request = new DeepSeekRequest
        {
            model = "deepseek-chat",
            messages = new List<LargeLanguageMessage>
            {
                new() { role = "system", content = systemPrompt },
                new() { role = "user", content = userContentJson }
            },
            responseFormat = ResponseFormat.Instance
        };

        Debug.Log("Sending : " + question.question + "|" + question.answer);

        StartCoroutine(client.PostJson<DeepSeekRequest, DeepSeekResponse>(
            baseUrl,
            request,
            "sk-bb76a44a33574f0dbeabaf557ec56248", // TOTO: Nanti masukin env
            onSuccess: response =>
            {
                var resp = JsonConvert.DeserializeObject<LargeLanguageResponse>(response.choices[0].message.content);
                onSuccess.Invoke(resp);
            },
            onError: onError
        ));
    }
}
