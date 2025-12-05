using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Spawns random tetromino shapes at the top of the grid.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        public static Spawner Instance { get; private set; }

        [Header("Spawner Settings")]
        public Block BlockPrefab;
        public Transform TetrominoParent;
        public Vector2Int SpawnPosition = new Vector2Int(4, 18);

        [Header("Preview")]
        public Transform PreviewParent;
        public Vector3 PreviewOffset = new Vector3(6f, 8f, 0f);

        private TetrominoType nextTetrominoType;
        private Tetromino currentTetromino;
        private GameObject previewObject;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PrepareNextTetromino();
        }

        /// <summary>
        /// Spawns a new tetromino at the spawn position.
        /// </summary>
        public Tetromino SpawnTetromino()
        {
            if (currentTetromino != null)
            {
                Debug.LogWarning("Current tetromino still exists. Lock it first!");
                return null;
            }

            // Use the prepared next type
            TetrominoType typeToSpawn = nextTetrominoType;
            
            // Prepare next tetromino
            PrepareNextTetromino();

            // Create new tetromino
            GameObject tetrominoObj = new GameObject($"Tetromino_{typeToSpawn}");
            
            if (TetrominoParent != null)
            {
                tetrominoObj.transform.SetParent(TetrominoParent);
            }

            Tetromino tetromino = tetrominoObj.AddComponent<Tetromino>();
            tetromino.Initialize(typeToSpawn, SpawnPosition, BlockPrefab);

            currentTetromino = tetromino;

            // Check if spawn position is valid (game over check)
            if (!GridManager.Instance.CanTetrominoMove(tetromino, SpawnPosition))
            {
                GameState.Instance.SetGameOver();
                return null;
            }

            return tetromino;
        }

        /// <summary>
        /// Prepares the next tetromino type and updates preview.
        /// </summary>
        private void PrepareNextTetromino()
        {
            TetrominoType[] types = (TetrominoType[])System.Enum.GetValues(typeof(TetrominoType));
            nextTetrominoType = types[Random.Range(0, types.Length)];

            UpdatePreview();
        }

        /// <summary>
        /// Updates the preview display for the next tetromino.
        /// </summary>
        private void UpdatePreview()
        {
            // Clear old preview
            if (previewObject != null)
            {
                Destroy(previewObject);
            }

            if (PreviewParent == null)
                return;

            previewObject = new GameObject("Preview");
            previewObject.transform.SetParent(PreviewParent);
            previewObject.transform.localPosition = PreviewOffset;

            // Create preview blocks
            Vector2Int[] positions = GetTetrominoPositions(nextTetrominoType);
            
            foreach (Vector2Int pos in positions)
            {
                if (BlockPrefab != null)
                {
                    Block previewBlock = Instantiate(BlockPrefab, previewObject.transform);
                    previewBlock.transform.localPosition = new Vector3(pos.x * 0.5f, pos.y * 0.5f, 0);
                    previewBlock.transform.localScale = Vector3.one * 0.5f;
                    previewBlock.SetValue(2);
                    
                    // Make preview slightly transparent
                    if (previewBlock.SpriteRenderer != null)
                    {
                        Color c = previewBlock.SpriteRenderer.color;
                        c.a = 0.7f;
                        previewBlock.SpriteRenderer.color = c;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the initial block positions for a tetromino type.
        /// </summary>
        private Vector2Int[] GetTetrominoPositions(TetrominoType type)
        {
            switch (type)
            {
                case TetrominoType.I:
                    return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
                case TetrominoType.O:
                    return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
                case TetrominoType.T:
                    return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                case TetrominoType.L:
                    return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
                case TetrominoType.J:
                    return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1) };
                case TetrominoType.S:
                    return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) };
                case TetrominoType.Z:
                    return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
                default:
                    return new[] { new Vector2Int(0, 0) };
            }
        }

        /// <summary>
        /// Gets the current falling tetromino.
        /// </summary>
        public Tetromino GetCurrentTetromino()
        {
            return currentTetromino;
        }

        /// <summary>
        /// Clears the current tetromino reference (called after locking).
        /// </summary>
        public void ClearCurrentTetromino()
        {
            currentTetromino = null;
        }

        /// <summary>
        /// Gets the next tetromino type for preview.
        /// </summary>
        public TetrominoType GetNextTetrominoType()
        {
            return nextTetrominoType;
        }
    }
}
