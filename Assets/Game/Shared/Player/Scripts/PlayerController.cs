using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;

    public void OnMove(InputValue inputValue)
    {
        Vector2 movement = inputValue.Get<Vector2>();
        transform.position += (Vector3)movement * _speed;
    }
}
