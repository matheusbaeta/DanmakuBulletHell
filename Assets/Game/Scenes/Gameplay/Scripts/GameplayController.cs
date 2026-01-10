using NUnit.Framework.Constraints;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    private static string sceneName = "Gameplay";
    private static GameplayData data;

    public GameplayData debugData;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI scoreText;

    public GameObject spaceBackground;
    public GameObject riverBackground;
    public GameObject orbitBackground;

    public static void Show(GameplayData gameplayData)
    {
        data = gameplayData;
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        if (data == null) data = debugData;

        spaceBackground.SetActive(false);
        riverBackground.SetActive(false);
        orbitBackground.SetActive(false);

        switch (data.levelData.background)
        {
            case BackgroundEnum.Space:
                spaceBackground.SetActive(true);
                break;
            case BackgroundEnum.River:
                riverBackground.SetActive(true);
                break;
            case BackgroundEnum.Orbit:
                orbitBackground.SetActive(true);
                break;
        }

        StartCoroutine(RunLevel());
        StartCoroutine(ShowLevelName(data.levelData.levelName));

        ScoreManager.Instance.StartRun(data.levelData.levelName);

        AudioController.Instance.PlayClip(data.levelData.levelSoundtrack);
    }

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreUpdated += OnScoreUpdated;
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreUpdated -= OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        scoreText.text = score.ToString();
    }

    private IEnumerator RunLevel()
    {
        foreach(EnemyData enemyData in data.levelData.enemyData)
        {
            yield return new WaitForSeconds(enemyData.time);

            BaseEnemy enemy = Instantiate(enemyData.enemy);
            enemy.Initialize(enemyData.data);
        }

        yield return ShowLevelName("Level Finished");

        ScoreManager.Instance.EndRun();

        MainMenuController.Show();
    }

    private IEnumerator ShowLevelName(string levelName)
    {
        yield return new WaitForSeconds(0.5f);

        levelNameText.text = "";
        foreach (char c in levelName)
        {
            levelNameText.text += c;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = levelName.Length; i >= 0; i--) 
        {
            levelNameText.text = levelName.Substring(0,i);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
