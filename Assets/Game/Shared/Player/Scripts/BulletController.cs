using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 20f;
    public int damage = 40;
    private Vector2 _direction = Vector2.up;

    public void SetData(Vector2 position, Vector2 direction)
    {
        transform.position = position;
        _direction = direction.normalized;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = _direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            enemy.TakeDamage(damage);
        }

        PlayerController.Instance.unusedBullets.Enqueue(this);
        gameObject.SetActive(false);
    }
}