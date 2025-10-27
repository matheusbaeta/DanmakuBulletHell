using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public BulletSystem system;
    public float fireInterval = 0.05f;
    public int ringCount = 20;
    private float time;

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            for (int i = 0; i < ringCount; i++)
            {
                float angle = i * Mathf.PI * 2f / ringCount;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                system.SpawnBullet(transform.position, dir);
            }
            time = fireInterval;
        }
    }
}
