using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fairy1Data", menuName = "EnemyData/Fairy1Data")]
public class Fairy1Data : BaseEnemyData
{
    public Vector2 initialPosition;
    public float fireRate;
    public float shootSpeed;
    public float movingSpeed;

    public List<Fairy1DataStep> steps;
}

[Serializable]
public class Fairy1DataStep
{
    [Header("Movement")]
    public Vector2 endPosition;

    [Header("Movement Fire")]
    public bool fireWhileMoving;

    [Header("Standing Fire")]
    public int shoots;

    [Header("Wait")]
    public float waitTime;
}
