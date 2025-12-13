using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss1", menuName = "EnemyData/Boss1")]
public class Boss1Data : BaseEnemyData
{
    public List<Boss1DataStep> Steps;
}

public enum Boss1Patterns
{
    Pattern1,
    Pattern2,
    Pattern3
}

[Serializable]
public class Boss1DataStep
{
    [Header("Movement")]
    public Vector2 endPosition;

    [Header("Pattern")]
    public Boss1Patterns pattern;

    [Header("Wait")]
    public float waitTime;
}
