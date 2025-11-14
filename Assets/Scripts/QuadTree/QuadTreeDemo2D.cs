using System.Collections.Generic;
using UnityEngine;
using static QuadTreeStructs;

public class QuadTreeDemo2D : MonoBehaviour
{
    [Header("Bounds del Quadtree (XY plano mundo)")]
    public Vector2 origin = new Vector2(-5, -5);
    public Vector2 size = new Vector2(10, 10);

    [Header("Parámetros")]
    [Min(1)] public int capacity = 4;
    [Min(0)] public int maxDepth = 8;
    [Min(0)] public int spawnCount = 250;
    public int randomSeed = 1234;

    [Header("Visual")]
    public bool autoRebuildOnValidate = true;
    public Color colorBounds = new Color(1, 1, 0, 0.8f);
    public Color colorPoints = new Color(0, 1, 1, 0.8f);
    public bool drawGizmos = true;

    [Header("Query (debug)")]
    public bool showRangeQuery;
    public Rect rangeRect = new Rect(-2, -2, 4, 4);
    public bool showCircleQuery;
    public Vector2 circleCenter = Vector2.zero;
    public float circleRadius = 2f;

    QuadNode _root;
    List<Vector2> _allPoints = new();
    List<Vector2> _queryResults = new();

    void Rebuild()
    {
        var aabb = new AABBRect(origin.x, origin.y, size.x, size.y);
        _root = CreateNode(aabb, capacity, 0, maxDepth);

        _allPoints.Clear();
        var rnd = new System.Random(randomSeed);
        for (int i = 0; i < spawnCount; i++)
        {
            float x = (float)(origin.x + rnd.NextDouble() * size.x);
            float y = (float)(origin.y + rnd.NextDouble() * size.y);
            var p = new Vector2(x, y);
            _allPoints.Add(p);
            InsertPoint(_root, p);
        }
    }

    void Awake()  { Rebuild(); }
    void Start()  { /* opcional */ }

    void OnValidate()
    {
        if (!Application.isPlaying && autoRebuildOnValidate)
            Rebuild();
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || _root == null) return;

        // dibuja árbol y puntos
        DrawNodeGizmos(_root, colorBounds, colorPoints);

        // queries de depuración
        _queryResults.Clear();
        if (showRangeQuery)
        {
            var rr = new AABBRect(rangeRect.x, rangeRect.y, rangeRect.width, rangeRect.height);
            QueryRange(_root, rr, _queryResults, null);

            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(new Vector3(rr.X + rr.Width * 0.5f, rr.Y + rr.Height * 0.5f, 0), new Vector3(rr.Width, rr.Height, 0));

            Gizmos.color = Color.green;
            foreach (var p in _queryResults)
                Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), 0.03f);
        }

        if (showCircleQuery)
        {
            _queryResults.Clear();
            QueryCircle(_root, circleCenter, circleRadius, _queryResults, null);

            // círculo con Handles si estás en Editor; con Gizmos haremos aproximación:
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawWireSphere(new Vector3(circleCenter.x, circleCenter.y, 0), circleRadius);

            Gizmos.color = Color.red;
            foreach (var p in _queryResults)
                Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), 0.03f);
        }
    }

    // Utilidad pública: reinserta todos (si quieres probar rebuild en runtime)
    [ContextMenu("Rebuild Now")]
    void RebuildNow() => Rebuild();
}
