using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public BulletSystem bulletSystem;
    public float collisionDistance;
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    public PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
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
            Debug.Log("HIT!!!");

            playerHealth.TakeHit();

            // Move player to start position
            transform.position = new Vector3(0, -4, 0);
        }
    }
}
