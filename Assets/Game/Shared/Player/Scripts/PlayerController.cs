using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public BulletSystem bulletSystem;
    public SpriteRenderer spriteRenderer;
    public float speed;
    public float collisionDistance;
    public PlayerData player;

    public void Awake()
    {
        player = new PlayerData(3, 1);
    }

    public void OnMove(InputValue inputValue)
    {
        var movement = inputValue.Get<Vector2>();
        Debug.Log(movement);
        transform.position += (Vector3)movement * speed;
    }

    private void Update()
    {
        spriteRenderer.color = bulletSystem.playerHit ? Color.red : Color.white;

        if(bulletSystem.playerHit && !player.isDead)
        {
            player.DecreaseRemaningLifes();
            transform.position = new Vector3(0, -7, 0);
        }

        if(player.isDead)
        {
            Debug.Log("you died");
            // SceneManager.LoadScene("MainMenu");
        }
    }
}
