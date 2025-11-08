using System.Collections.Generic;
using UnityEngine;

// Contiene las estructuras y la lógica principal para un QuadTree.
public class QuadTreeStructs
{
    // Define un rectángulo alineado con los ejes (AABB - Axis-Aligned Bounding Box).
    // Se renombra de 'Rect' a 'AABBRect' para evitar conflictos con UnityEngine.Rect.
    public struct AABBRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        // Constructor para facilitar la creación de rectángulos.
        public AABBRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    
    // Representa un nodo en el QuadTree.
    // Puede ser una hoja (con puntos) o una rama (con 4 hijos).
    public class QuadNode
    {
        public AABBRect bounds; // Los límites espaciales de este nodo.
        public int capacity; // Máximo número de puntos antes de subdividir.
        public int depth; // La profundidad del nodo en el árbol.
        public int maxDepth; // La máxima profundidad permitida para el árbol.
        public List<Vector2> points; // Lista de puntos si es un nodo hoja.
        public QuadNode[] children; // Array de 4 nodos hijos (NW, NE, SW, SE).

        // Comprueba si el nodo es una hoja (no tiene hijos).
        public bool IsLeaf()
        {
            //Si no existen hijos es una hoja
            return children == null;
        }
    }

    // Comprueba si un rectángulo contiene un punto.
    public static bool RectContains(AABBRect rect, Vector2 point)
    {
        return (point.x >= rect.X &&
                point.x < rect.X + rect.Width &&
                point.y >= rect.Y &&
                point.y < rect.Y + rect.Height);
    }

    // Comprueba si el rectángulo A se intersecta con el rectángulo B.
    public static bool RectIntersects(AABBRect a, AABBRect b)
    {
        // Si alguna de estas condiciones es verdadera, no hay intersección.
        // Se comprueba si A está completamente a la izquierda/derecha/arriba/abajo de B.
        return !(a.X + a.Width <= b.X ||
                 a.X >= b.X + b.Width ||
                 a.Y + a.Height <= b.Y ||  //Verificar esta de abajo
                 b.Y + b.Height <= a.Y);  // A está encima de B (coordenadas Y invertidas en Unity)
    }


    // Método de fábrica para crear un nuevo QuadNode.
    public static QuadNode CreateNode(AABBRect bounds, int capacity, int depth, int maxDepth)
    {
        QuadNode node = new QuadNode
        {
            bounds = bounds,
            capacity = capacity,
            depth = depth,
            maxDepth = maxDepth,
            points = new List<Vector2>(),
            children = null // Se inicializa sin hijos, es una hoja por defecto.
        };
        return node;
    }


    // Subdivide un nodo en 4 nodos hijos.
    public static void Subdivide(QuadNode node)
    {
        // No subdividir si ya se alcanzó la máxima profundidad.
        if (node.depth >= node.maxDepth) return;

        // Calcular dimensiones y posiciones para los 4 nuevos cuadrantes.
        float halfWidth = node.bounds.Width / 2f;
        float halfHeight = node.bounds.Height / 2f;
        float x = node.bounds.X;
        float y = node.bounds.Y;

        // Crear los rectángulos para cada cuadrante.
        // El orden común es: Noroeste, Noreste, Suroeste, Sureste.
        AABBRect nwRect = new AABBRect(x, y + halfHeight, halfWidth, halfHeight);
        AABBRect neRect = new AABBRect(x + halfWidth, y + halfHeight, halfWidth, halfHeight);
        AABBRect swRect = new AABBRect(x, y, halfWidth, halfHeight);
        AABBRect seRect = new AABBRect(x + halfWidth, y, halfWidth, halfHeight);

        // Crear los 4 nodos hijos.
        int nextDepth = node.depth + 1;
        node.children = new QuadNode[4];
        node.children[0] = CreateNode(nwRect, node.capacity, nextDepth, node.maxDepth); // NW
        node.children[1] = CreateNode(neRect, node.capacity, nextDepth, node.maxDepth); // NE
        node.children[2] = CreateNode(swRect, node.capacity, nextDepth, node.maxDepth); // SW
        node.children[3] = CreateNode(seRect, node.capacity, nextDepth, node.maxDepth); // SE

        // Redistribuir los puntos del nodo padre a los nuevos hijos.
        foreach (var point in node.points)
        {
            // Insertar cada punto en el hijo correspondiente.
            // Un punto solo puede pertenecer a un cuadrante.
            InsertPoint(node, point);
        }

        // Limpiar la lista de puntos del nodo padre, ya que ahora es una rama.
        node.points.Clear();
    }



