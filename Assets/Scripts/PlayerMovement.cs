using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;


    [Header("Colisiones")]
    [SerializeField] private Vector2 boxPosition;
    [SerializeField] private Vector2 boxSize;

    private BoxCollider2D boxCollider;


    private Rigidbody2D rigidBody;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPosition + boxSize / 2, boxSize);    
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rigidBody.linearVelocity = movement * speed ;

        ColisionConCaja(transform.position, boxCollider.size, boxPosition, boxSize);

    }
    


    private bool ColisionConCaja(Vector2 playerPos, Vector2 playerSize, Vector2 box, Vector2 boxSize)
    {
        if (playerPos.x < box.x + boxSize.x
           && playerPos.x + playerSize.x > box.x
           && playerPos.y < box.y + boxSize.y
           && playerPos.y + playerSize.y > box.y)
        {
            Debug.Log("Colision con caja");
            return true;
        }
        return false;
    }
}
