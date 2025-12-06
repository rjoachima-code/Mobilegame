using System.Collections.Generic;
using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Handles block merging logic and combo detection (M2 Block style).
    /// </summary>
    public class MergeLogic : MonoBehaviour
    {
        public static MergeLogic Instance { get; private set; }

        [Header("Merge Settings")]
        public float MergeDelay = 0.1f;
        public int MaxMergeIterations = 100;

        [Header("Combo Settings")]
        public float ComboTimeWindow = 2f;

        private int currentCombo = 0;
        private float lastMergeTime;
        private int totalMergesInCombo = 0;

        public event System.Action<int, int> OnMerge; // value, combo
        public event System.Action<int> OnComboEnd;

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

        private void Update()
        {
            // Check for combo timeout
            if (currentCombo > 0 && Time.time - lastMergeTime > ComboTimeWindow)
            {
                EndCombo();
            }
        }

        /// <summary>
        /// Processes all possible merges on the grid.
        /// </summary>
        public void ProcessMerges()
        {
            StartCoroutine(ProcessMergesCoroutine());
        }

        private System.Collections.IEnumerator ProcessMergesCoroutine()
        {
            int iterations = 0;
            bool mergesOccurred = true;

            while (mergesOccurred && iterations < MaxMergeIterations)
            {
                mergesOccurred = false;
                iterations++;

                // Scan for adjacent blocks with same value
                List<MergePair> mergePairs = FindMergePairs();

                if (mergePairs.Count > 0)
                {
                    mergesOccurred = true;

                    foreach (MergePair pair in mergePairs)
                    {
                        ExecuteMerge(pair);
                    }

                    yield return new WaitForSeconds(MergeDelay);

                    // Apply gravity after merges
                    ApplyGravity();
                    yield return new WaitForSeconds(MergeDelay);
                }
            }

            // Check for completed rows after all merges
            int rowsCleared = GridManager.Instance.ClearCompletedRows();
            if (rowsCleared > 0)
            {
                ScoreManager.Instance?.AddRowClearScore(rowsCleared, currentCombo);
            }
        }

        /// <summary>
        /// Finds all valid merge pairs on the grid.
        /// </summary>
        private List<MergePair> FindMergePairs()
        {
            List<MergePair> pairs = new List<MergePair>();
            HashSet<Block> usedBlocks = new HashSet<Block>();

            GridManager grid = GridManager.Instance;

            for (int y = 0; y < grid.GridHeight; y++)
            {
                for (int x = 0; x < grid.GridWidth; x++)
                {
                    Block block = grid.GetBlock(x, y);
                    
                    if (block == null || usedBlocks.Contains(block) || !block.IsLocked)
                        continue;

                    // Check right neighbor
                    Block rightBlock = grid.GetBlock(x + 1, y);
                    if (rightBlock != null && !usedBlocks.Contains(rightBlock) && 
                        rightBlock.IsLocked && block.Value == rightBlock.Value)
                    {
                        pairs.Add(new MergePair(block, rightBlock));
                        usedBlocks.Add(block);
                        usedBlocks.Add(rightBlock);
                        continue;
                    }

                    // Check top neighbor
                    Block topBlock = grid.GetBlock(x, y + 1);
                    if (topBlock != null && !usedBlocks.Contains(topBlock) && 
                        topBlock.IsLocked && block.Value == topBlock.Value)
                    {
                        pairs.Add(new MergePair(block, topBlock));
                        usedBlocks.Add(block);
                        usedBlocks.Add(topBlock);
                    }
                }
            }

            return pairs;
        }

        /// <summary>
        /// Executes a merge between two blocks.
        /// </summary>
        private void ExecuteMerge(MergePair pair)
        {
            GridManager grid = GridManager.Instance;

            // Double the value of the first block
            int newValue = pair.Block1.Value * 2;
            pair.Block1.SetValue(newValue);
            pair.Block1.PlayMergeEffect();

            // Remove second block from grid
            grid.RemoveBlock(pair.Block2.GridPosition.x, pair.Block2.GridPosition.y);
            Destroy(pair.Block2.gameObject);

            // Update combo
            currentCombo++;
            totalMergesInCombo++;
            lastMergeTime = Time.time;

            // Calculate and add score
            int mergeScore = CalculateMergeScore(newValue, currentCombo);
            ScoreManager.Instance?.AddScore(mergeScore);

            // Trigger event
            OnMerge?.Invoke(newValue, currentCombo);
        }

        /// <summary>
        /// Calculates score for a merge based on value and combo.
        /// </summary>
        private int CalculateMergeScore(int blockValue, int combo)
        {
            int baseScore = blockValue;
            float comboMultiplier = 1f + (combo * 0.5f);
            return Mathf.RoundToInt(baseScore * comboMultiplier);
        }

        /// <summary>
        /// Applies gravity to all blocks after merges.
        /// </summary>
        private void ApplyGravity()
        {
            GridManager grid = GridManager.Instance;

            for (int x = 0; x < grid.GridWidth; x++)
            {
                for (int y = 1; y < grid.GridHeight; y++)
                {
                    Block block = grid.GetBlock(x, y);
                    
                    if (block != null && block.IsLocked)
                    {
                        // Find lowest empty position
                        int targetY = y;
                        while (targetY > 0 && grid.GetBlock(x, targetY - 1) == null)
                        {
                            targetY--;
                        }

                        if (targetY != y)
                        {
                            // Move block down
                            grid.RemoveBlock(x, y);
                            grid.PlaceBlock(block, x, targetY);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when a block is merged (from Block script).
        /// </summary>
        public void OnBlockMerged(Block block, int newValue)
        {
            // Additional merge handling if needed
        }

        /// <summary>
        /// Ends the current combo.
        /// </summary>
        private void EndCombo()
        {
            if (currentCombo > 1)
            {
                OnComboEnd?.Invoke(currentCombo);
                
                // Bonus score for combo chain
                int comboBonus = currentCombo * 50;
                ScoreManager.Instance?.AddScore(comboBonus);
            }

            currentCombo = 0;
            totalMergesInCombo = 0;
        }

        /// <summary>
        /// Gets the current combo count.
        /// </summary>
        public int GetCurrentCombo()
        {
            return currentCombo;
        }

        /// <summary>
        /// Resets the merge logic state.
        /// </summary>
        public void Reset()
        {
            currentCombo = 0;
            totalMergesInCombo = 0;
            lastMergeTime = 0;
        }

        /// <summary>
        /// Represents a pair of blocks to merge.
        /// </summary>
        private struct MergePair
        {
            public Block Block1;
            public Block Block2;

            public MergePair(Block b1, Block b2)
            {
                Block1 = b1;
                Block2 = b2;
            }
        }
    }
}
