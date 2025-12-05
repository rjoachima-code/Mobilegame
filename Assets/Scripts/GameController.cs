using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Main game controller handling game loop, tetromino falling, and locking.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [Header("Game Settings")]
        public float BaseDropInterval = 1f;
        public float SoftDropMultiplier = 10f;
        public float LockDelay = 0.5f;

        private float dropTimer;
        private float lockTimer;
        private bool isSoftDropping;
        private bool isLocking;

        private Tetromino currentTetromino;

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
            // Wait for GameState to be ready
            if (GameState.Instance != null)
            {
                GameState.Instance.CurrentState = GameState.State.Playing;
            }

            // Spawn first tetromino
            StartCoroutine(DelayedStart());
        }

        private System.Collections.IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.5f);
            SpawnNewTetromino();
        }

        private void Update()
        {
            if (GameState.Instance == null || !GameState.Instance.IsPlaying())
                return;

            if (currentTetromino == null)
            {
                SpawnNewTetromino();
                return;
            }

            UpdateDropTimer();
        }

        /// <summary>
        /// Updates the drop timer and moves tetromino down.
        /// </summary>
        private void UpdateDropTimer()
        {
            float dropInterval = GetCurrentDropInterval();
            
            // Apply power-up speed modifier
            float speedMultiplier = PowerUpManager.Instance?.GetSpeedMultiplier() ?? 1f;
            dropInterval /= speedMultiplier;

            dropTimer += Time.deltaTime;

            if (dropTimer >= dropInterval)
            {
                dropTimer = 0f;
                
                if (!currentTetromino.Drop())
                {
                    // Tetromino can't move down, start lock delay
                    HandleLocking();
                }
                else
                {
                    // Reset lock timer if moving
                    isLocking = false;
                    lockTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Handles the lock delay when tetromino reaches bottom.
        /// </summary>
        private void HandleLocking()
        {
            if (!isLocking)
            {
                isLocking = true;
                lockTimer = 0f;
            }

            lockTimer += Time.deltaTime;

            if (lockTimer >= LockDelay)
            {
                LockCurrentTetromino();
            }
        }

        /// <summary>
        /// Gets the current drop interval based on level and soft drop.
        /// </summary>
        private float GetCurrentDropInterval()
        {
            float interval = ScoreManager.Instance?.GetDropSpeed() ?? BaseDropInterval;

            if (isSoftDropping)
            {
                interval /= SoftDropMultiplier;
            }

            return interval;
        }

        /// <summary>
        /// Sets soft drop state.
        /// </summary>
        public void SetSoftDrop(bool enabled)
        {
            isSoftDropping = enabled;
        }

        /// <summary>
        /// Locks the current tetromino and spawns a new one.
        /// </summary>
        public void LockCurrentTetromino()
        {
            if (currentTetromino == null)
                return;

            currentTetromino.Lock();
            currentTetromino = null;
            
            Spawner.Instance?.ClearCurrentTetromino();
            
            isLocking = false;
            lockTimer = 0f;
            dropTimer = 0f;

            // Check for game over
            if (GridManager.Instance.IsGameOver())
            {
                GameState.Instance.SetGameOver();
                return;
            }

            // Spawn new tetromino after a short delay
            Invoke(nameof(SpawnNewTetromino), 0.1f);
        }

        /// <summary>
        /// Spawns a new tetromino.
        /// </summary>
        private void SpawnNewTetromino()
        {
            currentTetromino = Spawner.Instance?.SpawnTetromino();
            dropTimer = 0f;
        }

        /// <summary>
        /// Gets the current tetromino.
        /// </summary>
        public Tetromino GetCurrentTetromino()
        {
            return currentTetromino;
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            GameState.Instance?.PauseGame();
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame()
        {
            GameState.Instance?.ResumeGame();
        }
    }
}
