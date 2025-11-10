using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;

    [SerializeField] private Transform playerPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float direccionX = playerPos.position.x - transform.position.x;
        float direccionY = playerPos.position.y - transform.position.y;

        float longitud = Mathf.Sqrt(direccionX * direccionX + direccionY * direccionY);

        direccionX = direccionX / longitud;
        direccionY = direccionY / longitud;

        if (longitud > 0)
        {
            transform.position = new Vector3(transform.position.x + direccionX * speed * Time.deltaTime, transform.position.y + direccionY * speed * Time.deltaTime, transform.position.z);
        }
    }
}
