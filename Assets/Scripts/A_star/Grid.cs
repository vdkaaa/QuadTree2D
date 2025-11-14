using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid 
{
    private readonly int width;
    private readonly int height;
    private readonly Nodo[,] nodos;
    private readonly Vector2Int gridOrigin;

    // --- Constructor ---
    public Grid(int width, int height, Tilemap tilemapCaminable, Tilemap tilemapDeObstaculos, Vector2Int gridOrigin) // <-- MODIFICADO
    {
        this.width = width;
        this.height = height;
        this.nodos = new Nodo[width, height];
        this.gridOrigin = gridOrigin; // <-- AÑADIR ESTO

        BuildGrid(tilemapCaminable, tilemapDeObstaculos);
    }   

    // Método para crear los nodos (se llama 1 vez)
    // MODIFICADO: Lee los tilemaps para determinar si es caminable
    private void BuildGrid(Tilemap tilemapCaminable, Tilemap tilemapDeObstaculos)
    {
    int caminables = 0;
    int obstaculos = 0;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Vector3Int posicionCelda = new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0); 

            bool hayCaminable = tilemapCaminable.HasTile(posicionCelda);
            bool hayObstaculo = tilemapDeObstaculos.HasTile(posicionCelda);

            bool esCaminable = hayCaminable && !hayObstaculo;

            if (esCaminable) caminables++;
            if (hayObstaculo) obstaculos++;

            nodos[x, y] = new Nodo(x, y, esCaminable);
        }
        }

        Debug.Log($"Grid generado: {caminables} celdas caminables, {obstaculos} celdas con obstáculo, width={width}, height={height}, origin={gridOrigin}");
        }


    // Método para obtener un nodo (corregí el nombre 'Obtein')
    // Necesita 'x' e 'y' como parámetros
    public Nodo GetNodo(int x, int y)
    {
        // Añadir comprobación de límites para evitar errores
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return nodos[x, y];
        }
        return null; // Fuera del grid
    }

        public void ResetPathData()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Nodo n = nodos[x, y];
                n.GCost = int.MaxValue;
                n.HCost = 0;
                n.CalcFCost();
                n.NodoPadre = null;
            }
        }
    }
    
    // Propiedades útiles para que el Pathfinder sepa el tamaño
    public int Width => width;
    public int Height => height;
}