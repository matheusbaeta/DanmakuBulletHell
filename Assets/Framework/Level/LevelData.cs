using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public AudioClip levelSoundtrack;
    public List<EnemyData> enemyData;
}
