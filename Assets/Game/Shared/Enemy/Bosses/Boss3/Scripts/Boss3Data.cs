using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss3", menuName = "EnemyData/Boss3")]
public class Boss3Data : BaseEnemyData
{
    public List<Boss3DataStep> Steps;
}

public enum Boss3Patterns
{
    Pattern1,
    Pattern2,
    Pattern3
}

[Serializable]
public class Boss3DataStep
{
    [Header("Movement")]
    public Vector2 endPosition;

    [Header("Pattern")]
    public Boss3Patterns pattern;

    [Header("Wait")]
    public float waitTime;
}

