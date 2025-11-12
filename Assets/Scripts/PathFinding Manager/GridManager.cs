using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Adapters;
using IsometricGame.Logic;

public class GridManager : MonoBehaviour
{
    [Header("Scene References")]
    public Grid unityGrid;
    public Tilemap groundTilemap;   // arrastra aquí tu tilemap del escenario

    private LogicGrid.PathGrid pathGrid;
    private UnityGridAdapter gridAdapter;

    void Awake()
    {
        // 1️⃣ Crear adapter de Tilemap y generar la grilla lógica
        var tileAdapter = new TilemapGridAdapter(groundTilemap);
        pathGrid = tileAdapter.BuildPathGrid();

        // 2️⃣ Crear adapter visual para conversiones Mundo↔Celda
        gridAdapter = new UnityGridAdapter(unityGrid);

        Debug.Log($"PathGrid generado: {pathGrid.CountWalkables()} celdas caminables");
    }

    void OnDrawGizmosSelected()
    {
        if (pathGrid == null || gridAdapter == null) return;

        // Dibuja en escena las celdas walkables
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var c = new LogicGrid.CellIndex { i = i, j = j };
                if (pathGrid.IsWalkable(c))
                    gridAdapter.DrawCellGizmo(c, Color.cyan);
            }
        }
    }
}
