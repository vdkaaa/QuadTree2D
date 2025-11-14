using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    

    Vector2 _input;

    void Update()
    {
        _input.x = Input.GetAxisRaw("Horizontal"); // A/D, Flechas
        _input.y = Input.GetAxisRaw("Vertical");   // W/S, Flechas
        _input = _input.normalized;

        transform.position += (Vector3)(_input * speed * Time.deltaTime);
    }
}
