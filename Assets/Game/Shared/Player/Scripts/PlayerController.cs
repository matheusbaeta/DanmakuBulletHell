using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Bullets")]
    public float bulletFrequency = 0.2f;
    public BulletController bulletPrefab;
    public Queue<BulletController> unusedBullets = new();

    [Header("Player Settings")]
    public float speed;
    public BulletSystem bulletSystem;
    public float collisionDistance;
    public SpriteRenderer spriteRenderer;
    public PlayerHealth playerHealth;

    private void Awake()
    {
        Instance = this;
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        StartCoroutine(ShootCoroutine());
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 movement = inputValue.Get<Vector2>();
        transform.position += (Vector3)movement * speed;
    }

    private void Update()
    {
        spriteRenderer.color = bulletSystem.playerHit ? Color.red : Color.white;
        
        if(bulletSystem.playerHit && !playerHealth.IsDead())
        {
            playerHealth.TakeHit();
            transform.position = new Vector3(0, -4, 0);
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (unusedBullets.TryDequeue(out BulletController bullet))
            {
                bullet.gameObject.SetActive(true);
                bullet.SetData(transform.position + Vector3.up * 0.5f, Vector2.up);
            }
            else
            {
                var newBullet = Instantiate(bulletPrefab);
                newBullet.SetData(transform.position + Vector3.up * 0.5f, Vector2.up);
            }
            yield return new WaitForSeconds(bulletFrequency);
        }
    }
}
