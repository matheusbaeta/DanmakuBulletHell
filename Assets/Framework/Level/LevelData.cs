using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public float time;
    public EnemyBase enemy;
    public BaseEnemyData data;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public List<EnemyData> enemyData;
}