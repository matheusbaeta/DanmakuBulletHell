using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelData data;

    private void Awake()
    {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        foreach (var enemyData in data.enemyData)
        {
            yield return new WaitForSeconds(enemyData.time);

            var enemy = Instantiate(enemyData.enemy);
            enemy.Initialize(enemyData.data);
        }
    }
}
