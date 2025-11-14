using UnityEngine;
using IsometricGame.Logic;

namespace IsometricGame.Interface
{
   public interface IGridCoordinateConverter
    {
    Vector3Int WorldToCell(Vector3 worldPos);
    Vector3 CellToWorld(Vector2Int cell);
    }
 
}
