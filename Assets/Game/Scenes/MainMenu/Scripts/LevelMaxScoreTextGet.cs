using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelMaxScoreTextGet : MonoBehaviour
{
    public string levelName;

    private void Start()
    {
        var history = ScoreManager.Instance.GetHistory();
        var levelHistory = history.entries.FindAll(entry => entry.levelName == levelName);
        levelHistory.Sort((a, b) => b.totalScore.CompareTo(a.totalScore));
        GetComponent<TextMeshProUGUI>().text = $"Max Score: {(levelHistory.Count > 0 ? levelHistory[0].totalScore.ToString() : '0')}";
    }
}
