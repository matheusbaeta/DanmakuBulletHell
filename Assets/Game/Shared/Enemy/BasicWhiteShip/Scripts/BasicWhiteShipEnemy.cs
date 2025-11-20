using System.Collections;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class BasicWhiteShipEnemy : BaseEnemy
{

    public override void Initialize(BaseEnemyData data)
    {
        if (data is BasicWhiteShipData basicWhiteShipData)
        {
            transform.position = basicWhiteShipData.initialPosition;
            StartCoroutine(Run(basicWhiteShipData));
        }
    }

    public IEnumerator Run(BasicWhiteShipData data)
    {
        foreach(BasicWhiteShipDataStep step in data.Steps)
        {
            float movingTimer = data.fireRate;
            while(Vector2.Distance(transform.position, step.endPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, step.endPosition, data.movingSpeed = Time.deltaTime);
                movingTimer -= Time.deltaTime;

                if(movingTimer <0f)
                {
                    ShootToPlayer(data);
                    movingTimer = data.fireRate;
                }
                yield return null;
            }

            for (int i = 0; i < step.shoots; i++)
            {
                ShootToPlayer(data);
                yield return new WaitForSeconds(data.fireRate);
            }
            yield return new WaitForSeconds(step.waitTime);
        }
        Destroy(gameObject);
    }
    
    public void ShootToPlayer(BasicWhiteShipData data)
    {
        Vector3 direction = (BulletSystem.Instance.player.transform.position - transform.position).normalized;
        BulletSystem.Instance.SpawnBullet(transform.position, direction * data.shootSpeed, data.bulletSprite, data.bulletRadius);
    }
    
}
