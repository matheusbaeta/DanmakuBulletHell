using System.Collections;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class Boss1Enemy : BaseEnemy
{
    public int totalLife = 40;

    [Header("Pattern 1 Settings")]
    public int pattern1_numberOfBullets = 20;
    public int pattern1_radius = 5;
    public int pattern1_startAngleDeg = 180;
    public int pattern1_endAngleDeg = 360;

    [Header("Pattern 3 Settings")]
    public int pattern3_waves = 5;
    public int pattern3_bulletsPerWave = 24;
    public float pattern3_baseSpeed = 3f;
    public float pattern3_speedStep = 1.2f;
    public float pattern3_waveDelay = 0.5f;
    public float pattern3_chargeTime = 0.8f;

    public override void Initialize(BaseEnemyData data)
    {
        if (data is Boss1Data boss1Data)
        {
            transform.position = boss1Data.initialPosition;
            StartCoroutine(Run(boss1Data));
        }
    }

    public IEnumerator Run(Boss1Data data)
    {
        foreach(Boss1DataStep step in data.Steps)
        {
            while(Vector2.Distance(transform.position, step.endPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, step.endPosition, data.movingSpeed * Time.deltaTime);
                yield return null;
            }

            RunPattern(step.pattern, data);

            yield return new WaitForSeconds(step.waitTime);
        }
        Destroy(gameObject);
    }

    private void RunPattern(Boss1Patterns pattern, Boss1Data data)
    {
        switch (pattern)
        {
            case Boss1Patterns.Pattern1:
                RunPattern1(data);
                break;
            case Boss1Patterns.Pattern2:
                StartCoroutine(RunPattern2(data));
                break;
            case Boss1Patterns.Pattern3:
                StartCoroutine(RunPattern3(data));
                break;
        }
    }

    private void RunPattern1(Boss1Data data)
    {
        for (int i = 0; i < pattern1_numberOfBullets; i++)
        {
            float t = i / (float)(pattern1_numberOfBullets - 1);
            float angle = Mathf.Lerp(pattern1_startAngleDeg, pattern1_endAngleDeg, t) * Mathf.Deg2Rad;

            float x = transform.position.x + Mathf.Cos(angle) * pattern1_radius;
            float y = transform.position.y + Mathf.Sin(angle) * pattern1_radius;

            ShootToPlayer(new Vector3(x, y, 0), data);
        }
    }

    private IEnumerator RunPattern2(Boss1Data data)
    {
        for (int bCount = 1; bCount <= 15; bCount += 2)
        {
            for (int i = 0; i < bCount; i++)
            {
                var direction = Vector2.down;
                var angle = Mathf.Ceil(i / 2f);
                var signal = i % 2 == 0 ? 1 : -1;
                direction.x = (angle * 0.1f) * signal;
                BulletSystem.Instance.SpawnBullet(transform.position, direction.normalized * data.shootSpeed, data.bulletSprite, data.bulletRadius);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator RunPattern3(Boss1Data data)
    {
        // Charge time
        yield return new WaitForSeconds(pattern3_chargeTime);

        Vector3 center = transform.position;

        for (int wave = 0; wave < pattern3_waves; wave++)
        {
            float speed = pattern3_baseSpeed + wave * pattern3_speedStep;

            for (int i = 0; i < pattern3_bulletsPerWave; i++)
            {
                float angle = (360f / pattern3_bulletsPerWave) * i * Mathf.Deg2Rad;

                Vector3 direction = new Vector3(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle),
                    0f
                );

                BulletSystem.Instance.SpawnBullet(
                    center,
                    direction * speed,
                    data.bulletSprite,
                    data.bulletRadius
                );
            }
            // yield return new WaitForSeconds(pattern3_waveDelay);
        }
        yield return new WaitForSeconds(0.4f);
    }

    public void ShootToPlayer(Vector3 startPosition, Boss1Data data)
    {
        Vector3 direction = (BulletSystem.Instance.player.transform.position - startPosition).normalized;
        BulletSystem.Instance.SpawnBullet(startPosition, direction * data.shootSpeed, data.bulletSprite, data.bulletRadius);
    }

    public override void TakeDamage(int damage)
    {
        ScoreManager.Instance.AddDamage(damage);

        totalLife -= damage;
        if (totalLife <= 0)
        {
            ScoreManager.Instance.AddEnemyKill();
            Destroy(gameObject);
        }
    }
}
