#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;

namespace Jacameno.Editor
{
    public class JacamenoAutoSetup : EditorWindow
    {
        [MenuItem("Jacameno/Auto Setup")]
        public static void Open()
        {
            GetWindow<JacamenoAutoSetup>("Jacameno Auto Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Auto Setup for Jacameno", EditorStyles.boldLabel);
            if (GUILayout.Button("Create Sample Assets & Scene"))
            {
                CreateSampleAssetsAndScene();
            }
            if (GUILayout.Button("Try Add DOTween to manifest.json and enable define"))
            {
                TryAddDOTweenAndDefine();
            }
        }

        private static void CreateSampleAssetsAndScene()
        {
            // Ensure folders
            Directory.CreateDirectory("Assets/Data/ShapeData");
            Directory.CreateDirectory("Assets/Prefabs/Shapes");

            // Create ShapeData assets: Triangle -> Square -> Pentagon
            var tri = ScriptableObject.CreateInstance<Jacameno.ShapeData>();
            var sq = ScriptableObject.CreateInstance<Jacameno.ShapeData>();
            var pent = ScriptableObject.CreateInstance<Jacameno.ShapeData>();

            // Use reflection to set private serialized fields
            var fn = typeof(Jacameno.ShapeData).GetField("shapeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fv = typeof(Jacameno.ShapeData).GetField("vertexCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fnex = typeof(Jacameno.ShapeData).GetField("nextEvolution", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fcolor = typeof(Jacameno.ShapeData).GetField("neonColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            fn.SetValue(tri, "Triangle"); fv.SetValue(tri, 3); fcolor.SetValue(tri, Color.cyan);
            fn.SetValue(sq, "Square"); fv.SetValue(sq, 4); fcolor.SetValue(sq, Color.magenta);
            fn.SetValue(pent, "Pentagon"); fv.SetValue(pent, 5); fcolor.SetValue(pent, Color.yellow);

            fnex.SetValue(tri, sq);
            fnex.SetValue(sq, pent);
            fnex.SetValue(pent, null);

            AssetDatabase.CreateAsset(tri, "Assets/Data/ShapeData/Triangle.asset");
            AssetDatabase.CreateAsset(sq, "Assets/Data/ShapeData/Square.asset");
            AssetDatabase.CreateAsset(pent, "Assets/Data/ShapeData/Pentagon.asset");
            AssetDatabase.SaveAssets();

            // Create a basic shape prefab
            GameObject shapeGO = new GameObject("Shape_Prefab");
            var sr = shapeGO.AddComponent<SpriteRenderer>();
            var sc = shapeGO.AddComponent<Jacameno.ShapeController>();
            // Optionally assign sr sprite later in Editor

            string prefabPath = "Assets/Prefabs/Shapes/Shape_Prefab.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(shapeGO, prefabPath);
            Object.DestroyImmediate(shapeGO);

            // Create a basic scene objects: GridManager and InputController
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var gmGO = new GameObject("GridManager");
            var gm = gmGO.AddComponent<Jacameno.GridManager>();
            // assign prefab reference
            var so = new SerializedObject(gm);
            so.FindProperty("shapePrefab").objectReferenceValue = prefab;
            so.ApplyModifiedProperties();

            var imGO = new GameObject("InputController");
            var ic = imGO.AddComponent<Jacameno.InputController>();
            var sic = new SerializedObject(ic);
            sic.FindProperty("activeShapePrefab").objectReferenceValue = prefab;
            sic.FindProperty("gridManager").objectReferenceValue = gm;
            sic.ApplyModifiedProperties();

            // Add MergeMechanic
            var mmGO = new GameObject("MergeMechanic");
            var mm = mmGO.AddComponent<Jacameno.MergeMechanic>();
            var somm = new SerializedObject(gm);
            somm.FindProperty("mergeMechanic").objectReferenceValue = mm;
            somm.ApplyModifiedProperties();

            // Save scene
            string scenePath = "Assets/Scenes/JacamenoSampleScene.unity";
            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);

            AssetDatabase.Refresh();
            Debug.Log("Created sample ShapeData assets, prefab, and sample scene at: " + scenePath);
        }

        private static void TryAddDOTweenAndDefine()
        {
            // Try edit Packages/manifest.json
            string manifestPath = "Packages/manifest.json";
            if (!File.Exists(manifestPath))
            {
                Debug.LogWarning("manifest.json not found in Packages folder. Please open the project in Unity first.");
                return;
            }

            string manifest = File.ReadAllText(manifestPath);
            if (manifest.Contains("com.demigiant.dotween") || manifest.Contains("Demigiant"))
            {
                Debug.Log("DOTween entry appears to already be present in manifest.json");
            }
            else
            {
                // Simple insertion: add a line in dependencies block. This is best-effort.
                string insert = "\n    \"com.demigiant.dotween\": \"https://github.com/Demigiant/DOTween.git\"\n";
                if (manifest.Contains("\"dependencies\": {"))
                {
                    manifest = manifest.Replace("\"dependencies\": {", "\"dependencies\": {" + insert);
                    File.WriteAllText(manifestPath, manifest);
                    Debug.Log("Inserted DOTween entry into manifest.json (best-effort). You may need to resolve via Package Manager.");
                }
                else
                {
                    Debug.LogWarning("Couldn't find dependencies block in manifest.json to insert DOTween. Please add DOTween via Package Manager manually.");
                }
            }

            // Add scripting define symbols for common build targets
            var targets = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
            foreach (var t in targets)
            {
                string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(t);
                if (!current.Contains("JACAMENO_USE_DOTWEEN"))
                {
                    string next = string.IsNullOrEmpty(current) ? "JACAMENO_USE_DOTWEEN" : current + ";JACAMENO_USE_DOTWEEN";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(t, next);
                    Debug.Log($"Added JACAMENO_USE_DOTWEEN to defines for {t}");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
#endif

