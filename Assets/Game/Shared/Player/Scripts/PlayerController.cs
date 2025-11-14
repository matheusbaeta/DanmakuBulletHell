using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public BulletSystem bulletSystem;
    public float collisionDistance;

    public void OnMove(InputValue inputValue)
    {
        Vector2 movement = inputValue.Get<Vector2>();
        transform.position += (Vector3)movement * speed;
    }
}
