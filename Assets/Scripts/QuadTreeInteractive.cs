using System.Collections.Generic;
using UnityEngine;
using static QuadTreeStructs;

public class QuadTreeInteractive : MonoBehaviour
{
    [Header("Refs")]
    public CircleSpawner2D spawner;     // arrastra el del Hierarchy
    public Transform player;            // arrastra el Player
    public CameraFollow2D camFollow;    // arrastra la cámara con este script

    [Header("Quadtree")]
    [Min(1)] public int capacity = 4;
    [Min(0)] public int maxDepth = 8;

    [Header("Query")]
    public float queryRadius = 2.5f;
    public bool rebuildEachFrame = false; // para demo; si los puntos no se mueven, mantenlo en false

    [Header("Debug")]
    public bool drawTreeGizmos = true;
    public Color gizmoBounds = new(1, 1, 0, 0.75f);
    public Color gizmoPoints = new(0, 1, 1, 0.75f);

    QuadNode _root;
    readonly List<Vector2> _results = new();
    readonly HashSet<Vector2> _resultSet = new(); // para marcar rápido
    readonly QueryStats _stats = new();

    void Start()
    {
        if (camFollow && player) camFollow.target = player;
        RebuildTree();
    }

    void Update()
    {
        if (rebuildEachFrame)
            RebuildTree();

        if (!player || _root == null) return;

        // Query con círculo alrededor del player
        _results.Clear();
        _resultSet.Clear();
        _stats.nodesVisited = _stats.pointsTested = 0;

        QueryCircle(_root, (Vector2)player.position, queryRadius, _results, _stats);
        foreach (var p in _results) _resultSet.Add(p);

        // Marcar visualmente
        MarkResults();
    }

    void RebuildTree()
    {
        if (!spawner)
        {
            Debug.LogWarning("Assign CircleSpawner2D");
            return;
        }
        if (spawner.positions.Count == 0) spawner.SpawnNow();

        // Construir árbol sobre el AABB del spawner
        var aabb = new AABBRect(spawner.origin.x, spawner.origin.y, spawner.size.x, spawner.size.y);
        _root = CreateNode(aabb, capacity, 0, maxDepth);

        for (int i = 0; i < spawner.positions.Count; i++)
            InsertPoint(_root, spawner.positions[i]);
    }

    void MarkResults()
    {
        // Recorre todos los objetos spawneados y aplica highlight si su posición está en el set de resultados
        for (int i = 0; i < spawner.spawned.Count; i++)
        {
            var go = spawner.spawned[i];
            if (!go) continue;

            var cp = go.GetComponent<CirclePoint>();
            if (!cp) continue;

            Vector2 pos = spawner.positions[i]; // misma lista que usamos para construir el árbol
            bool hit = _resultSet.Contains(pos);
            cp.SetHighlighted(hit);
        }
    }

    void OnDrawGizmos()
    {
        if (!drawTreeGizmos || _root == null) return;
        DrawNodeGizmos(_root, gizmoBounds, gizmoPoints);

        if (player)
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawWireSphere(player.position, queryRadius);
        }
    }

    [ContextMenu("Rebuild Tree")]
    void RebuildTreeMenu() => RebuildTree();
}
