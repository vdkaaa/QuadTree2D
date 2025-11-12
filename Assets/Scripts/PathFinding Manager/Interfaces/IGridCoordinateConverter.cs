using UnityEngine;
using IsometricGame.Logic;

namespace IsometricGame.Interface
{
   public interface IGridCoordinateConverter
    {
    LogicGrid.CellIndex WorldToCell(Vector3 worldPos);
    Vector3 CellToWorld(LogicGrid.CellIndex cell);
    }
 
}
