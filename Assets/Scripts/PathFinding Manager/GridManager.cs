using IsometricGame.Logic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 5;
    public int gridHeigth = 5;


    private LogicGrid.PathGrid grid;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Paso 1: crear la configuración base
        LogicGrid.GridConfig cfg = new LogicGrid.GridConfig(gridWidth, gridHeigth);


        // Paso 2: crear la grilla lógica e inicializarla
        grid = new LogicGrid.PathGrid();
        grid.Initialize(cfg);
        // Paso 3: pruebas de modificación
        grid.GetAllMap();

    }
}
