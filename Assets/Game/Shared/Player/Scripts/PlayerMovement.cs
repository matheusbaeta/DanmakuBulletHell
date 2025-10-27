using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public BulletSystem bulletSystem;
    public SpriteRenderer spriteRenderer;
    public float speed;

    public void OnMove(InputValue inputValue)
    {
        var movement = inputValue.Get<Vector2>();
        Debug.Log(movement);
        transform.position += (Vector3)movement * speed;
    }

    private void Update()
    {
        spriteRenderer.color = bulletSystem.playerHit ? Color.red : Color.white;
    }
}
