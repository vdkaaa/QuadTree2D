using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private Tilemap tilemapDeObstaculos;
    [SerializeField] private Tilemap tilemapCaminable;
    // Puedes ajustar esto en el Inspector de Unity
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 10;

    [SerializeField] private Vector2Int gridOrigin = Vector2Int.zero;
    // Inicializa el grid al empezar la escena
    void Awake()
    {
        // 1) Tomar los bounds del tilemap caminable
        var bounds = tilemapCaminable.cellBounds;

        // Origen real del área que tiene tiles
        gridOrigin = new Vector2Int(bounds.xMin, bounds.yMin);

        // Tamaño real en celdas
        gridWidth  = bounds.size.x;
        gridHeight = bounds.size.y;

        Debug.Log($"Grid origin: {gridOrigin}, size: {gridWidth}x{gridHeight}");

        // 2) Crear el grid con esos datos
        this.grid = new Grid(gridWidth, gridHeight, tilemapCaminable, tilemapDeObstaculos, gridOrigin);
    }

    // --- ¡TAMBIÉN ACTUALIZA ESTOS MÉTODOS! ---
    // Necesitan saber el offset para convertir coordenadas correctamente

    public Vector2Int MundoAColumnaFila(Vector3 posicionMundo)
    {
        Vector3Int cellPosition = tilemapCaminable.WorldToCell(posicionMundo);
        // ¡Restar el origen!
        return new Vector2Int(cellPosition.x - gridOrigin.x, cellPosition.y - gridOrigin.y);
    }

    public Vector3 ColumnaFilaAMundo(int x, int y)
    {
        // ¡Sumar el origen!
        Vector3 worldPos = tilemapCaminable.GetCellCenterWorld(new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0));
        return worldPos;
    }

    public List<Nodo> FindPath(Nodo inicio, Nodo fin)
    {
        grid.ResetPathData();
        // 1. Inicializar las listas
        // openList es una Lista porque necesitamos encontrar el F-cost más bajo
        List<Nodo> openList = new List<Nodo>();
        
        // closedSet es un HashSet porque solo necesitamos saber si "Contiene" un nodo,
        // y HashSet.Contains() es muchísimo más rápido que List.Contains().
        HashSet<Nodo> closedSet = new HashSet<Nodo>();

        // 2. Añadir el nodo inicial
        inicio.GCost = 0; // El costo para llegar al inicio es 0
        inicio.HCost = HeuristicDistance(inicio, fin); // Calcular H
        inicio.CalcFCost(); // Calcular F
        openList.Add(inicio);

        // 3. Iniciar el bucle de búsqueda
        while (openList.Count > 0)
        {
            // 4. Encontrar el nodo con el F-Cost MÁS BAJO en la listaAbierta
            Nodo nodoActual = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < nodoActual.FCost || 
                    (openList[i].FCost == nodoActual.FCost && openList[i].HCost < nodoActual.HCost))
                {
                    nodoActual = openList[i];
                }
            }

            // 5. Mover el nodo actual de "Abierta" a "Cerrada"
            openList.Remove(nodoActual);
            closedSet.Add(nodoActual);

            // 6. ¡ÉXITO! Se encontró el camino
            if (nodoActual == fin)
            {
                return ReBuildPath(inicio, fin);
            }

            // 7. Recorrer los vecinos del nodo actual
            foreach (Nodo vecino in GetNeighbours(nodoActual))
            {
                // 8. Omitir el vecino si no es caminable o si ya está en la lista cerrada
                if (!vecino.IsWalkable || closedSet.Contains(vecino))
                {
                    continue; // Saltar al siguiente vecino
                }

                // 9. Calcular el nuevo costo G para este vecino
                int nuevoGCostParaVecino = nodoActual.GCost + HeuristicDistance(nodoActual, vecino);

                // 10. Si este camino al vecino es mejor (más corto) O el vecino no está en la lista abierta
                if (nuevoGCostParaVecino < vecino.GCost || !openList.Contains(vecino))
                {
                    // Actualizar costos
                    vecino.GCost = nuevoGCostParaVecino;
                    vecino.HCost = HeuristicDistance(vecino, fin);
                    vecino.CalcFCost();
                    vecino.NodoPadre = nodoActual; // ¡Muy importante!

                    // Añadir a la lista abierta si no estaba ya
                    if (!openList.Contains(vecino))
                    {
                        openList.Add(vecino);
                    }
                }
            }
        }

        // 11. FRACASO: No se encontró camino (la listaAbierta está vacía)
        return null;
    }

    private List<Nodo> ReBuildPath(Nodo inicio, Nodo fin)
    {
        List<Nodo> path = new List<Nodo>();
        Nodo nodoActual = fin;

        while (nodoActual != inicio)
        {
            path.Add(nodoActual);
            nodoActual = nodoActual.NodoPadre; // Ir hacia atrás
        }
        
        // Añadir el nodo inicial (el bucle se detiene antes de añadirlo)
        path.Add(inicio); 
        
        path.Reverse(); // El camino está al revés (del final al inicio), así que lo invertimos
        return path;
    }

    // ¡Firma corregida!
    private int HeuristicDistance(Nodo a, Nodo b)
    {
        // Esta es la Distancia Manhattan
        int distX = Mathf.Abs(a.X - b.X);
        int distY = Mathf.Abs(a.Y - b.Y);
        
        // Asumiendo 10 por movimiento horizontal/vertical y 14 por diagonal
        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    // Corregí el nombre 'Obtein'
    private List<Nodo> GetNeighbours(Nodo nodo)
    {
         List<Nodo> neighbours = new List<Nodo>();

         // Búsqueda en 3x3 (o solo 4 direcciones si no quieres diagonales)
         for (int x = -1; x <= 1; x++)
         {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Es el mismo nodo

                int checkX = nodo.X + x;
                int checkY = nodo.Y + y;

                // Usamos GetNodo para seguridad
                Nodo vecino = grid.GetNodo(checkX, checkY);
                if (vecino != null) // Si no es null (está dentro del grid)
                {
                    neighbours.Add(vecino);
                }
            }
         }
         return neighbours;
    }

    // 1. Un "getter" público para que otros scripts lean el grid
    public Grid GetGrid()
    {
    return grid;
    }



}