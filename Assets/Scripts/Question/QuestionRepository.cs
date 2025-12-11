using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class QuestionData {
    public string sentence;
    public string type;
    public string level;
}

public class QuestionRepository : MonoBehaviour
{
    public static QuestionRepository instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        LoadCsvFromPath(_csvPath);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private readonly string _csvPath = "Assets/Resources/Data/japanese_question.csv";
    private readonly List<QuestionData> _questions = new();

    private void LoadCsvFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("CSV file tidak ditemukan: " + path);
            return;
        }

        string[] rows = File.ReadAllLines(path);

        for (int i = 1; i < rows.Length; i++) // lewati header
        {
            string[] cols = rows[i].Split(',');

            if (cols.Length < 4) continue;

            _questions.Add(new QuestionData {
                sentence = cols[1],
                type = cols[2],
                level = cols[3]
            });
        }

        Debug.Log("CSV Loaded, total: " + _questions.Count);
    }

    public QuestionData GetRandomQuestion(string category, string level = null)
    {
        var filtered = _questions.AsQueryable();
        if (!string.IsNullOrEmpty(category))
        {
            filtered = filtered.Where(q => q.type == category);
        }
        if (!string.IsNullOrEmpty(level))
        {
            filtered = filtered.Where(q => q.level == level);
        }

        var result = filtered.ToList();
        if (result.Count == 0)
        {
            Debug.LogWarning($"Tidak ada data dengan kategori {category} dan level {level}");
            return null;
        }

        return result[Random.Range(0, result.Count)];
    }
}