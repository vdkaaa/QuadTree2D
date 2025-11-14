using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private TileCursor tileCursor; // Referencia al cursor
    [SerializeField] private float velocidadMovimiento = 5.0f;

    private List<Nodo> caminoActual;
    private Coroutine corrutinaMovimiento;

    void Update()
    {
        // Al hacer clic
        if (Input.GetMouseButtonDown(0))
        {
            // Pedir una nueva ruta
            IniciarBusquedaDeCamino();
        }
    }

    private void IniciarBusquedaDeCamino()
    {
        // 1. Obtener Nodo Inicial (posición actual del Player)
        Vector2Int posGridPlayer = pathfinder.MundoAColumnaFila(transform.position);
        Nodo nodoInicial = pathfinder.GetGrid().GetNodo(posGridPlayer.x, posGridPlayer.y);

        // 2. Obtener Nodo Final (posición actual del Cursor)
        Vector2Int posGridCursor = tileCursor.PosicionGrid;
        Nodo nodoFinal = pathfinder.GetGrid().GetNodo(posGridCursor.x, posGridCursor.y);

        // 3. Verificar que los nodos sean válidos y caminables
        if (nodoInicial == null || nodoFinal == null || !nodoFinal.IsWalkable)
        {
            Debug.Log("Destino no válido o no caminable.");
            return;
        }

        // 4. Calcular el camino
        caminoActual = pathfinder.FindPath(nodoInicial, nodoFinal);

        // 5. Si hay un camino, detener el movimiento anterior y seguir el nuevo
        if (caminoActual != null)
        {
            if (corrutinaMovimiento != null)
            {
                StopCoroutine(corrutinaMovimiento);
            }
            corrutinaMovimiento = StartCoroutine(SeguirCamino(caminoActual));
        }
        else
        {
            Debug.Log("No se encontró camino.");
        }
    }

    // Corrutina para moverse suavemente a lo largo del camino
    private IEnumerator SeguirCamino(List<Nodo> camino)
    {
        // Empezar desde el segundo nodo (el primero es donde ya estamos)
        for (int i = 1; i < camino.Count; i++)
        {
            // Obtener la posición del mundo del siguiente nodo
            Vector3 destino = pathfinder.ColumnaFilaAMundo(camino[i].X, camino[i].Y);

            // Moverse hacia ese destino
            while (Vector3.Distance(transform.position, destino) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
                yield return null; // Esperar al siguiente frame
            }
            
            // Asegurarse de que el jugador quede exactamente en el destino
            transform.position = destino;
        }
        
        // Se completó el camino
        corrutinaMovimiento = null;
    }
}