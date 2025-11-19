<div align="center">

  <h1>🌳 QuadTree2D</h1>

  <p><strong>Demo de Quadtree espacial en 2D</strong><br/>
  usando un jugador que se mueve y consulta sólo los nodos cercanos.</p>

  <p>
    <a href="https://github.com/vdkaaa/QuadTree2D/tree/main">
      <img src="https://img.shields.io/badge/repo-QuadTree2D-171717?logo=github" alt="Repo badge">
    </a>
    <img src="https://img.shields.io/badge/engine-Unity-000000?logo=unity" alt="Unity badge">
    <img src="https://img.shields.io/badge/language-C%23-239120?logo=csharp" alt="C# badge">
  </p>

  <!-- Si subes un GIF o imagen de la demo, cambia la ruta de abajo -->
  <!-- Ejemplo: guarda tu gif en Assets/Readme/quadtree-demo.gif y referencia la ruta relativa -->
  <p>
    <img src="Assets/Readme/quadtree-demo.gif" alt="QuadTree2D demo" width="600">
  </p>

</div>

---

## 🧩 Sobre el proyecto

Este repositorio es un pequeño laboratorio para jugar con un **Quadtree en 2D**, donde:

- Un jugador puede moverse libremente por el mapa.
- Los objetos de la escena se registran en un quadtree.
- El jugador consulta sólo los **nodos cercanos**, en vez de iterar sobre todos los objetos. :contentReference[oaicite:0]{index=0}  

Además del quadtree, hay otros scripts y escenas que usé para probar ideas en 2D, pero el foco principal es mostrar **cómo usar la estructura de quadtree en un proyecto 2D**.

---

## 🧠 ¿Qué es un Quadtree (versión corta)?

Un **Quadtree** es una estructura de datos que divide el espacio 2D en 4 subregiones (cuadrantes) de forma recursiva:

- Cada nodo representa un rectángulo del mundo.
- Cuando un nodo tiene demasiados elementos, se subdivide en 4 hijos.
- Las búsquedas de “objetos cercanos” se hacen sólo en los nodos relevantes,
  evitando recorrer todos los elementos.

Esto es muy útil para:

- Detección de colisiones.
- Búsqueda de vecinos.
- Culling / optimización de rendimiento en juegos 2D.

---

## 🎮 Cómo se usa el Quadtree en este proyecto

> 👉 Esta sección es la que más te sirve para LinkedIn y para que otros devs entiendan cómo integrarlo.

### 1. Estructura básica

Normalmente tendrás algo así (cambia los nombres por los de tus scripts reales):

- `QuadTree2D` / `QuadTreeManager`  
  Componente que mantiene la instancia principal del quadtree (root) y define:
  - El tamaño del mundo 2D.
  - Capacidad máxima por nodo.
  - Profundidad máxima.

- `QuadTreeObject` (o similar)  
  Script que agregas a los objetos que quieres que vivan dentro del quadtree.
  Suele encargarse de:
  - Registrarse en el quadtree al iniciar.
  - Actualizar su posición en el quadtree cuando se mueve.
  - Eliminarse del quadtree cuando se destruye.

- `PlayerController`  
  Controla el movimiento del jugador y hace las **consultas de vecinos** usando el quadtree.

- (Opcional) `QuadTreeDebugDrawer`  
  Dibuja los límites de los nodos del quadtree en pantalla para debug/visualización.

### 2. Flujo de uso en la escena

1. **Crear el mundo / manager**
   - Arrastras un `GameObject` vacío en la escena (por ejemplo `QuadTreeRoot`).
   - Le añades el script `QuadTreeManager`.
   - Configuras:
     - **World Bounds** (ancho/alto del área que cubre el quadtree).
     - **Capacidad por nodo** (cuántos objetos antes de subdividir).
     - **Profundidad máxima** (para evitar subdividir infinito).

2. **Registrar objetos en el quadtree**
   - A cualquier entidad que quieras que participe en consultas espaciales le pones un script tipo `QuadTreeObject`.
   - Ese script se encarga de decirle al manager:
     ```csharp
     void OnEnable()
     {
         QuadTreeManager.Instance.Register(this);
     }

     void OnDisable()
     {
         QuadTreeManager.Instance.Unregister(this);
     }
     ```
   - Internamente, el manager llama algo como:
     ```csharp
     quadTree.Insert(this.Bounds, this);
     ```

3. **Consultar vecinos alrededor del jugador**

   En el `PlayerController` (o en un sistema aparte) puedes hacer algo del estilo:

   ```csharp
   // Pseudocódigo – adapta los nombres a tus clases reales
   var searchArea = new Rect(
       playerPosition.x - visionRadius,
       playerPosition.y - visionRadius,
       visionRadius * 2f,
       visionRadius * 2f
   );

   var nearbyObjects = quadTree.QueryRange(searchArea);

   foreach (var obj in nearbyObjects)
   {
       // Aquí puedes:
       // - Dibujar un gizmo
       // - Hacer lógica de colisión
       // - Mostrar info visual, etc.
   }
