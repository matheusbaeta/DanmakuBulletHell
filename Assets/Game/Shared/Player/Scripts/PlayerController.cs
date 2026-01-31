using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private Camera cam;

    [Header("Bullets")]
    public int bulletDamage = 40;
    public float bulletFrequency = 0.2f;
    public BulletController bulletPrefab;
    public Queue<BulletController> unusedBullets = new();

    [Header("Player Settings")]
    public float speed = 75;
    public BulletSystem bulletSystem;
    public float collisionDistance;
    public SpriteRenderer spriteRenderer;
    public PlayerHealth playerHealth;

    [Header("Invincibility Visuals")]
    public float blinkInterval = 0.1f;

    private Coroutine blinkCoroutine;

    private void Awake()
    {
        Instance = this;
        // cam = Camera.main;

        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        StartCoroutine(ShootCoroutine());
    }

    public void OnMove(InputValue inputValue)
    {

        if (cam == null)
            cam = Camera.main;

        Vector2 movement = inputValue.Get<Vector2>();

        Vector3 newPosition = transform.position + (Vector3)movement * speed * Time.deltaTime;

        float zDistance = transform.position.z - cam.transform.position.z;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        newPosition.x = Mathf.Clamp(newPosition.x, min.x, max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, min.y, max.y);

        transform.position = newPosition;
    }

    private void Update()
    {
        // Apply damage
        if (bulletSystem.playerHit)
        {
            playerHealth.TakeHit();
        }

        // Handle blinking
        if (playerHealth.IsInvincible && blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(Blink());
        }
        else if (!playerHealth.IsInvincible && blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            spriteRenderer.enabled = true; // restore visibility
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (unusedBullets.TryDequeue(out BulletController bullet))
            {
                bullet.gameObject.SetActive(true);
                bullet.SetDamage(bulletDamage);
                bullet.SetData(transform.position + Vector3.up * 0.5f, Vector2.up);
            }
            else
            {
                var newBullet = Instantiate(bulletPrefab);
                newBullet.SetDamage(bulletDamage);
                newBullet.SetData(transform.position + Vector3.up * 0.5f, Vector2.up);
            }
            yield return new WaitForSeconds(bulletFrequency);
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            var item = collision.GetComponent<ItemController>();
            if (item.itemType == ItemType.Health)
            {
                playerHealth.Heal();
            }
            else if (item.itemType == ItemType.PowerUp)
            {
                bulletDamage += 10;
            }
            Destroy(collision.gameObject);
        }
    }
}
