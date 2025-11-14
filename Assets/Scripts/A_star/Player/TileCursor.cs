using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCursor : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private SpriteRenderer spriteRenderer; // El sprite de tu cursor
    
    [SerializeField] private Color colorCaminable = Color.green;
    [SerializeField] private Color colorObstaculo = Color.red;

    private Camera camaraPrincipal;
    
    // Propiedad pública para que el Player sepa dónde está el cursor
    public Vector2Int PosicionGrid { get; private set; }

    void Start()
    {
        camaraPrincipal = Camera.main;
        
        // Ocultar el cursor original del mouse
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mousepos = Input.mousePosition;

        // 1. Obtener la posición del mouse en el mundo
        Vector3 posMouseMundo = camaraPrincipal.ScreenToWorldPoint(new Vector3(mousepos.x,mousepos.y,10f));

        // 2. Convertir la posición del mundo a la celda del grid
        PosicionGrid = pathfinder.MundoAColumnaFila(posMouseMundo);

        // 3. Obtener el centro de esa celda para posicionar el cursor visual
        transform.position = pathfinder.ColumnaFilaAMundo(PosicionGrid.x, PosicionGrid.y);

        // 4. Obtener el nodo de esa posición
        Nodo nodoCursor = pathfinder.GetGrid().GetNodo(PosicionGrid.x, PosicionGrid.y);

        // 5. Actualizar el color del cursor
        if (nodoCursor != null && nodoCursor.IsWalkable)
        {
            spriteRenderer.color = colorCaminable;
        }
        else
        {
            spriteRenderer.color = colorObstaculo;
        }
    }
}