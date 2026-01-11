using System.Collections;
using UnityEngine;

public class Boss3Enemy : BaseEnemy
{
    public int totalLife = 80;

    [Header("Pattern 1 – Spiral Hell")]
    public int pattern1_bulletsPerWave = 6;
    public float pattern1_waveDelay = 0.08f;
    public float pattern1_rotationSpeed = 18f;
    public int pattern1_totalWaves = 60;

    [Header("Pattern 2 – Rotating Ring Cage")]
    public int pattern2_ringCount = 4;
    public int pattern2_bulletsPerRing = 10;
    public float pattern2_rotationSpeed = 20f;
    public float pattern2_duration = 2.6f;
    public float pattern2_spawnInterval = 0.15f;
    public float pattern2_spacingMultiplier = 0.5f;
    public float pattern2_baseRadius = 0.4f;
    public float pattern2_ringSpacing = 0.4f;

    [Header("Pattern 3 – Delayed Explosion")]
    public int pattern3_shots = 9;
    public int pattern3_fragments = 12;
    public float pattern3_delay = 1.4f;
    public float pattern3_fragmentSpeed = 4f;

    public override void Initialize(BaseEnemyData data)
    {
        if (data is Boss3Data boss3Data)
        {
            transform.position = boss3Data.initialPosition;
            StartCoroutine(Run(boss3Data));
        }
    }

    private IEnumerator Run(Boss3Data data)
    {
        foreach (Boss3DataStep step in data.Steps)
        {
            while (Vector2.Distance(transform.position, step.endPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    step.endPosition,
                    data.movingSpeed * Time.deltaTime
                );
                yield return null;
            }

            RunPattern(step.pattern, data);
            yield return new WaitForSeconds(step.waitTime);
        }

        Destroy(gameObject);
    }

    private void RunPattern(Boss3Patterns pattern, Boss3Data data)
    {
        switch (pattern)
        {
            case Boss3Patterns.Pattern1:
                StartCoroutine(RunPattern1(data));
                break;
            case Boss3Patterns.Pattern2:
                StartCoroutine(RunPattern2(data));
                break;
            case Boss3Patterns.Pattern3:
                StartCoroutine(RunPattern3(data));
                break;
        }
    }

    private IEnumerator RunPattern1(Boss3Data data)
    {
        float angle = 0f;

        for (int wave = 0; wave < pattern1_totalWaves; wave++)
        {
            for (int i = 0; i < pattern1_bulletsPerWave; i++)
            {
                float currentAngle = angle + (360f / pattern1_bulletsPerWave) * i;
                Vector3 direction = new Vector3(
                    Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                    Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                    0f
                );

                BulletSystem.Instance.SpawnBullet(
                    transform.position,
                    direction * data.shootSpeed,
                    data.bulletSprite,
                    data.bulletRadius
                );
            }

            angle += pattern1_rotationSpeed;
            yield return new WaitForSeconds(pattern1_waveDelay);
        }
    }

    private IEnumerator RunPattern2(Boss3Data data)
    {
        float timer = 0f;
        float angleOffset = 0f;

        while (timer < pattern2_duration)
        {
            for (int ring = 1; ring <= pattern2_ringCount; ring++)
            {
                float radius = pattern2_baseRadius + (ring - 1) * pattern2_ringSpacing;

                for (int i = 0; i < pattern2_bulletsPerRing; i++)
                {
                    float angle = angleOffset + (360f / pattern2_bulletsPerRing) * i;

                    Vector3 pos = transform.position + new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad),
                        0f
                    ) * radius;

                    Vector3 dir = (pos - transform.position).normalized;

                    BulletSystem.Instance.SpawnBullet(
                        pos,
                        dir * (data.shootSpeed * 0.5f),
                        data.bulletSprite,
                        data.bulletRadius
                    );
                }
            }

            angleOffset += pattern2_rotationSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator RunPattern3(Boss3Data data)
    {
        for (int i = 0; i < pattern3_shots; i++)
        {
            Vector3 dir = (BulletSystem.Instance.player.transform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + dir * 1.5f;

            StartCoroutine(DelayedExplosion(spawnPos, data));

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator DelayedExplosion(Vector3 position, Boss3Data data)
    {
        yield return new WaitForSeconds(pattern3_delay);

        for (int i = 0; i < pattern3_fragments; i++)
        {
            float angle = (360f / pattern3_fragments) * i * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

            BulletSystem.Instance.SpawnBullet(
                position,
                dir * pattern3_fragmentSpeed,
                data.bulletSprite,
                data.bulletRadius
            );
        }
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