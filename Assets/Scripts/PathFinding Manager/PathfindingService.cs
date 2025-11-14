using System;
using System.Collections.Generic;
using IsometricGame.Logic; // Asumiendo tu namespace
using UnityEngine;

public class PathfindingService
{
    /// <summary>
    /// Encuentra un camino desde startCell a goalCell usando el algoritmo A*.
    /// </summary>
    /// <returns>Una lista de Vector2Int con el camino, o una lista vacía si no se encuentra.</returns>
    public List<Vector2Int> FindPath(Vector2Int startCell, Vector2Int goalCell, LogicGrid.PathGrid pathGrid)
    {
        // 1. Caso trivial: ya estamos en el destino
        if (startCell == goalCell)
        {
            return new List<Vector2Int> { startCell };
        }

        // 2. Inicialización de las colecciones
        var openSet = new HashSet<Vector2Int>();
        var closedSet = new HashSet<Vector2Int>();

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, float>();
        var fScore = new Dictionary<Vector2Int, float>();

        // 3. Configuración de la celda inicial
        gScore[startCell] = 0;
        fScore[startCell] = Heuristic(startCell, goalCell);
        openSet.Add(startCell);

        // 4. El bucle principal del algoritmo A*
        while (openSet.Count > 0)
        {
            // 4a. Encontrar la celda en openSet con el fScore más bajo
            var currentCell = CellfScoreLowCost(openSet, fScore);

            // 4b. Comprobar si hemos llegado al final
            if (currentCell == goalCell)
            {
                return ReBuildPath(cameFrom, currentCell);
            }

            // 4c. Mover la celda actual de "abiertos" a "cerrados"
            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            // 4d. Procesar los vecinos de la celda actual
            
            foreach (var neighbor in GetNeighbors(currentCell, pathGrid))
            {
                // Si el vecino ya está evaluado (en cerrados), ignorarlo.
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Si el vecino no es caminable, ignorarlo.
                if (!pathGrid.IsWalkable(neighbor)) // <--- Asunción 1
                {
                    closedSet.Add(neighbor); // Opcional: tratarlo como cerrado
                    continue;
                }

                // Costo desde el inicio hasta la celda actual
                float currentGScore = gScore.GetValueOrDefault(currentCell, float.PositiveInfinity);
                
                // Costo del paso (1 para una grilla uniforme)
                float stepCost = 1; 
                float tentativeG = currentGScore + stepCost;

                // Si este camino al vecino es peor que uno ya existente, ignorarlo
                float neighborGScore = gScore.GetValueOrDefault(neighbor, float.PositiveInfinity);
                if (tentativeG >= neighborGScore)
                {
                    continue; // No es un camino mejor
                }

                // ¡Es un camino mejor! Lo registramos.
                cameFrom[neighbor] = currentCell;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, goalCell);

                // Si no está en openSet, lo añadimos para evaluarlo
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }
        }

        // 5. Si salimos del bucle sin encontrar el 'goalCell', no hay camino
        return new List<Vector2Int>();
    }

    // --- FUNCIONES AUXILIARES ---

    /// <summary>
    /// Reconstruye el camino hacia atrás desde la celda final.
    /// </summary>
    private List<Vector2Int> ReBuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var pathWay = new List<Vector2Int>() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            pathWay.Insert(0, current); // Añadir al inicio de la lista
        }
        return pathWay;
    }

    /// <summary>
    /// Encuentra la celda en el openSet con el fScore más bajo.
    /// </summary>
    private Vector2Int CellfScoreLowCost(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
    {
        Vector2Int bestcell = new Vector2Int();
        float bestF = float.PositiveInfinity;

        foreach (var item in openSet)
        {
            float f;
            
            // Usamos GetValueOrDefault para seguridad
            if (!fScore.TryGetValue(item, out f))
            {
                f = float.PositiveInfinity;
            }

            if (f < bestF)
            {
                bestF = f;
                bestcell = item;
            }
        }
        return bestcell;
    }

    /// <summary>
    /// Calcula la heurística (Distancia Manhattan) entre dos celdas.
    /// </summary>
    private float Heuristic(Vector2Int cellA, Vector2Int cellB)
    {
        var dx = Mathf.Abs(cellA.x - cellB.x);
        var dy = Mathf.Abs(cellA.y - cellB.y);
        return dx + dy;
    }

    /// <summary>
    /// Obtiene los vecinos caminables de una celda (4 direcciones).
    /// </summary>
    private List<Vector2Int> GetNeighbors(Vector2Int cell, LogicGrid.PathGrid pathGrid)
    {
        var neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1),  // Arriba
            new Vector2Int(0, -1), // Abajo
            new Vector2Int(-1, 0), // Izquierda
            new Vector2Int(1, 0)   // Derecha
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = cell + dir;

            // Comprobar si el vecino está dentro de los límites
            if (pathGrid.InBounds(neighbor)) // <--- Asunción 2
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
}