    // Inserta un punto en el QuadTree.
    public static bool InsertPoint(QuadNode node, Vector2 point)
    {
        // Si el punto no está dentro de los límites de este nodo, ignorarlo.
        if (!RectContains(node.bounds, point))
        {
            return false;
        }

        // Si el nodo es una hoja y aún no ha alcanzado su capacidad...
        if (node.IsLeaf() && node.points.Count < node.capacity)
        {
            // ...simplemente añade el punto a la lista.
            node.points.Add(point);
            return true;
        }

        // Si el nodo es una hoja pero ya está lleno...
        if (node.IsLeaf())
        {
            // ...y no hemos alcanzado la profundidad máxima, hay que subdividir.
            if (node.depth < node.maxDepth)
            {
                Subdivide(node);
                // Después de subdividir, el nodo ya no es una hoja.
                // La inserción continuará en la siguiente sección.
            }
            else
            {
                // Si estamos en una hoja llena y en la profundidad máxima,
                // simplemente añadimos el punto. Se excede la capacidad, pero no hay más remedio.
                node.points.Add(point);
                return true;
            }
        }

        // Si el nodo ya no es una hoja (tiene hijos),
        // intentamos insertar el punto en el hijo apropiado.
        foreach (var child in node.children)
        {
            // La función de inserción se encargará de comprobar si el punto
            // pertenece a ese hijo. Si la inserción tiene éxito, terminamos.
            if (InsertPoint(child, point))
            {
                return true;
            }
        }

        // Esto no debería ocurrir si la lógica es correcta, pero es una salvaguarda.
        return false;
    }
    

    // Opcional: para métricas
    public class QueryStats
    {
        public int nodesVisited;
        public int pointsTested;
    }

    // ---- Queries ----
    public static void QueryRange(QuadNode node, AABBRect range, List<Vector2> results, QueryStats stats = null)
    {
        if (node == null) return;
        if (stats != null) stats.nodesVisited++;

        if (!RectIntersects(node.bounds, range)) return;

        if (node.IsLeaf())
        {
            if (stats != null) stats.pointsTested += node.points.Count;
            for (int i = 0; i < node.points.Count; i++)
            {
                var p = node.points[i];
                if (RectContains(range, p)) results.Add(p);
            }
            return;
        }

        // hijos: NW, NE, SW, SE (tu orden)
        for (int i = 0; i < 4; i++)
            QueryRange(node.children[i], range, results, stats);
    }

    public static bool RectIntersectsCircle(AABBRect r, Vector2 c, float rad)
    {
        float cx = Mathf.Clamp(c.x, r.X, r.X + r.Width);
        float cy = Mathf.Clamp(c.y, r.Y, r.Y + r.Height);
        float dx = cx - c.x;
        float dy = cy - c.y;
        return (dx * dx + dy * dy) <= (rad * rad);
    }

    public static void QueryCircle(QuadNode node, Vector2 center, float radius, List<Vector2> results, QueryStats stats = null)
    {
        if (node == null) return;
        if (stats != null) stats.nodesVisited++;

        if (!RectIntersectsCircle(node.bounds, center, radius)) return;

        float r2 = radius * radius;

        if (node.IsLeaf())
        {
            if (stats != null) stats.pointsTested += node.points.Count;
            for (int i = 0; i < node.points.Count; i++)
            {
                var p = node.points[i];
                var d = p - center;
                if (d.sqrMagnitude <= r2) results.Add(p);
            }
            return;
        }

        for (int i = 0; i < 4; i++)
            QueryCircle(node.children[i], center, radius, results, stats);
    }

    // ---- Helpers para dibujar en escena (Gizmos) ----
    public static void DrawNodeGizmos(QuadNode node, Color colorBounds, Color colorPoints)
    {
        if (node == null) return;

        // dibuja bounds
        var c = new Vector3(node.bounds.X + node.bounds.Width * 0.5f,
                            node.bounds.Y + node.bounds.Height * 0.5f, 0f);
        var s = new Vector3(node.bounds.Width, node.bounds.Height, 0f);

        Gizmos.color = colorBounds;
        Gizmos.DrawWireCube(c, s);

        if (node.IsLeaf())
        {
            Gizmos.color = colorPoints;
            for (int i = 0; i < node.points.Count; i++)
            {
                var p = node.points[i];
                Gizmos.DrawSphere(new Vector3(p.x, p.y, 0f), 0.02f);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
                DrawNodeGizmos(node.children[i], colorBounds, colorPoints);
        }
    }


}

    

