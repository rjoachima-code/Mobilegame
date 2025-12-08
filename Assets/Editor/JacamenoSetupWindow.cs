#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Jacameno;

/// <summary>
/// Simple editor window to help spawn shapes, create prefabs and link sample ShapeData.
/// </summary>
public class JacamenoSetupWindow : EditorWindow
{
    private ShapeData sampleShape = null;
    private GameObject shapePrefab = null;
    private GridManager gridManager = null;

    [MenuItem("Jacameno/Setup Window")]
    public static void Open()
    {
        GetWindow<JacamenoSetupWindow>("Jacameno Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Quick Setup Helpers", EditorStyles.boldLabel);
        sampleShape = (ShapeData)EditorGUILayout.ObjectField("Sample ShapeData", sampleShape, typeof(ShapeData), false);
        shapePrefab = (GameObject)EditorGUILayout.ObjectField("Shape Prefab", shapePrefab, typeof(GameObject), false);
        gridManager = (GridManager)EditorGUILayout.ObjectField("GridManager", gridManager, typeof(GridManager), true);

        if (GUILayout.Button("Create Sample Prefab from ShapeData") && sampleShape != null && shapePrefab != null)
        {
            CreatePrefabFromShape(sampleShape, shapePrefab);
        }

        if (GUILayout.Button("Assign Prefab to GridManager") && gridManager != null && shapePrefab != null)
        {
            Undo.RecordObject(gridManager, "Assign shape prefab");
            var so = new SerializedObject(gridManager);
            so.FindProperty("shapePrefab").objectReferenceValue = shapePrefab;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(gridManager);
            Debug.Log("Assigned prefab to GridManager");
        }
    }

    private void CreatePrefabFromShape(ShapeData data, GameObject basePrefab)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        var sc = go.GetComponent<Jacameno.ShapeController>();
        if (sc != null)
        {
            sc.Initialize(data);
        }
        string path = "Assets/Prefabs/Shapes";
        if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder("Assets/Prefabs", "Shapes");
        string prefabPath = $"{path}/{data.ShapeName}_prefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        DestroyImmediate(go);
        AssetDatabase.Refresh();
        Debug.Log("Created prefab at: " + prefabPath);
    }
}
#endif

