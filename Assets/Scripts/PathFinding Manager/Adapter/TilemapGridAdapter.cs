using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Logic;

namespace IsometricGame.Adapters
{
    public class TilemapGridAdapter
    {
        private readonly Tilemap tilemap;

        public TilemapGridAdapter(Tilemap tilemap)
        {
            this.tilemap = tilemap;
        }

        /// <summary>
        /// Construye un PathGrid lógico en base al contenido visual del Tilemap.
        /// </summary>
        public LogicGrid.PathGrid BuildPathGrid()
        {
            // 1️⃣ Calcular bounds del Tilemap (en celdas)
            var bounds = tilemap.cellBounds;
            int width = bounds.size.x;
            int height = bounds.size.y;
            Debug.Log("El tilemap es de " + width + " por " + height);
            // 2️⃣ Crear configuración lógica
            var config = new LogicGrid.GridConfig(width, height);
            var pathGrid = new LogicGrid.PathGrid();
            pathGrid.Initialize(config);

            // 3️⃣ Recorrer cada celda visual y mapear sus propiedades
            foreach (var pos in bounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos);
                var c = new LogicGrid.CellIndex
                {
                    i = pos.x - bounds.xMin, // normalizar a índice 0-based
                    j = pos.y - bounds.yMin
                };

                if (tile == null)
                {
                    pathGrid.SetWalkable(c, false);
                    continue;
                }

            // 4️⃣/5️⃣ Regla simple: solo tiles con "grass" son caminables
            bool isGrass = tile.name != null && tile.name.ToLowerInvariant().Contains("grass");

            // Walkable: solo pasto
            pathGrid.SetWalkable(c, isGrass);

            // Costos: 0 si es pasto; infinito si no (por seguridad)
            pathGrid.SetCost(c, isGrass ? 0f : float.PositiveInfinity);

            }

            return pathGrid;
        }

        /// <summary>
        /// Devuelve un costo base según el tipo o nombre del tile.
        /// </summary>
        private float ClassifyTileCost(TileBase tile)
        {
            string name = tile.name.ToLowerInvariant();

            if (name.Contains("grass")) return 0.0f;
            if (name.Contains("dirt"))  return 0.2f;
            if (name.Contains("mud"))   return 0.5f;
            if (name.Contains("rock"))  return 1.0f;

            return 0.0f; // default
        }
    }
}
