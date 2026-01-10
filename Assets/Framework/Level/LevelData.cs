using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum BackgroundEnum
{
    Space,
    River
}

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public BackgroundEnum background;
    public AudioClip levelSoundtrack;
    public List<EnemyData> enemyData;
}
