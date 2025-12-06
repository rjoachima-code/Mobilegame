using System.Collections.Generic;
using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Manages power-ups including spawning, activation, and effects.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }

        [Header("Power-Up Settings")]
        public float SpawnChance = 0.1f;
        public int MinRowsForSpawn = 3;
        public float PowerUpDuration = 5f;

        [Header("Power-Up Types")]
        public PowerUpData[] PowerUpTypes;

        private List<ActivePowerUp> activePowerUps = new List<ActivePowerUp>();
        private Queue<PowerUpType> powerUpQueue = new Queue<PowerUpType>();

        public event System.Action<PowerUpType> OnPowerUpCollected;
        public event System.Action<PowerUpType> OnPowerUpActivated;
        public event System.Action<PowerUpType> OnPowerUpExpired;

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
            UpdateActivePowerUps();
        }

        /// <summary>
        /// Attempts to spawn a power-up after row clears.
        /// </summary>
        public void TrySpawnPowerUp(int rowsCleared)
        {
            if (rowsCleared < MinRowsForSpawn)
                return;

            float adjustedChance = SpawnChance * rowsCleared;
            
            if (Random.value < adjustedChance)
            {
                PowerUpType type = GetRandomPowerUpType();
                AddPowerUpToQueue(type);
            }
        }

        /// <summary>
        /// Gets a random power-up type.
        /// </summary>
        private PowerUpType GetRandomPowerUpType()
        {
            PowerUpType[] types = (PowerUpType[])System.Enum.GetValues(typeof(PowerUpType));
            return types[Random.Range(0, types.Length)];
        }

        /// <summary>
        /// Adds a power-up to the queue.
        /// </summary>
        public void AddPowerUpToQueue(PowerUpType type)
        {
            powerUpQueue.Enqueue(type);
            OnPowerUpCollected?.Invoke(type);
        }

        /// <summary>
        /// Activates the next power-up in the queue.
        /// </summary>
        public bool ActivateNextPowerUp()
        {
            if (powerUpQueue.Count == 0)
                return false;

            PowerUpType type = powerUpQueue.Dequeue();
            ActivatePowerUp(type);
            return true;
        }

        /// <summary>
        /// Activates a specific power-up.
        /// </summary>
        public void ActivatePowerUp(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.ClearRow:
                    ActivateClearRow();
                    break;
                case PowerUpType.Bomb:
                    ActivateBomb();
                    break;
                case PowerUpType.Freeze:
                    ActivateFreeze();
                    break;
                case PowerUpType.SlowDown:
                    ActivateSlowDown();
                    break;
                case PowerUpType.ColorBomb:
                    ActivateColorBomb();
                    break;
                case PowerUpType.Shuffle:
                    ActivateShuffle();
                    break;
            }

            OnPowerUpActivated?.Invoke(type);
        }

        /// <summary>
        /// Clears the bottom row.
        /// </summary>
        private void ActivateClearRow()
        {
            GridManager grid = GridManager.Instance;
            
            // Find the lowest non-empty row
            for (int y = 0; y < grid.GridHeight; y++)
            {
                bool hasBlocks = false;
                for (int x = 0; x < grid.GridWidth; x++)
                {
                    if (grid.GetBlock(x, y) != null)
                    {
                        hasBlocks = true;
                        break;
                    }
                }

                if (hasBlocks)
                {
                    // Clear this row
                    for (int x = 0; x < grid.GridWidth; x++)
                    {
                        Block block = grid.GetBlock(x, y);
                        if (block != null)
                        {
                            grid.RemoveBlock(x, y);
                            block.DestroyBlock();
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Destroys blocks in a 3x3 area around the center.
        /// </summary>
        private void ActivateBomb()
        {
            GridManager grid = GridManager.Instance;
            
            // Find center of the grid (or active play area)
            int centerX = grid.GridWidth / 2;
            int centerY = 5; // Lower area where blocks accumulate

            // Find the actual center based on blocks
            for (int y = grid.GridHeight - 1; y >= 0; y--)
            {
                for (int x = 0; x < grid.GridWidth; x++)
                {
                    if (grid.GetBlock(x, y) != null)
                    {
                        centerX = x;
                        centerY = y;
                        break;
                    }
                }
            }

            // Destroy 3x3 area
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;
                    
                    if (grid.IsInsideGrid(x, y))
                    {
                        Block block = grid.GetBlock(x, y);
                        if (block != null)
                        {
                            grid.RemoveBlock(x, y);
                            block.DestroyBlock();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Freezes the current tetromino in place for a duration.
        /// </summary>
        private void ActivateFreeze()
        {
            ActivePowerUp powerUp = new ActivePowerUp
            {
                Type = PowerUpType.Freeze,
                RemainingTime = PowerUpDuration
            };
            activePowerUps.Add(powerUp);
        }

        /// <summary>
        /// Slows down the drop speed temporarily.
        /// </summary>
        private void ActivateSlowDown()
        {
            ActivePowerUp powerUp = new ActivePowerUp
            {
                Type = PowerUpType.SlowDown,
                RemainingTime = PowerUpDuration * 2
            };
            activePowerUps.Add(powerUp);
        }

        /// <summary>
        /// Destroys all blocks of a random color/value.
        /// </summary>
        private void ActivateColorBomb()
        {
            GridManager grid = GridManager.Instance;
            List<Block> allBlocks = new List<Block>();

            // Collect all blocks
            for (int x = 0; x < grid.GridWidth; x++)
            {
                for (int y = 0; y < grid.GridHeight; y++)
                {
                    Block block = grid.GetBlock(x, y);
                    if (block != null)
                    {
                        allBlocks.Add(block);
                    }
                }
            }

            if (allBlocks.Count == 0)
                return;

            // Pick a random value to destroy
            int targetValue = allBlocks[Random.Range(0, allBlocks.Count)].Value;

            // Destroy all blocks with that value
            foreach (Block block in allBlocks)
            {
                if (block.Value == targetValue)
                {
                    grid.RemoveBlock(block.GridPosition.x, block.GridPosition.y);
                    block.DestroyBlock();
                }
            }
        }

        /// <summary>
        /// Shuffles all block values on the grid.
        /// </summary>
        private void ActivateShuffle()
        {
            GridManager grid = GridManager.Instance;
            List<Block> allBlocks = new List<Block>();

            // Collect all blocks
            for (int x = 0; x < grid.GridWidth; x++)
            {
                for (int y = 0; y < grid.GridHeight; y++)
                {
                    Block block = grid.GetBlock(x, y);
                    if (block != null)
                    {
                        allBlocks.Add(block);
                    }
                }
            }

            if (allBlocks.Count < 2)
                return;

            // Collect all values
            List<int> values = new List<int>();
            foreach (Block block in allBlocks)
            {
                values.Add(block.Value);
            }

            // Shuffle values
            for (int i = values.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }

            // Assign shuffled values
            for (int i = 0; i < allBlocks.Count; i++)
            {
                allBlocks[i].SetValue(values[i]);
            }
        }

        /// <summary>
        /// Updates active power-up timers.
        /// </summary>
        private void UpdateActivePowerUps()
        {
            for (int i = activePowerUps.Count - 1; i >= 0; i--)
            {
                activePowerUps[i].RemainingTime -= Time.deltaTime;
                
                if (activePowerUps[i].RemainingTime <= 0)
                {
                    OnPowerUpExpired?.Invoke(activePowerUps[i].Type);
                    activePowerUps.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Checks if a specific power-up type is active.
        /// </summary>
        public bool IsPowerUpActive(PowerUpType type)
        {
            foreach (ActivePowerUp powerUp in activePowerUps)
            {
                if (powerUp.Type == type)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of power-ups in queue.
        /// </summary>
        public int GetQueuedPowerUpCount()
        {
            return powerUpQueue.Count;
        }

        /// <summary>
        /// Gets the speed multiplier based on active power-ups.
        /// </summary>
        public float GetSpeedMultiplier()
        {
            if (IsPowerUpActive(PowerUpType.Freeze))
                return 0f;
            
            if (IsPowerUpActive(PowerUpType.SlowDown))
                return 0.5f;
            
            return 1f;
        }

        /// <summary>
        /// Resets the power-up manager state.
        /// </summary>
        public void Reset()
        {
            activePowerUps.Clear();
            powerUpQueue.Clear();
        }

        private class ActivePowerUp
        {
            public PowerUpType Type;
            public float RemainingTime;
        }
    }

    /// <summary>
    /// Types of power-ups available.
    /// </summary>
    public enum PowerUpType
    {
        ClearRow,
        Bomb,
        Freeze,
        SlowDown,
        ColorBomb,
        Shuffle
    }

    /// <summary>
    /// Data container for power-up configuration.
    /// </summary>
    [System.Serializable]
    public class PowerUpData
    {
        public PowerUpType Type;
        public string DisplayName;
        public Color Color;
        public Sprite Icon;
    }
}
