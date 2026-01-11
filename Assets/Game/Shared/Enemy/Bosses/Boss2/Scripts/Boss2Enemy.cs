using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Enemy : BaseEnemy
{
    public int totalLife = 60;

    [Header("Pattern 1 – Oscillating Scythe Waves")]
    public int pattern1_waves = 8;
    public int pattern1_bulletsPerWave = 12;
    public float pattern1_waveSpread = 60f;
    public float pattern1_oscillationWidth = 2f;
    public float pattern1_waveDelay = 0.4f;

    [Header("Pattern 2 – Recursive Fractal Bloom (BUFFED)")]
    public float pattern2_duration = 5f;
    public float pattern2_spawnInterval = 0.25f;
    public int pattern2_mainShots = 10;
    public int pattern2_childBullets = 10;
    public float pattern2_delay = 0.9f;
    public float pattern2_mainSpeed = 2.2f;
    public float pattern2_childSpeed = 3.8f;

    [Header("Pattern 3 – Geometric Star Cage")]
    public int pattern3_bursts = 3;           // How many shapes to spawn
    public int pattern3_sides = 6;            // 6 = Hexagon
    public float pattern3_radius = 3f;        // Distance from boss
    public int pattern3_bulletsPerSide = 10;  // How many bullets make the line
    public float pattern3_expandSpeed = 1.5f; // How fast the shape moves out
    public float pattern3_burstDelay = 2.0f;  // Time between shapes

    public override void Initialize(BaseEnemyData data)
    {
        if (data is Boss2Data boss2Data)
        {
            transform.position = boss2Data.Steps[0].endPosition;
            StartCoroutine(Run(boss2Data));
        }
    }

    private IEnumerator Run(Boss2Data data)
    {
        foreach (Boss2DataStep step in data.Steps)
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

            yield return StartCoroutine(RunPattern(step.pattern, data));
            yield return new WaitForSeconds(step.waitTime);
        }

        Destroy(gameObject);
    }

    private IEnumerator RunPattern(Boss2Patterns pattern, Boss2Data data)
    {
        switch (pattern)
        {
            case Boss2Patterns.Pattern1:
                yield return StartCoroutine(RunPattern1(data));
                break;
            case Boss2Patterns.Pattern2:
                yield return StartCoroutine(RunPattern2(data));
                break;
            case Boss2Patterns.Pattern3:
                yield return StartCoroutine(RunPattern3(data));
                break;
        }
    }

    private IEnumerator RunPattern1(Boss2Data data)
    {
        Vector3 startPos = transform.position;
        for (int w = 0; w < pattern1_waves; w++)
        {
            // Visual oscillation: Boss shifts slightly left/right while firing
            float offsetX = Mathf.Sin(Time.time * 5f) * pattern1_oscillationWidth;
            Vector3 firePos = startPos + new Vector3(offsetX, 0, 0);

            for (int i = 0; i < pattern1_bulletsPerWave; i++)
            {
                float angle = Mathf.Lerp(-pattern1_waveSpread, pattern1_waveSpread, i / (float)(pattern1_bulletsPerWave - 1));
                // Fire downwards with an angular spread
                Quaternion rotation = Quaternion.Euler(0, 0, angle + 180f);
                Vector3 dir = rotation * Vector3.up;

                BulletSystem.Instance.SpawnBullet(firePos, dir * data.shootSpeed, data.bulletSprite, data.bulletRadius);
            }
            yield return new WaitForSeconds(pattern1_waveDelay);
        }
    }


    private IEnumerator RunPattern2(Boss2Data data)
    {
        float timer = 0f;
        while (timer < pattern2_duration)
        {
            for (int i = 0; i < pattern2_mainShots; i++)
            {
                float angle = (360f / pattern2_mainShots) * i * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                Vector3 pos = transform.position;

                BulletSystem.Instance.SpawnBullet(pos, dir * pattern2_mainSpeed, data.bulletSprite, data.bulletRadius);
                StartCoroutine(FractalExplosion(pos + dir * 0.8f, data));
            }
            timer += pattern2_spawnInterval;
            yield return new WaitForSeconds(pattern2_spawnInterval);
        }
    }

    private IEnumerator FractalExplosion(Vector3 position, Boss2Data data)
    {
        yield return new WaitForSeconds(pattern2_delay);
        for (int i = 0; i < pattern2_childBullets; i++)
        {
            float angle = (360f / pattern2_childBullets) * i * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            BulletSystem.Instance.SpawnBullet(position, dir * pattern2_childSpeed, data.bulletSprite, data.bulletRadius * 0.75f);
        }
    }

    private IEnumerator RunPattern3(Boss2Data data)
    {
        for (int b = 0; b < pattern3_bursts; b++)
        {
            Vector3[] vertices = new Vector3[pattern3_sides];
            float angleStep = 360f / pattern3_sides;
            float randomOffset = Random.Range(0f, 360f); 

            for (int i = 0; i < pattern3_sides; i++)
            {
                float angle = (randomOffset + (angleStep * i)) * Mathf.Deg2Rad;
                vertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            }


            for (int i = 0; i < pattern3_sides; i++)
            {
                Vector3 startDir = vertices[i];
                Vector3 endDir = vertices[(i + 1) % pattern3_sides];

                for (int j = 0; j < pattern3_bulletsPerSide; j++)
                {
                    float t = j / (float)pattern3_bulletsPerSide;

                    Vector3 bulletDir = Vector3.Lerp(startDir, endDir, t);

                    BulletSystem.Instance.SpawnBullet(
                        transform.position,
                        bulletDir.normalized * pattern3_expandSpeed,
                        data.bulletSprite,
                        data.bulletRadius
                    );
                }
            }

            yield return new WaitForSeconds(pattern3_burstDelay);
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