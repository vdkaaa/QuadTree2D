<div align="center">

  <h1 style="font-size:2.8rem; margin-bottom:0.2rem;">🌳 QuadTree2D</h1>

  <p style="font-size:1.1rem; max-width:680px;">
    Demo interactiva de <strong>Quadtree 2D</strong> donde un jugador se mueve dentro de una nube de puntos
    y consulta sólo los puntos cercanos usando una búsqueda espacial eficiente.
  </p>

  <p>
    <a href="https://github.com/vdkaaa/QuadTree2D">
      <img src="https://img.shields.io/badge/repo-QuadTree2D-171717?logo=github" alt="Repo badge">
    </a>
    <img src="https://img.shields.io/badge/engine-Unity-000000?logo=unity" alt="Unity badge">
    <img src="https://img.shields.io/badge/language-C%23-239120?logo=csharp" alt="C# badge">
  </p>

  <!-- Cambia la ruta por un GIF/imagen real de tu demo -->
  <p>
    <img src="Assets/Readme/quadtree-demo.gif" alt="QuadTree2D demo" width="640">
  </p>

</div>

---

## 🧩 Resumen

Este repo es un pequeño laboratorio para jugar con un **Quadtree en 2D**:

- Se generan muchos puntos en un área usando `CircleSpawner2D`.
- A partir de esas posiciones se construye un **Quadtree estático**.
- Un jugador (`Transform player`) se mueve por el espacio.
- Cada frame se ejecuta una **búsqueda en círculo** alrededor del jugador (`queryRadius`)
  usando el quadtree en vez de revisar todos los puntos.
- Los puntos que caen dentro del área de búsqueda se destacan visualmente.

La idea principal es mostrar, de forma visual e interactiva, cómo un quadtree puede
ayudar a optimizar consultas espaciales en un proyecto 2D.

---

## 🧠 Scripts principales

<div align="center">

<table>
  <thead>
    <tr>
      <th style="text-align:left;">Script</th>
      <th style="text-align:left;">Rol en el proyecto</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>QuadTreeInteractive</code></td>
      <td>
        Componente principal de la demo.<br/>
        Construye el quadtree a partir del <code>CircleSpawner2D</code>, ejecuta la búsqueda circular
        alrededor del jugador y dibuja gizmos del árbol.
      </td>
    </tr>
    <tr>
      <td><code>QuadTreeStructs</code></td>
      <td>
        Contiene las estructuras y funciones del quadtree:<br/>
        <code>QuadNode</code>, <code>AABBRect</code>, <code>CreateNode</code>, <code>InsertPoint</code>,
        <code>QueryCircle</code>, <code>DrawNodeGizmos</code>, <code>QueryStats</code>, etc.
      </td>
    </tr>
    <tr>
      <td><code>CircleSpawner2D</code></td>
      <td>
        Genera una nube de puntos en 2D:<br/>
        mantiene una lista de <code>positions</code> y de <code>spawned</code> GameObjects,
        usada luego para construir el quadtree y para resaltar resultados.
      </td>
    </tr>
    <tr>
      <td><code>CirclePoint</code></td>
      <td>
        Script en cada punto instanciado.<br/>
        Expone <code>SetHighlighted(bool)</code> para encender/apagar el highlight
        según si el punto aparece en la última query.
      </td>
    </tr>
    <tr>
      <td><code>CameraFollow2D</code></td>
      <td>
        Hace que la cámara siga al <code>player</code> para que puedas moverte por la nube de puntos
        y ver cómo cambia la búsqueda del quadtree.
      </td>
    </tr>
  </tbody>
</table>

</div>

---

### Componente central: `QuadTreeInteractive`

```csharp
public class QuadTreeInteractive : MonoBehaviour
{
    [Header("Refs")]
    public CircleSpawner2D spawner;
    public Transform player;
    public CameraFollow2D camFollow;

    [Header("Quadtree")]
    [Min(1)] public int capacity = 4;
    [Min(0)] public int maxDepth = 8;

    [Header("Query")]
    public float queryRadius = 2.5f;
    public bool rebuildEachFrame = false;

    [Header("Debug")]
    public bool drawTreeGizmos = true;
    public Color gizmoBounds = new(1, 1, 0, 0.75f);
    public Color gizmoPoints = new(0, 1, 1, 0.75f);

    QuadNode _root;
    readonly List<Vector2> _results = new();
    readonly HashSet<Vector2> _resultSet = new();
    readonly QueryStats _stats = new();
}
