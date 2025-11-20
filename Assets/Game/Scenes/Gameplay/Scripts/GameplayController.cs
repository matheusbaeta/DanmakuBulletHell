using NUnit.Framework.Constraints;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    private static string sceneName = "Gameplay";
    private static GameplayData data;

    public GameplayData debugData;

    public static void Show(GameplayData gameplayData)
    {
        data = gameplayData;
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        if (data == null) data = debugData;
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        foreach(EnemyData enemyData in data.levelData.enemyData)
        {
            yield return new WaitForSeconds(enemyData.time);

            BaseEnemy enemy = Instantiate(enemyData.enemy);
            enemy.Initialize(enemyData.data);
        }
    }

}
