using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss2", menuName = "EnemyData/Boss2")]
public class Boss2Data : BaseEnemyData
{
    public List<Boss2DataStep> Steps;
}

public enum Boss2Patterns
{
    Pattern1,
    Pattern2,
    Pattern3
}

[Serializable]
public class Boss2DataStep
{
    [Header("Movement")]
    public Vector2 endPosition;

    [Header("Pattern")]
    public Boss2Patterns pattern;

    [Header("Wait")]
    public float waitTime;
}