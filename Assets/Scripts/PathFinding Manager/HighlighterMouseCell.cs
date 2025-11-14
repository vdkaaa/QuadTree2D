using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Logic;
using System;
using UnityEngine.PlayerLoop;
using IsometricGame.Adapters;




public class HighlighterMouseCell : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera cam;           // Si se deja vacío, usa Camera.main
    [SerializeField] private Tilemap tilemap;      // Tilemap a inspeccionar
    [SerializeField] private GameObject mouseTarget;
    [SerializeField] private Grid unityGrid;
    private LogicGrid.PathGrid pathGrid;
    private UnityGridAdapter gridAdapter;


    public Action<LogicGrid.CellIndex, bool> OnHoverCellChanged;
    private BoundsInt _bounds;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var tileAdapter = new TilemapGridAdapter(tilemap);
        pathGrid = tileAdapter.BuildPathGrid();
        mouseTarget.SetActive(false);
        // 2️⃣ Crear adapter visual para conversiones Mundo↔Celda
        gridAdapter = new UnityGridAdapter(unityGrid);

        if (!cam) cam = Camera.main;
        _bounds = tilemap ? tilemap.cellBounds : new BoundsInt();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!tilemap) return;
        if (!cam) cam = Camera.main;

        Vector3 mousePos = Input.mousePosition;
        //Distancia de los dos puntos de la camara al tilemap
        float depth = Mathf.Abs(tilemap.transform.position.z - cam.transform.position.z);
        //Debug.Log("Posicion del Mouse " + mousePos);
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y, depth));

        //Debug.Log("Posicion del mundo " + worldPos);

        Vector3Int cellPos = gridAdapter.WorldToCell(worldPos);

        //Debug.Log(cellPos);
        
        int i = cellPos.x -tilemap.cellBounds.xMin;
        int j = cellPos.y - tilemap.cellBounds.yMin;
        var index = new LogicGrid.CellIndex(i,j);

        if (!pathGrid.InBounds(index))
        {
            mouseTarget.SetActive(false);
            return;
        }
        else
        {
            if (pathGrid.IsWalkable(index))
            {
                //Debug.Log("Es caminable");
                var worldCellCenter = tilemap.GetCellCenterWorld(cellPos);
                mouseTarget.transform.position = new Vector3(worldCellCenter.x,worldCellCenter.y,0);
                mouseTarget.SetActive(true);
            }
            else
            {              
                mouseTarget.SetActive(false);
            }
        }
        

    }
}