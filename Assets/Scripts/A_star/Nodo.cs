using UnityEngine;

public class Nodo 
{
    // --- Información del Grid ---
    // Propiedades de solo lectura (se establecen 1 vez)
    public int X { get; }
    public int Y { get; }

    // Propiedad pública, se puede cambiar (para el Tilemap)
    public bool IsWalkable { get; set; }

    // --- Información de A* ---
    // El Pathfinder necesita leer y escribir esto
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; private set; } // El set es privado, solo se calcula con CalcFCost()

    // --- Reconstrucción del Camino ---
    public Nodo NodoPadre { get; set; }

    // --- Constructor ---
    // Se llama desde la clase Grid para crear un nuevo nodo
    public Nodo(int x, int y, bool isWalkable) 
    {
        this.X = x;
        this.Y = y;
        this.IsWalkable = isWalkable;

        // Inicializar costos (opcional, pero buena práctica)
        GCost = int.MaxValue; // Infinito al inicio
        HCost = 0;
        FCost = int.MaxValue;
        NodoPadre = null;
    }

    // --- Métodos ---
    public void CalcFCost()
    {
        FCost = GCost + HCost;
    }
}