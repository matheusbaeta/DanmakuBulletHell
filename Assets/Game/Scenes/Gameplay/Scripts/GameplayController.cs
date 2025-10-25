using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayData
{
    public LevelData levelData;
}

public class GameplayController : MonoBehaviour
{
    private static string sceneName = "Gameplay";
    private static GameplayData data;

    public static void Show(GameplayData gameplayData)
    {
        data = gameplayData;
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        foreach (var enemyData in data.levelData.enemyData)
        {
            yield return new WaitForSeconds(enemyData.time);

            var enemy = Instantiate(enemyData.enemy);
            enemy.Initialize(enemyData.data);
        }
    }
}
