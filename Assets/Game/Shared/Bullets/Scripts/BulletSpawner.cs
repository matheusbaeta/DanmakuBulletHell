using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public Sprite sprite;
    public float speed = 10f;
    public float fireInterval = 0.05f;
    public float ringCount = 20;
    public float bulletRadius = 0.5f;

    private float time;

    private void Update()
    {
        time -= Time.deltaTime;

        if(time <= 0f)
        {
            for(int i = 0; i < ringCount; i++)
            {
                float angle = i * Mathf.PI * 2f / ringCount;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * speed;
                BulletSystem.Instance.SpawnBullet(transform.position, dir, sprite, bulletRadius);
            }
        }
        time = fireInterval;
    }

}
