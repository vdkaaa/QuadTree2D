// Assets/Scripts/Debug/PathGridGizmos.cs
#nullable enable
using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Logic;
using IsometricGame.Adapters;

namespace IsometricGame.DebugTools
{
    /// <summary>
    /// Dibuja gizmos por celda del Tilemap para visualizar el PathGrid:
    /// - Verde: walkable (solo tiles cuyo nombre contiene "grass")
    /// - Rojo: no walkable
    /// </summary>
    [ExecuteAlways]
    public class PathGridGizmos : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Tilemap? tilemap;

        [Header("Apariencia")]
        [Tooltip("Reduce el tamaño del cuadro para que se vea un margen entre celdas.")]
        [Range(0f, 0.49f)]
        [SerializeField] private float inset = 0.05f;

        [Tooltip("Desfase en Z para evitar z-fighting con el arte.")]
        [SerializeField] private float zOffset = 0.0f;

        [Tooltip("Dibuja también el borde del cuadro.")]
        [SerializeField] private bool drawWire = true;

        [Tooltip("Opacidad del relleno.")]
        [Range(0f, 1f)]
        [SerializeField] private float fillAlpha = 0.35f;

        [Header("Reconstrucción")]
        [Tooltip("Reconstruir automáticamente cuando cambien valores en el inspector.")]
        [SerializeField] private bool autoRebuildOnValidate = true;

        private LogicGrid.PathGrid? _grid;
        private BoundsInt _cachedBounds;

        private void OnEnable()
        {
            Rebuild();
        }

        private void OnValidate()
        {
            if (autoRebuildOnValidate && isActiveAndEnabled)
                Rebuild();
        }

        /// <summary>
        /// Reconstruye el PathGrid desde el Tilemap usando la Opción A:
        /// solo 'grass' es caminable; todo lo demás no.
        /// </summary>
        public void Rebuild()
        {
            if (tilemap == null) { _grid = null; return; }

            // Usamos tu adaptador existente para respetar tu arquitectura
            var adapter = new TilemapGridAdapter(tilemap);
            _grid = adapter.BuildPathGrid();
            _cachedBounds = tilemap.cellBounds;
        }

        private void OnDrawGizmos()
        {
            if (tilemap == null || _grid == null) return;

            var cellSize = tilemap.cellSize;
            var half = new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0f);
            var drawSize = new Vector3(
                Mathf.Max(0.001f, cellSize.x - inset * 2f),
                Mathf.Max(0.001f, cellSize.y - inset * 2f),
                0.01f
            );

            // Recorremos el mismo bounds que usó el adaptador
            foreach (var pos in _cachedBounds.allPositionsWithin)
            {
                // Indices lógicos 0-based
                int i = pos.x - _cachedBounds.xMin;
                int j = pos.y - _cachedBounds.yMin;

                var data = _grid.SampleCell(i, j); // seguro ante fuera de rango
                var worldBase = tilemap.CellToWorld(pos);
                var center = worldBase + half;
                center.z += zOffset;

                // Colores según walkable
                Color fill = data.walkable
                    ? new Color(0f, 1f, 0f, fillAlpha)   // verde
                    : new Color(1f, 0f, 0f, fillAlpha);  // rojo

                Color wire = data.walkable
                    ? new Color(0f, 0.75f, 0f, 1f)
                    : new Color(0.75f, 0f, 0f, 1f);

                // Relleno
                Gizmos.color = fill;
                Gizmos.DrawCube(center, drawSize);

                // Borde
                if (drawWire)
                {
                    Gizmos.color = wire;
                    Gizmos.DrawWireCube(center, drawSize);
                }
            }
        }
    }
}
