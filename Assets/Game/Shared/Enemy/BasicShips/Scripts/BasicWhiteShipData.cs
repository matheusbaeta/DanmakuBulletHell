using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicWhiteShip", menuName = "EnemyData/BasicWhiteShipData")]
public class BasicWhiteShipData : BaseEnemyData
{
    public List<BasicWhiteShipDataStep> Steps;
}

[Serializable]
public class BasicWhiteShipDataStep
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
