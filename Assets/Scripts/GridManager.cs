using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Manages the game grid (10x20), cell states, and collision detection.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        public int GridWidth = 10;
        public int GridHeight = 20;
        public float CellSize = 1f;
        public Transform GridParent;

        // Grid data structure: stores references to blocks at each position
        private Block[,] grid;

        // Visual representation of the grid
        private GameObject[,] gridCells;

        [Header("Visual Settings")]
        public GameObject GridCellPrefab;
        public Color GridLineColor = new Color(0.2f, 0.8f, 1f, 0.3f);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            grid = new Block[GridWidth, GridHeight];
            gridCells = new GameObject[GridWidth, GridHeight];

            if (GridParent == null)
            {
                GridParent = transform;
            }

            CreateGridVisual();
        }

        private void CreateGridVisual()
        {
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    Vector3 position = GridToWorld(x, y);
                    
                    // Create visual cell if prefab exists
                    if (GridCellPrefab != null)
                    {
                        GameObject cell = Instantiate(GridCellPrefab, position, Quaternion.identity, GridParent);
                        cell.name = $"Cell_{x}_{y}";
                        gridCells[x, y] = cell;
                    }
                }
            }
        }

        /// <summary>
        /// Converts grid coordinates to world position.
        /// </summary>
        public Vector3 GridToWorld(int x, int y)
        {
            float offsetX = -(GridWidth * CellSize) / 2f + CellSize / 2f;
            float offsetY = -(GridHeight * CellSize) / 2f + CellSize / 2f;
            return new Vector3(x * CellSize + offsetX, y * CellSize + offsetY, 0);
        }

        /// <summary>
        /// Converts world position to grid coordinates.
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            float offsetX = -(GridWidth * CellSize) / 2f + CellSize / 2f;
            float offsetY = -(GridHeight * CellSize) / 2f + CellSize / 2f;
            
            int x = Mathf.RoundToInt((worldPos.x - offsetX) / CellSize);
            int y = Mathf.RoundToInt((worldPos.y - offsetY) / CellSize);
            
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Checks if a position is within the grid bounds.
        /// </summary>
        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && x < GridWidth && y >= 0 && y < GridHeight;
        }

        /// <summary>
        /// Checks if a position is valid (inside grid and empty).
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return IsInsideGrid(x, y) && grid[x, y] == null;
        }

        /// <summary>
        /// Places a block at the specified grid position.
        /// </summary>
        public bool PlaceBlock(Block block, int x, int y)
        {
            if (!IsValidPosition(x, y))
                return false;

            grid[x, y] = block;
            block.GridPosition = new Vector2Int(x, y);
            block.transform.position = GridToWorld(x, y);
            return true;
        }

        /// <summary>
        /// Removes a block from the specified grid position.
        /// </summary>
        public void RemoveBlock(int x, int y)
        {
            if (IsInsideGrid(x, y))
            {
                grid[x, y] = null;
            }
        }

        /// <summary>
        /// Gets the block at the specified grid position.
        /// </summary>
        public Block GetBlock(int x, int y)
        {
            if (IsInsideGrid(x, y))
                return grid[x, y];
            return null;
        }

        /// <summary>
        /// Checks if a tetromino can move to the specified position.
        /// </summary>
        public bool CanTetrominoMove(Tetromino tetromino, Vector2Int targetPos)
        {
            foreach (Block block in tetromino.Blocks)
            {
                Vector2Int newPos = targetPos + block.LocalPosition;
                
                if (!IsInsideGrid(newPos.x, newPos.y))
                    return false;
                
                if (grid[newPos.x, newPos.y] != null && !tetromino.Blocks.Contains(grid[newPos.x, newPos.y]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a tetromino can rotate.
        /// </summary>
        public bool CanTetrominoRotate(Tetromino tetromino, Vector2Int[] newLocalPositions)
        {
            for (int i = 0; i < tetromino.Blocks.Count; i++)
            {
                Vector2Int newPos = tetromino.GridPosition + newLocalPositions[i];
                
                if (!IsInsideGrid(newPos.x, newPos.y))
                    return false;
                
                Block existingBlock = grid[newPos.x, newPos.y];
                if (existingBlock != null && !tetromino.Blocks.Contains(existingBlock))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Locks a tetromino in place on the grid.
        /// </summary>
        public void LockTetromino(Tetromino tetromino)
        {
            foreach (Block block in tetromino.Blocks)
            {
                Vector2Int gridPos = tetromino.GridPosition + block.LocalPosition;
                if (IsInsideGrid(gridPos.x, gridPos.y))
                {
                    grid[gridPos.x, gridPos.y] = block;
                    block.GridPosition = gridPos;
                    block.transform.SetParent(GridParent);
                    block.IsLocked = true;
                }
            }
        }

        /// <summary>
        /// Checks and clears completed rows, returns number of rows cleared.
        /// </summary>
        public int ClearCompletedRows()
        {
            int rowsCleared = 0;

            for (int y = GridHeight - 1; y >= 0; y--)
            {
                if (IsRowComplete(y))
                {
                    ClearRow(y);
                    MoveRowsDown(y);
                    rowsCleared++;
                    y++; // Check same row again after moving down
                }
            }

            return rowsCleared;
        }

        private bool IsRowComplete(int y)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                if (grid[x, y] == null)
                    return false;
            }
            return true;
        }

        private void ClearRow(int y)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        private void MoveRowsDown(int startRow)
        {
            for (int y = startRow + 1; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (grid[x, y] != null)
                    {
                        Block block = grid[x, y];
                        grid[x, y - 1] = block;
                        grid[x, y] = null;
                        block.GridPosition = new Vector2Int(x, y - 1);
                        block.transform.position = GridToWorld(x, y - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the game is over (blocks at top row).
        /// </summary>
        public bool IsGameOver()
        {
            for (int x = 0; x < GridWidth; x++)
            {
                if (grid[x, GridHeight - 1] != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the entire grid.
        /// </summary>
        public void ClearGrid()
        {
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        Destroy(grid[x, y].gameObject);
                        grid[x, y] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all blocks in a specific row.
        /// </summary>
        public Block[] GetRow(int y)
        {
            Block[] row = new Block[GridWidth];
            for (int x = 0; x < GridWidth; x++)
            {
                row[x] = grid[x, y];
            }
            return row;
        }

        private void OnDrawGizmos()
        {
            // Draw grid outline in editor
            Gizmos.color = GridLineColor;
            
            for (int x = 0; x <= GridWidth; x++)
            {
                Vector3 start = GridToWorld(x, 0) - new Vector3(CellSize / 2f, CellSize / 2f, 0);
                Vector3 end = GridToWorld(x, GridHeight - 1) + new Vector3(-CellSize / 2f, CellSize / 2f, 0);
                Gizmos.DrawLine(start, end);
            }
            
            for (int y = 0; y <= GridHeight; y++)
            {
                Vector3 start = GridToWorld(0, y) - new Vector3(CellSize / 2f, CellSize / 2f, 0);
                Vector3 end = GridToWorld(GridWidth - 1, y) + new Vector3(CellSize / 2f, -CellSize / 2f, 0);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
