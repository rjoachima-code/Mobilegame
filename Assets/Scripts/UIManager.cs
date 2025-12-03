using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JACAMENO
{
    /// <summary>
    /// Manages in-game UI elements like score, level, and next piece preview.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Score Display")]
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI LinesText;

        [Header("Combo Display")]
        public TextMeshProUGUI ComboText;
        public float ComboDisplayDuration = 2f;

        [Header("Power-Up Display")]
        public TextMeshProUGUI PowerUpCountText;
        public Image PowerUpIcon;

        [Header("Pause Menu")]
        public GameObject PausePanel;
        public Button ResumeButton;
        public Button RestartButton;
        public Button MainMenuButton;

        [Header("Animation Settings")]
        public float ScorePopDuration = 0.2f;
        public float ScorePopScale = 1.2f;

        private float comboDisplayTimer;
        private Vector3 originalScoreScale;

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
            if (ScoreText != null)
                originalScoreScale = ScoreText.transform.localScale;

            // Subscribe to events
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ScoreManager.Instance.OnLevelChanged += UpdateLevelDisplay;
                ScoreManager.Instance.OnLinesCleared += UpdateLinesDisplay;
                ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreDisplay;
            }

            if (MergeLogic.Instance != null)
            {
                MergeLogic.Instance.OnMerge += ShowCombo;
                MergeLogic.Instance.OnComboEnd += HideCombo;
            }

            if (GameState.Instance != null)
            {
                GameState.Instance.OnGamePause += ShowPauseMenu;
                GameState.Instance.OnGameResume += HidePauseMenu;
            }

            // Setup buttons
            SetupButtons();

            // Initial display
            UpdateAllDisplays();
            HidePauseMenu();
            HideCombo(0);
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                ScoreManager.Instance.OnLevelChanged -= UpdateLevelDisplay;
                ScoreManager.Instance.OnLinesCleared -= UpdateLinesDisplay;
                ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreDisplay;
            }

            if (MergeLogic.Instance != null)
            {
                MergeLogic.Instance.OnMerge -= ShowCombo;
                MergeLogic.Instance.OnComboEnd -= HideCombo;
            }

            if (GameState.Instance != null)
            {
                GameState.Instance.OnGamePause -= ShowPauseMenu;
                GameState.Instance.OnGameResume -= HidePauseMenu;
            }
        }

        private void Update()
        {
            // Update combo display timer
            if (comboDisplayTimer > 0)
            {
                comboDisplayTimer -= Time.unscaledDeltaTime;
                if (comboDisplayTimer <= 0)
                {
                    HideCombo(0);
                }
            }
        }

        private void SetupButtons()
        {
            if (ResumeButton != null)
                ResumeButton.onClick.AddListener(() => GameState.Instance?.ResumeGame());

            if (RestartButton != null)
                RestartButton.onClick.AddListener(() => GameState.Instance?.RestartGame());

            if (MainMenuButton != null)
                MainMenuButton.onClick.AddListener(() => GameState.Instance?.GoToMainMenu());
        }

        private void UpdateAllDisplays()
        {
            if (ScoreManager.Instance != null)
            {
                UpdateScoreDisplay(ScoreManager.Instance.GetScore());
                UpdateHighScoreDisplay(ScoreManager.Instance.GetHighScore());
                UpdateLevelDisplay(ScoreManager.Instance.GetLevel());
                UpdateLinesDisplay(ScoreManager.Instance.GetTotalLinesCleared());
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (ScoreText != null)
            {
                ScoreText.text = FormatNumber(score);
                StartCoroutine(PopAnimation(ScoreText.transform));
            }
        }

        private void UpdateHighScoreDisplay(int highScore)
        {
            if (HighScoreText != null)
            {
                HighScoreText.text = $"BEST: {FormatNumber(highScore)}";
            }
        }

        private void UpdateLevelDisplay(int level)
        {
            if (LevelText != null)
            {
                LevelText.text = $"LEVEL {level}";
            }
        }

        private void UpdateLinesDisplay(int lines)
        {
            if (LinesText != null)
            {
                LinesText.text = $"LINES: {lines}";
            }
        }

        private void ShowCombo(int value, int combo)
        {
            if (ComboText != null && combo > 1)
            {
                ComboText.gameObject.SetActive(true);
                ComboText.text = $"COMBO x{combo}!";
                comboDisplayTimer = ComboDisplayDuration;

                // Animate combo text
                StartCoroutine(PopAnimation(ComboText.transform));
            }
        }

        private void HideCombo(int finalCombo)
        {
            if (ComboText != null)
            {
                ComboText.gameObject.SetActive(false);
            }
        }

        private void ShowPauseMenu()
        {
            if (PausePanel != null)
            {
                PausePanel.SetActive(true);
            }
        }

        private void HidePauseMenu()
        {
            if (PausePanel != null)
            {
                PausePanel.SetActive(false);
            }
        }

        private string FormatNumber(int number)
        {
            if (number >= 1000000)
                return $"{number / 1000000f:F1}M";
            if (number >= 1000)
                return $"{number / 1000f:F1}K";
            return number.ToString("N0");
        }

        private System.Collections.IEnumerator PopAnimation(Transform target)
        {
            if (target == null)
                yield break;

            Vector3 originalScale = Vector3.one;
            Vector3 popScale = originalScale * ScorePopScale;
            float elapsed = 0f;

            // Pop up
            while (elapsed < ScorePopDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                target.localScale = Vector3.Lerp(originalScale, popScale, elapsed / (ScorePopDuration / 2));
                yield return null;
            }

            elapsed = 0f;
            // Return to normal
            while (elapsed < ScorePopDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                target.localScale = Vector3.Lerp(popScale, originalScale, elapsed / (ScorePopDuration / 2));
                yield return null;
            }

            target.localScale = originalScale;
        }

        /// <summary>
        /// Shows a power-up notification.
        /// </summary>
        public void ShowPowerUpNotification(string powerUpName)
        {
            // Could add a floating text notification here
            Debug.Log($"Power-Up: {powerUpName}");
        }

        /// <summary>
        /// Updates power-up count display.
        /// </summary>
        public void UpdatePowerUpCount(int count)
        {
            if (PowerUpCountText != null)
            {
                PowerUpCountText.text = count.ToString();
                PowerUpCountText.gameObject.SetActive(count > 0);
            }

            if (PowerUpIcon != null)
            {
                PowerUpIcon.gameObject.SetActive(count > 0);
            }
        }
    }
}
