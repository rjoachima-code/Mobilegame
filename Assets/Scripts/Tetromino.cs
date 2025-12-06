using System.Collections.Generic;
using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Represents a falling tetromino shape composed of blocks.
    /// </summary>
    public class Tetromino : MonoBehaviour
    {
        [Header("Tetromino Properties")]
        public TetrominoType Type;
        public Vector2Int GridPosition;
        public List<Block> Blocks = new List<Block>();

        private int rotationState = 0;
        
        // Rotation offsets for each tetromino type (4 rotation states)
        private static readonly Dictionary<TetrominoType, Vector2Int[,]> RotationData = new Dictionary<TetrominoType, Vector2Int[,]>
        {
            // I piece
            { TetrominoType.I, new Vector2Int[,] {
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            }},
            
            // O piece (no rotation needed)
            { TetrominoType.O, new Vector2Int[,] {
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) }
            }},
            
            // T piece
            { TetrominoType.T, new Vector2Int[,] {
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) },
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0) }
            }},
            
            // L piece
            { TetrominoType.L, new Vector2Int[,] {
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, -1) },
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1) }
            }},
            
            // J piece
            { TetrominoType.J, new Vector2Int[,] {
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1) },
                { new Vector2Int(0, -1), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(-1, -1) }
            }},
            
            // S piece
            { TetrominoType.S, new Vector2Int[,] {
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) },
                { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1) },
                { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) },
                { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1) }
            }},
            
            // Z piece
            { TetrominoType.Z, new Vector2Int[,] {
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
                { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(0, 2) }
            }}
        };

        /// <summary>
        /// Initializes the tetromino with blocks.
        /// </summary>
        public void Initialize(TetrominoType type, Vector2Int startPos, Block blockPrefab)
        {
            Type = type;
            GridPosition = startPos;
            rotationState = 0;

            Vector2Int[,] rotationOffsets = RotationData[type];
            int blockCount = rotationOffsets.GetLength(1);

            // Generate random block values (powers of 2)
            int[] possibleValues = { 2, 2, 2, 4, 4, 8 }; // Weighted towards lower values

            for (int i = 0; i < blockCount; i++)
            {
                Vector2Int localPos = rotationOffsets[0, i];
                
                Block block = Instantiate(blockPrefab, transform);
                block.LocalPosition = localPos;
                block.SetValue(possibleValues[Random.Range(0, possibleValues.Length)]);
                
                Vector3 worldPos = GridManager.Instance.GridToWorld(startPos.x + localPos.x, startPos.y + localPos.y);
                block.transform.position = worldPos;
                
                Blocks.Add(block);
            }

            UpdateBlockPositions();
        }

        /// <summary>
        /// Moves the tetromino in the specified direction.
        /// </summary>
        public bool Move(Vector2Int direction)
        {
            Vector2Int newPos = GridPosition + direction;

            if (GridManager.Instance.CanTetrominoMove(this, newPos))
            {
                GridPosition = newPos;
                UpdateBlockPositions();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the tetromino clockwise.
        /// </summary>
        public bool RotateClockwise()
        {
            int newRotationState = (rotationState + 1) % 4;
            Vector2Int[] newLocalPositions = GetRotationPositions(newRotationState);

            if (GridManager.Instance.CanTetrominoRotate(this, newLocalPositions))
            {
                rotationState = newRotationState;
                ApplyRotation(newLocalPositions);
                return true;
            }

            // Wall kick attempts
            Vector2Int[] wallKickOffsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(2, 0), new Vector2Int(-2, 0) };
            
            foreach (Vector2Int offset in wallKickOffsets)
            {
                Vector2Int testPos = GridPosition + offset;
                Vector2Int[] adjustedPositions = new Vector2Int[newLocalPositions.Length];
                
                for (int i = 0; i < newLocalPositions.Length; i++)
                {
                    adjustedPositions[i] = newLocalPositions[i];
                }

                // Temporarily change position to test
                Vector2Int originalPos = GridPosition;
                GridPosition = testPos;

                if (GridManager.Instance.CanTetrominoRotate(this, adjustedPositions))
                {
                    rotationState = newRotationState;
                    ApplyRotation(adjustedPositions);
                    return true;
                }

                GridPosition = originalPos;
            }

            return false;
        }

        /// <summary>
        /// Gets rotation positions for a given state.
        /// </summary>
        private Vector2Int[] GetRotationPositions(int state)
        {
            Vector2Int[,] rotationOffsets = RotationData[Type];
            int blockCount = rotationOffsets.GetLength(1);
            Vector2Int[] positions = new Vector2Int[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                positions[i] = rotationOffsets[state, i];
            }

            return positions;
        }

        /// <summary>
        /// Applies rotation to all blocks.
        /// </summary>
        private void ApplyRotation(Vector2Int[] newLocalPositions)
        {
            for (int i = 0; i < Blocks.Count && i < newLocalPositions.Length; i++)
            {
                Blocks[i].LocalPosition = newLocalPositions[i];
            }
            UpdateBlockPositions();
        }

        /// <summary>
        /// Updates all block world positions based on grid position.
        /// </summary>
        public void UpdateBlockPositions()
        {
            foreach (Block block in Blocks)
            {
                Vector2Int gridPos = GridPosition + block.LocalPosition;
                Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos.x, gridPos.y);
                block.transform.position = worldPos;
            }
        }

        /// <summary>
        /// Drops the tetromino one cell down.
        /// </summary>
        public bool Drop()
        {
            return Move(Vector2Int.down);
        }

        /// <summary>
        /// Hard drops the tetromino to the bottom.
        /// </summary>
        public int HardDrop()
        {
            int cellsDropped = 0;
            while (Move(Vector2Int.down))
            {
                cellsDropped++;
            }
            return cellsDropped;
        }

        /// <summary>
        /// Locks the tetromino in place on the grid.
        /// </summary>
        public void Lock()
        {
            GridManager.Instance.LockTetromino(this);
            
            // Process merges after locking
            if (MergeLogic.Instance != null)
            {
                MergeLogic.Instance.ProcessMerges();
            }
            
            // Don't destroy the game object - individual blocks are now parented to grid
            Destroy(this); // Remove just the tetromino component
        }

        /// <summary>
        /// Gets the ghost position (where the tetromino would land).
        /// </summary>
        public Vector2Int GetGhostPosition()
        {
            Vector2Int ghostPos = GridPosition;
            
            while (GridManager.Instance.CanTetrominoMove(this, ghostPos + Vector2Int.down))
            {
                ghostPos += Vector2Int.down;
            }
            
            return ghostPos;
        }
    }

    /// <summary>
    /// Types of tetromino shapes.
    /// </summary>
    public enum TetrominoType
    {
        I,
        O,
        T,
        L,
        J,
        S,
        Z
    }
}
