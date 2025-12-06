using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Handles player input for movement, rotation, and dropping.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Input Settings")]
        public float MoveDelay = 0.1f;
        public float SoftDropMultiplier = 10f;
        public float AutoRepeatDelay = 0.3f;
        public float AutoRepeatRate = 0.05f;

        [Header("Touch Settings")]
        public float SwipeThreshold = 50f;
        public float TapThreshold = 0.2f;

        private float lastMoveTime;
        private float keyHoldTime;
        private bool isHoldingKey;
        private Vector2 touchStartPos;
        private float touchStartTime;
        private bool isTouching;

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
            if (GameState.Instance == null || !GameState.Instance.IsPlaying())
                return;

            // Handle both keyboard and touch input
            HandleKeyboardInput();
            HandleTouchInput();
        }

        private void HandleKeyboardInput()
        {
            Tetromino current = Spawner.Instance?.GetCurrentTetromino();
            if (current == null)
                return;

            // Rotation
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                current.RotateClockwise();
            }

            // Horizontal movement
            float horizontalInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                horizontalInput = -1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1f;
            }

            if (horizontalInput != 0)
            {
                ProcessHorizontalInput(current, horizontalInput);
            }
            else
            {
                isHoldingKey = false;
                keyHoldTime = 0f;
            }

            // Soft drop
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                GameController.Instance?.SetSoftDrop(true);
            }
            else
            {
                GameController.Instance?.SetSoftDrop(false);
            }

            // Hard drop
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int cellsDropped = current.HardDrop();
                ScoreManager.Instance?.AddScore(cellsDropped * 2);
                GameController.Instance?.LockCurrentTetromino();
            }

            // Pause
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                GameState.Instance.TogglePause();
            }
        }

        private void ProcessHorizontalInput(Tetromino tetromino, float direction)
        {
            Vector2Int moveDir = direction < 0 ? Vector2Int.left : Vector2Int.right;

            if (!isHoldingKey)
            {
                // First press
                tetromino.Move(moveDir);
                isHoldingKey = true;
                keyHoldTime = 0f;
            }
            else
            {
                keyHoldTime += Time.deltaTime;
                
                if (keyHoldTime >= AutoRepeatDelay)
                {
                    // Auto-repeat
                    if (Time.time - lastMoveTime >= AutoRepeatRate)
                    {
                        tetromino.Move(moveDir);
                        lastMoveTime = Time.time;
                    }
                }
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                isTouching = false;
                return;
            }

            Touch touch = Input.GetTouch(0);
            Tetromino current = Spawner.Instance?.GetCurrentTetromino();
            
            if (current == null)
                return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                    isTouching = true;
                    break;

                case TouchPhase.Moved:
                    if (isTouching)
                    {
                        Vector2 delta = touch.position - touchStartPos;
                        
                        // Check for horizontal swipe
                        if (Mathf.Abs(delta.x) > SwipeThreshold)
                        {
                            if (delta.x > 0)
                            {
                                current.Move(Vector2Int.right);
                            }
                            else
                            {
                                current.Move(Vector2Int.left);
                            }
                            touchStartPos = touch.position;
                        }
                        
                        // Check for downward swipe (soft drop)
                        if (delta.y < -SwipeThreshold)
                        {
                            GameController.Instance?.SetSoftDrop(true);
                            touchStartPos = touch.position;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isTouching)
                    {
                        float touchDuration = Time.time - touchStartTime;
                        Vector2 delta = touch.position - touchStartPos;

                        // Quick tap for rotation
                        if (touchDuration < TapThreshold && delta.magnitude < SwipeThreshold)
                        {
                            // Tap on left side = rotate counter-clockwise (not implemented, just rotate)
                            // Tap on right side = rotate clockwise
                            current.RotateClockwise();
                        }
                        
                        // Quick swipe down for hard drop
                        if (delta.y < -SwipeThreshold * 2 && touchDuration < TapThreshold)
                        {
                            int cellsDropped = current.HardDrop();
                            ScoreManager.Instance?.AddScore(cellsDropped * 2);
                            GameController.Instance?.LockCurrentTetromino();
                        }
                    }
                    
                    GameController.Instance?.SetSoftDrop(false);
                    isTouching = false;
                    break;

                case TouchPhase.Canceled:
                    GameController.Instance?.SetSoftDrop(false);
                    isTouching = false;
                    break;
            }
        }

        /// <summary>
        /// Checks if any input is currently being processed.
        /// </summary>
        public bool IsInputActive()
        {
            return isTouching || Input.anyKey;
        }
    }
}
