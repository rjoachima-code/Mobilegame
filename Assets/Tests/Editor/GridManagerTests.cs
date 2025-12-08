using NUnit.Framework;
using UnityEngine;
using Jacameno;

public class GridManagerTests
{
    [Test]
    public void Merge_TopTwoSameShape_CreatesNextEvolution()
    {
        var go = new GameObject("Grid");
        var gm = go.AddComponent<GridManager>();
        // Setup a simple shape prefab
        var prefab = new GameObject("shapePrefab");
        var sr = prefab.AddComponent<SpriteRenderer>();
        var sc = prefab.AddComponent<ShapeController>();

        gm.GetType().GetField("shapePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gm, prefab);
        gm.GetType().GetField("columnCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gm, 3);

        // Create two ShapeData objects and link next
        var shapeA = ScriptableObject.CreateInstance<ShapeData>();
        var shapeB = ScriptableObject.CreateInstance<ShapeData>();
        shapeA.name = "A";
        shapeB.name = "B";
        // Set private shapeName and nextEvolution via reflection
        var fName = typeof(ShapeData).GetField("shapeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fNext = typeof(ShapeData).GetField("nextEvolution", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fName.SetValue(shapeA, "A");
        fName.SetValue(shapeB, "A");
        fNext.SetValue(shapeA, shapeB);

        // Spawn two shape instances and drop them into column 0
        var go1 = GameObject.Instantiate(prefab);
        var s1 = go1.GetComponent<ShapeController>();
        s1.Initialize(shapeA);

        var go2 = GameObject.Instantiate(prefab);
        var s2 = go2.GetComponent<ShapeController>();
        s2.Initialize(shapeA);

        gm.DropShape(0, s1);
        gm.DropShape(0, s2);

        // Call CheckMerge directly
        gm.CheckMerge(0);

        // After merge, there should be 1 in column 0 and its shape should be shapeB
        var colsField = gm.GetType().GetField("columns", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var cols = (System.Collections.IList)colsField.GetValue(gm);
        var list = (System.Collections.IList)cols[0];
        Assert.AreEqual(1, list.Count);
        var top = list[list.Count - 1] as ShapeController;
        Assert.IsNotNull(top);
        Assert.AreEqual(shapeB, top.Data);

        // Cleanup
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(prefab);
    }
}

