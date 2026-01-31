using System;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const string FileName = "score_history.json";

    [Header("Score weights (ajustáveis)")]
    public int damageWeight = 10;
    public int enemyKillWeight = 200;
    public int timePenaltyPerSecond = 5;
    public int hitPenalty = 50;

    private ScoreHistory history = new ScoreHistory();

    // Runtime metrics
    private string currentLevel;
    private float startTime;
    private int damageDealt;
    private int hitsTaken;
    private int enemiesKilled;
    private int currentScore;

    public event Action<int> OnScoreUpdated;
    public event Action<ScoreEntry> OnRunFinished;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadHistory();
    }

    public void StartRun(string levelName)
    {
        currentLevel = levelName;
        startTime = Time.time;
        damageDealt = 0;
        hitsTaken = 0;
        enemiesKilled = 0;
        currentScore = 0;
        OnScoreUpdated?.Invoke(currentScore);
    }

    public void AddDamage(int amount)
    {
        if (amount <= 0) return;
        damageDealt += amount;
        RecalculateScoreRealtime();
    }

    public void RegisterHit()
    {
        hitsTaken++;
        RecalculateScoreRealtime();
    }

    public void AddEnemyKill()
    {
        enemiesKilled++;
        RecalculateScoreRealtime();
    }

    private void RecalculateScoreRealtime()
    {
        float elapsed = Time.time - startTime;
        int score = Mathf.Max(0,
            damageDealt * damageWeight +
            enemiesKilled * enemyKillWeight -
            Mathf.FloorToInt(elapsed) * timePenaltyPerSecond -
            hitsTaken * hitPenalty
        );

        currentScore = score;
        OnScoreUpdated?.Invoke(currentScore);
    }

    public void EndRun()
    {
        float timeTaken = Time.time - startTime;

        int finalScore = Mathf.Max(0,
            damageDealt * damageWeight +
            enemiesKilled * enemyKillWeight -
            Mathf.FloorToInt(timeTaken) * timePenaltyPerSecond -
            hitsTaken * hitPenalty
        );

        var entry = new ScoreEntry
        {
            levelName = currentLevel,
            timestamp = DateTime.UtcNow.ToString("o"),
            totalScore = finalScore,
            damageDealt = damageDealt,
            timeTaken = timeTaken,
            hitsTaken = hitsTaken,
            enemiesKilled = enemiesKilled
        };

        history.entries.Add(entry);
        SaveHistory();

        OnRunFinished?.Invoke(entry);
    }

    public ScoreHistory GetHistory() => history;

    private void SaveHistory()
    {
        try
        {
            string json = JsonUtility.ToJson(history, true);
            string path = Path.Combine(Application.persistentDataPath, FileName);
            File.WriteAllText(path, json);
#if UNITY_EDITOR
            Debug.Log($"Score history saved: {path}");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save score history: {ex}");
        }
    }

    private void LoadHistory()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            var loaded = JsonUtility.FromJson<ScoreHistory>(json);
            if (loaded != null) history = loaded;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load score history: {ex}");
            history = new ScoreHistory();
        }
    }
}