using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public void OnMove(InputValue inputValue)
    {
        var movement = inputValue.Get<Vector2>();
        Debug.Log(movement);
        transform.position += (Vector3)movement * speed;
    }
}
