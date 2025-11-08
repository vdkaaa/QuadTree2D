using System.Collections.Generic;
using UnityEngine;

public class CircleSpawner2D : MonoBehaviour
{
    [Header("Spawn Area (XY)")]
    public Vector2 origin = new(-8, -5);
    public Vector2 size = new(16, 10);

    [Header("Spawn")]
    public GameObject circlePrefab;
    [Min(0)] public int count = 300;
    public int randomSeed = 12345;

    public readonly List<GameObject> spawned = new();
    public readonly List<Vector2> positions = new();

    public void ClearAll()
    {
        foreach (var go in spawned) if (go) DestroyImmediate(go);
        spawned.Clear();
        positions.Clear();
    }

    [ContextMenu("Spawn Now")]
    public void SpawnNow()
    {
        ClearAll();
        if (!circlePrefab) { Debug.LogWarning("Assign circlePrefab"); return; }

        var rnd = new System.Random(randomSeed);
        for (int i = 0; i < count; i++)
        {
            float x = (float)(origin.x + rnd.NextDouble() * size.x);
            float y = (float)(origin.y + rnd.NextDouble() * size.y);
            Vector2 pos = new(x, y);

            var go = Instantiate(circlePrefab, pos, Quaternion.identity, transform);
            spawned.Add(go);
            positions.Add(pos);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.35f);
        var c = (Vector3)(origin + size * 0.5f);
        var s = (Vector3)(new Vector2(size.x, size.y));
        Gizmos.DrawWireCube(c, s);
    }
}
