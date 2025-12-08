using UnityEngine;

namespace Jacameno
{
    [CreateAssetMenu(menuName = "Jacameno/ShapeData", fileName = "NewShapeData")]
    public class ShapeData : ScriptableObject
    {
        [SerializeField] private string shapeName = "Shape";
        [SerializeField] private int vertexCount = 3;
        [SerializeField] private Sprite icon = null;
        [SerializeField] private Color neonColor = Color.cyan;
        [SerializeField] private ShapeData nextEvolution = null;

        // Public accessors (read-only in inspector by SerializeField)
        public string ShapeName => shapeName;
        public int VertexCount => vertexCount;
        public Sprite Icon => icon;
        public Color NeonColor => neonColor;
        public ShapeData NextEvolution => nextEvolution;

        /// <summary>
        /// Returns the score value for this shape. Default formula: vertexCount * 10
        /// </summary>
        public int GetScoreValue()
        {
            return Mathf.Max(0, vertexCount) * 10;
        }
    }
}

