using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Fairy1Enemy : BaseEnemy
{
    public override void Initialize(BaseEnemyData data)
    {
        if (data is Fairy1Data fairy1Data)
        {
            transform.position = fairy1Data.initialPosition;
            StartCoroutine(Run(fairy1Data));
        }
    }

    private IEnumerator Run(Fairy1Data data)
    {
        foreach (var step in data.steps)
        {
            float movingTimer = data.fireRate;
            while (Vector2.Distance(transform.position, step.endPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    step.endPosition,
                    data.movingSpeed * Time.deltaTime
                );
                movingTimer -= Time.deltaTime;
                if (movingTimer < 0f)
                {
                    ShootToPlayer(data);
                    movingTimer = data.fireRate;
                }
                yield return null;
            }

            for (var i = 0; i < step.shoots; i++)
            {
                ShootToPlayer(data);
                yield return new WaitForSeconds(data.fireRate);
            }

            yield return new WaitForSeconds(step.waitTime);
        }

        Destroy(gameObject);
    }

    private void ShootToPlayer(Fairy1Data data)
    {
        Vector3 direction = (BulletSystem.Instance.player.transform.position - transform.position).normalized;

        BulletSystem.Instance.SpawnBullet(transform.position, direction * data.shootSpeed, data.bulletSprite, data.bulletRadius);
    }
}
