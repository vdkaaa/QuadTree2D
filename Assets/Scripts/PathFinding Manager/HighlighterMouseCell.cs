using UnityEngine;
using UnityEngine.Tilemaps;
using IsometricGame.Logic;
using System;
using UnityEngine.PlayerLoop;




public class HighlighterMouseCell : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera cam;           // Si se deja vacío, usa Camera.main
    [SerializeField] private Tilemap tilemap;      // Tilemap a inspeccionar

    [SerializeField] private GameObject mouseTarget;

    [SerializeField]
    private bool inBounds = false;

    public Action<LogicGrid.CellIndex, bool> OnHoverCellChanged;
    private BoundsInt _bounds;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
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
        Debug.Log("Posicion del Mouse " + mousePos);
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        Debug.Log("Posicion del mundo " + worldPos);
    }
}