using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Logic;

namespace IsometricGame.DebugTools
{
    /// <summary>
    /// Detecta la celda bajo el mouse y la expone como LogicGrid.CellIndex (0-based).
    /// Dibuja un highlight visual en la escena para depuración.
    /// - Soporta Tilemap isométrico (usa orientationMatrix para alinear el rombo).
    /// - No modifica tu PathGrid; sólo calcula índices a partir del Tilemap.
    /// </summary>
    [ExecuteAlways]
    public class MouseCellHighlighter : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Camera cam;           // Si se deja vacío, usa Camera.main
        [SerializeField] private Tilemap tilemap;      // Tilemap a inspeccionar

        [Header("Visual")]
        [Range(0f, 0.49f)] public float inset = 0.05f; // margen interno del rombo
        public float zOffset = 0.02f;                  // levantar un poco para evitar z-fighting
        public Color wireColor = new Color(1f, 1f, 0f, 1f); // contorno (amarillo)
        public Color fillColor = new Color(1f, 1f, 0f, 0.25f); // relleno translúcido
        public bool drawFill = true;
        public bool drawWire = true;

        [Header("Salida (solo lectura)")]
        [SerializeField, Tooltip("Índice 0-based dentro de los bounds del Tilemap")]
        private int hoverI = -1;
        [SerializeField]
        private int hoverJ = -1;
        [SerializeField]
        private bool inBounds = false;

        public Action<LogicGrid.CellIndex, bool> OnHoverCellChanged;

        private BoundsInt _bounds;
        private Matrix4x4 _orient;

        // cache del quad para dibujar relleno
        private Mesh _quad;

        void OnEnable()
        {
            if (!cam) cam = Camera.main;
            _bounds = tilemap ? tilemap.cellBounds : new BoundsInt();
            _orient = tilemap ? tilemap.orientationMatrix : Matrix4x4.identity;

            if (_quad == null)
            {
                _quad = new Mesh { name = "MouseCellRombo" };
                _quad.vertices  = new Vector3[4];
                _quad.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                _quad.normals   = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            }
        }

        void OnValidate()
        {
            if (tilemap)
            {
                _bounds = tilemap.cellBounds;
                _orient = tilemap.orientationMatrix;
            }
        }

        void Update()
        {
            if (!tilemap) return;
            if (!cam) cam = Camera.main;

            // 1) Mouse → mundo (z correcto para perspectiva u ortho)
            Vector3 mp = Input.mousePosition;
            float depth = cam.orthographic
                ? (tilemap.transform.position.z - cam.transform.position.z)
                : cam.WorldToScreenPoint(tilemap.transform.position).z;

            Vector3 world = cam.ScreenToWorldPoint(new Vector3(mp.x, mp.y, depth));

            // 2) Mundo → celda (coordenadas de tilemap)
            Vector3Int cell = tilemap.WorldToCell(world);

            // 3) Normalizar a 0-based dentro de bounds
            int i = cell.x - _bounds.xMin;
            int j = cell.y - _bounds.yMin;

            bool inside = (i >= 0 && i < _bounds.size.x && j >= 0 && j < _bounds.size.y);

            // 4) Notificar si cambió
            if (inside != inBounds || i != hoverI || j != hoverJ)
            {
                inBounds = inside;
                hoverI = inside ? i : -1;
                hoverJ = inside ? j : -1;

                OnHoverCellChanged?.Invoke(new LogicGrid.CellIndex { i = hoverI, j = hoverJ }, inBounds);
            }
        }

        /// <summary>
        /// Devuelve el índice actual (0-based) y si está dentro de los bounds.
        /// </summary>
        public (LogicGrid.CellIndex cell, bool isInside) GetHoverCell()
        {
            return (new LogicGrid.CellIndex { i = hoverI, j = hoverJ }, inBounds);
        }

        void OnDrawGizmos()
        {
            if (!tilemap) return;
            if (!inBounds) return;

            // Reconstruye datos por si cambió algo en editor
            _bounds = tilemap.cellBounds;
            _orient = tilemap.orientationMatrix;

            // Celda world-space
            int cx = hoverI + _bounds.xMin;
            int cy = hoverJ + _bounds.yMin;
            Vector3Int cell = new Vector3Int(cx, cy, 0);
            Vector3 worldBase = tilemap.CellToWorld(cell);

            float w = tilemap.cellSize.x;
            float h = tilemap.cellSize.y;
            float insetX = Mathf.Clamp(inset, 0f, 0.49f) * w * 2f * 0.5f;
            float insetY = Mathf.Clamp(inset, 0f, 0.49f) * h * 2f * 0.5f;

            // Cuatro esquinas locales (cuadrado), luego orientationMatrix → rombo
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(0f + insetX, 0f + insetY, 0f);
            corners[1] = new Vector3(w  - insetX, 0f + insetY, 0f);
            corners[2] = new Vector3(w  - insetX, h  - insetY, 0f);
            corners[3] = new Vector3(0f + insetX, h  - insetY, 0f);

            for (int k = 0; k < 4; k++)
            {
                corners[k] = worldBase + _orient.MultiplyPoint3x4(corners[k]);
                corners[k].z += zOffset;
            }

            if (drawWire)
            {
                Gizmos.color = wireColor;
                Gizmos.DrawLine(corners[0], corners[1]);
                Gizmos.DrawLine(corners[1], corners[2]);
                Gizmos.DrawLine(corners[2], corners[3]);
                Gizmos.DrawLine(corners[3], corners[0]);
            }

            if (drawFill && _quad != null)
            {
                _quad.vertices = corners;
                _quad.RecalculateBounds();
                Gizmos.color = fillColor;
                Gizmos.DrawMesh(_quad);
            }
        }
    }
}
