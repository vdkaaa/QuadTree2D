// Scripts/Adapters/UnityGridAdapter.cs
using UnityEngine;
using IsometricGame.Logic;
using IsometricGame.Interface;

namespace IsometricGame.Adapters
{
    /// <summary>
    /// Traduce coordenadas entre el Grid de Unity y tu PathGrid lógico.
    /// </summary>
    public class UnityGridAdapter : IGridCoordinateConverter
    {
        private readonly Grid unityGrid;
        private readonly Vector3 cellSize;
        private readonly Vector3 cellGap;      // por si usas separación
        private readonly Vector3 originOffset; // offset del transform del Grid
        private readonly bool isIsoZAsY;

        public UnityGridAdapter(Grid grid)
        {
            unityGrid   = grid;
            cellSize    = grid.cellSize;
            cellGap     = grid.cellGap;
            originOffset= grid.transform.position;
            isIsoZAsY   = (grid.cellLayout == GridLayout.CellLayout.IsometricZAsY);
        }

        /// <summary>
        /// Mundo → Celda (índices i,j de tu PathGrid)
        /// </summary>
        public LogicGrid.CellIndex WorldToCell(Vector3 worldPos)
        {
            // Ojo: WorldToCell ya considera rotación/escala del Grid isométrico.
            var cell = unityGrid.WorldToCell(worldPos);
            return new LogicGrid.CellIndex { i = cell.x, j = cell.y };
        }

        /// <summary>
        /// Celda → Mundo (centro visual del tile)
        /// </summary>
        public Vector3 CellToWorld(LogicGrid.CellIndex cell)
        {
            // Unity devuelve la “esquina” del tile; centramos sumando mitad del cellSize.
            var cellPos   = new Vector3Int(cell.i, cell.j, 0);
            var worldBase = unityGrid.CellToWorld(cellPos);
            var center    = worldBase + new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0f);

            // En isometric Z-as-Y puedes querer fijar z=0 para evitar profundidad no deseada.
            if (isIsoZAsY) center.z = 0f;

            return center;
        }

        // --------- Helpers opcionales ---------

        /// <summary> Dibuja un wireframe del tile para debug en Scene view. </summary>
        public void DrawCellGizmo(LogicGrid.CellIndex cell, Color color)
        {
            var center = CellToWorld(cell);
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, new Vector3(cellSize.x, cellSize.y, 0.01f));
        }
    }
}
