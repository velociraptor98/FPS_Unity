// Assets/Editor/LevelBuilder.cs
// Run via: Unity menu → FPS → Build Level
// Clears the old environment and rebuilds a full cross-shaped arena using
// the Kenney Scifi Pack prefabs that already live in the project.

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelBuilder
{
    // ── Prefab roots ──────────────────────────────────────────────────────
    private const string KENNEY = "Assets/Udemy FPS Assets/Models/Environment/Kenney Scifi Pack/";
    private const string GAME   = "Assets/Prefab/";

    // ── Grid constants ────────────────────────────────────────────────────
    // All Kenney tiles have a 2-unit footprint in XZ; pivot at corner (0,0).
    // Every grid coordinate below is a tile ORIGIN (bottom-left corner).
    private const float T = 2f;

    // ─────────────────────────────────────────────────────────────────────
    [MenuItem("FPS/Build Level")]
    public static void BuildLevel()
    {
        // Wipe the old scene objects we own
        Obliterate("Environment");
        Obliterate("Target");
        Obliterate("Enemies");
        Obliterate("Checkpoints");

        // ── Scene hierarchy ───────────────────────────────────────────────
        Transform env   = NewGO("Environment");
        Transform floor = NewChild(env, "Floor");
        Transform walls = NewChild(env, "Walls");
        Transform decor = NewChild(env, "Decor");
        Transform foes  = NewGO("Enemies");
        Transform cps   = NewGO("Checkpoints");

        // ── FLOORS ───────────────────────────────────────────────────────
        // Cross-shaped map. All tile origins at y = 0.
        // Legend: (xMin, xMax, zMin, zMax)  →  (xMax-xMin)/T × (zMax-zMin)/T tiles
        // Core arena  16×16
        Grid(floor, "metalTile",     -8,  8, -8,  8);
        // North corridor  8×8
        Grid(floor, "metalTile",     -4,  4,  8, 16);
        // South corridor  8×8
        Grid(floor, "metalTile",     -4,  4,-16, -8);
        // East corridor   8×8
        Grid(floor, "metalTile",      8, 16, -4,  4);
        // West corridor   8×8
        Grid(floor, "metalTile",    -16, -8, -4,  4);
        // North room  12×12
        Grid(floor, "groundTile",    -6,  6, 16, 28);
        // South room  12×12  (player spawn area)
        Grid(floor, "groundTile",    -6,  6,-28,-16);
        // East room  12×12
        Grid(floor, "groundTile",    16, 28, -6,  6);
        // West room  12×12
        Grid(floor, "groundTile",   -28,-16, -6,  6);

        // Raised centre platform (2×2 tiles at height 2)
        Grid(floor, "metalTileLarge", -2,  2, -2,  2, 2f);

        // ── WALLS ────────────────────────────────────────────────────────
        BuildWalls(walls);

        // ── COVER (centre arena) ─────────────────────────────────────────
        K(decor, "barrel",           new(-6, 0,  2));
        K(decor, "barrel",           new(-6, 0, -4));
        K(decor, "barrel",           new( 4, 0,  2));
        K(decor, "barrel",           new( 4, 0, -4));
        K(decor, "barrelLarge",      new(-4, 0,  0));
        K(decor, "barrelLarge",      new( 2, 0, -6));
        K(decor, "metalStructure",   new( 6, 0,  0));
        K(decor, "metalStructure",   new(-8, 0, -6));
        K(decor, "rocks",            new( 4, 0, -6));
        K(decor, "rocksSmall",       new(-2, 0, -2));
        K(decor, "rocksSmall",       new( 0, 0,  6));
        K(decor, "metalStructureCross", new(0, 0, -4));

        // ── DECOR (rooms) ─────────────────────────────────────────────────
        // North room
        K(decor, "satelliteDish",       new( 0, 0, 22),  45f);
        K(decor, "satelliteDishLarge",  new(-4, 0, 20), 135f);
        K(decor, "portal",              new( 0, 0, 25),   0f);
        K(decor, "pipeStand",           new( 3, 0, 18));
        K(decor, "pipeStand",           new(-3, 0, 18));
        // South room
        K(decor, "console",             new(-2, 0,-22), 180f);
        K(decor, "consoleScreen",       new( 2, 0,-22), 180f);
        K(decor, "consoleCorner",       new(-4, 0,-20), 270f);
        // East room
        K(decor, "crater",              new(22, 0,  0));
        K(decor, "craterLarge",         new(20, 0, -2),  30f);
        K(decor, "spaceCraftStand",     new(24, 0,  2));
        K(decor, "meteorFull",          new(26, 0,  3),  60f);
        // West room
        K(decor, "meteorFullRound",     new(-22, 0,  0));
        K(decor, "meteorHalf",          new(-20, 0, -2),  45f);
        K(decor, "station",             new(-24, 0,  2),  90f);
        K(decor, "rocks",               new(-26, 0, -2), 120f);

        // ── ENEMIES ──────────────────────────────────────────────────────
        // North room — 3 enemies
        G(foes, "Enemy", new(-3, 1, 20));
        G(foes, "Enemy", new( 1, 1, 22));
        G(foes, "Enemy", new(-1, 1, 24));
        // East room — 2 enemies
        G(foes, "Enemy", new(20, 1,  2));
        G(foes, "Enemy", new(22, 1, -2));
        // West room — 2 enemies
        G(foes, "Enemy", new(-22, 1,  2));
        G(foes, "Enemy", new(-20, 1, -2));
        // Arena — 2 roaming enemies
        G(foes, "Enemy", new(-6, 1,  4));
        G(foes, "Enemy", new( 4, 1, -4));

        // ── TURRETS ──────────────────────────────────────────────────────
        G(foes, "Turret", new(-2,  0, 20), 180f);   // N room, facing south
        G(foes, "Turret", new( 0,  2,  0), 180f);   // elevated centre platform
        G(foes, "Turret", new(22,  0,  0), 270f);   // E room, facing west

        // ── BOUNCE PADS ──────────────────────────────────────────────────
        BouncePad(decor, new(-8, 0,  8), 14f);
        BouncePad(decor, new( 6, 0,  8), 14f);
        BouncePad(decor, new( 0, 0, -8), 10f);

        // ── CHECKPOINTS ──────────────────────────────────────────────────
        Checkpoint(cps, new(0, 1,-16), "cp_south");
        Checkpoint(cps, new(0, 1, 16), "cp_north");

        // ── PLAYER SPAWN ─────────────────────────────────────────────────
        Obliterate("PlayerSpawnPoint");
        CreateSpawnPoint(new Vector3(0, 1, -22));

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[LevelBuilder] Level built. Press Ctrl+S to save.");
    }

    // ─────────────────────────────────────────────────────────────────────
    // WALLS
    // frameHighTile default facing = +Z.  Rotations used here:
    //   0°  → face +Z (north)   180° → face -Z (south)
    //   90° → face +X (east)    270° → face -X (west)
    // Walls are placed one T step outside the floor tile grid so they sit
    // flush with the tile's outer edge.
    // ─────────────────────────────────────────────────────────────────────
    static void BuildWalls(Transform p)
    {
        // ── Core arena perimeter ─────────────────────────────────────────
        // North edge (z=8): solid except corridor gap x[-4,4]
        WallRow(p, "frameHighTile", -8, -4,  8, 180f);
        WallRow(p, "frameHighTile",  4,  8,  8, 180f);
        // South edge (z=-8)
        WallRow(p, "frameHighTile", -8, -4, -8,   0f);
        WallRow(p, "frameHighTile",  4,  8, -8,   0f);
        // East edge (x=8): gap z[-4,4]
        WallCol(p, "frameHighTile",  8, -8, -4, 270f);
        WallCol(p, "frameHighTile",  8,  4,  8, 270f);
        // West edge (x=-8)
        WallCol(p, "frameHighTile", -8, -8, -4,  90f);
        WallCol(p, "frameHighTile", -8,  4,  8,  90f);

        // ── North corridor sides ──────────────────────────────────────────
        WallCol(p, "frameHighTile",  4,  8, 16, 270f);   // east side
        WallCol(p, "frameHighTile", -4,  8, 16,  90f);   // west side

        // ── South corridor sides ──────────────────────────────────────────
        WallCol(p, "frameHighTile",  4,-16, -8, 270f);
        WallCol(p, "frameHighTile", -4,-16, -8,  90f);

        // ── East corridor sides ───────────────────────────────────────────
        WallRow(p, "frameHighTile",  8, 16,  4, 180f);   // north side
        WallRow(p, "frameHighTile",  8, 16, -4,   0f);   // south side

        // ── West corridor sides ───────────────────────────────────────────
        WallRow(p, "frameHighTile",-16, -8,  4, 180f);
        WallRow(p, "frameHighTile",-16, -8, -4,   0f);

        // ── North room ────────────────────────────────────────────────────
        WallRow(p, "frameHigh",    -6,  6, 28, 180f);   // north wall
        WallCol(p, "frameHigh",     6, 16, 28, 270f);   // east wall
        WallCol(p, "frameHigh",    -6, 16, 28,  90f);   // west wall
        // South partial (gap for corridor: x[-4,4])
        Spawn(LoadKenney("frameHigh"), p, new(-6, 0, 16), 0f);
        Spawn(LoadKenney("frameHigh"), p, new( 4, 0, 16), 0f);

        // ── South room ────────────────────────────────────────────────────
        WallRow(p, "frameHigh",    -6,  6,-28,   0f);   // south wall
        WallCol(p, "frameHigh",     6,-28,-16, 270f);   // east wall
        WallCol(p, "frameHigh",    -6,-28,-16,  90f);   // west wall
        // North partial
        Spawn(LoadKenney("frameHigh"), p, new(-6, 0,-16), 180f);
        Spawn(LoadKenney("frameHigh"), p, new( 4, 0,-16), 180f);

        // ── East room ─────────────────────────────────────────────────────
        WallCol(p, "frameHigh",    28, -6,  6, 270f);   // east wall
        WallRow(p, "frameHigh",    16, 28,  6, 180f);   // north wall
        WallRow(p, "frameHigh",    16, 28, -6,   0f);   // south wall
        // West partial
        Spawn(LoadKenney("frameHigh"), p, new(16, 0,-6), 90f);
        Spawn(LoadKenney("frameHigh"), p, new(16, 0, 4), 90f);

        // ── West room ─────────────────────────────────────────────────────
        WallCol(p, "frameHigh",   -28, -6,  6,  90f);   // west wall
        WallRow(p, "frameHigh",   -28,-16,  6, 180f);   // north wall
        WallRow(p, "frameHigh",   -28,-16, -6,   0f);   // south wall
        // East partial
        Spawn(LoadKenney("frameHigh"), p, new(-16, 0,-6), 270f);
        Spawn(LoadKenney("frameHigh"), p, new(-16, 0, 4), 270f);
    }

    // ─────────────────────────────────────────────────────────────────────
    // PRIMITIVES
    // ─────────────────────────────────────────────────────────────────────

    // Fill floor tiles: loop x [xMin, xMax) step T, z [zMin, zMax)
    static void Grid(Transform parent, string tileName,
                     float xMin, float xMax, float zMin, float zMax, float y = 0f)
    {
        var prefab = LoadKenney(tileName);
        if (prefab == null) return;
        for (float x = xMin; x < xMax; x += T)
        for (float z = zMin; z < zMax; z += T)
            Spawn(prefab, parent, new Vector3(x, y, z), 0f);
    }

    // Horizontal wall line along X axis
    static void WallRow(Transform parent, string pName,
                        float xMin, float xMax, float z, float yRot)
    {
        var prefab = LoadKenney(pName);
        if (prefab == null) return;
        for (float x = xMin; x < xMax; x += T)
            Spawn(prefab, parent, new Vector3(x, 0f, z), yRot);
    }

    // Vertical wall line along Z axis
    static void WallCol(Transform parent, string pName,
                        float x, float zMin, float zMax, float yRot)
    {
        var prefab = LoadKenney(pName);
        if (prefab == null) return;
        for (float z = zMin; z < zMax; z += T)
            Spawn(prefab, parent, new Vector3(x, 0f, z), yRot);
    }

    static void BouncePad(Transform parent, Vector3 pos, float force)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "BouncePad";
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(2f, 0.2f, 2f);

        var mr  = go.GetComponent<MeshRenderer>();
        var mat = new Material(Shader.Find("Standard")) { color = new Color(1f, 0.8f, 0f) };
        mr.sharedMaterial = mat;

        var col = go.GetComponent<BoxCollider>();
        col.isTrigger = true;

        var b  = go.AddComponent<Bounce>();
        b.bounceForce = force;

        Undo.RegisterCreatedObjectUndo(go, "BouncePad");
    }

    static void Checkpoint(Transform parent, Vector3 pos, string cpName)
    {
        var go = new GameObject($"Checkpoint_{cpName}");
        go.transform.SetParent(parent);
        go.transform.position = pos;

        var col  = go.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size      = new Vector3(8f, 3f, 2f);

        var cp = go.AddComponent<CheckPoint>();
        var so = new SerializedObject(cp);
        so.FindProperty("cpname").stringValue = cpName;
        so.ApplyModifiedPropertiesWithoutUndo();

        Undo.RegisterCreatedObjectUndo(go, "Checkpoint");
    }

    static void CreateSpawnPoint(Vector3 pos)
    {
        var go = new GameObject("PlayerSpawnPoint");
        go.transform.position = pos;
        go.AddComponent<PlayerSpawnPoint>();
        Undo.RegisterCreatedObjectUndo(go, "PlayerSpawnPoint");
    }

    // ─────────────────────────────────────────────────────────────────────
    // INSTANTIATION HELPERS
    // ─────────────────────────────────────────────────────────────────────

    static void K(Transform parent, string name, Vector3 pos, float yRot = 0f)
    {
        var prefab = LoadKenney(name);
        if (prefab == null) return;
        Spawn(prefab, parent, pos, yRot);
    }

    static void G(Transform parent, string name, Vector3 pos, float yRot = 0f)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GAME + name + ".prefab");
        if (prefab == null) { Debug.LogWarning($"[LevelBuilder] Missing game prefab: {name}"); return; }
        Spawn(prefab, parent, pos, yRot);
    }

    static void Spawn(GameObject prefab, Transform parent, Vector3 pos, float yRot = 0f)
    {
        if (prefab == null) return;
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
        Undo.RegisterCreatedObjectUndo(go, "LevelBuilder Spawn");
    }

    static GameObject LoadKenney(string name)
    {
        string path = KENNEY + name + ".prefab";
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) Debug.LogWarning($"[LevelBuilder] Cannot load: {path}");
        return go;
    }

    // ─────────────────────────────────────────────────────────────────────
    // SCENE HELPERS
    // ─────────────────────────────────────────────────────────────────────

    static void Obliterate(string goName)
    {
        var go = GameObject.Find(goName);
        if (go != null) Undo.DestroyObjectImmediate(go);
    }

    static Transform NewGO(string name)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, name);
        return go.transform;
    }

    static Transform NewChild(Transform parent, string name)
    {
        var t = NewGO(name);
        t.SetParent(parent);
        return t;
    }
}
#endif
