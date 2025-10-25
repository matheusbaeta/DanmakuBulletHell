using UnityEngine;

public class SmallShipEnemy : EnemyBase
{
    public override void Initialize(BaseEnemyData data)
    {
        if (data is SmallShipData smallShipData)
        {
            transform.position = smallShipData.initialPosition;
        }
    }
}
