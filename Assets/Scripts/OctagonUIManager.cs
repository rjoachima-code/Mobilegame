using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Jacameno
{
    /// <summary>
    /// Manages UI elements for the octagon merge game.
    /// Displays score, level, combo, and game status.
    /// </summary>
    public class OctagonUIManager : MonoBehaviour
    {
        public static OctagonUIManager Instance { get; private set; }

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText = null;
        [SerializeField] private TextMeshProUGUI highScoreText = null;
        [SerializeField] private TextMeshProUGUI levelText = null;
        [SerializeField] private TextMeshProUGUI mergesText = null;

        [Header("Combo Display")]
        [SerializeField] private GameObject comboPanel = null;
        [SerializeField] private TextMeshProUGUI comboText = null;
        [SerializeField] private float comboPanelShowDuration = 2f;

        [Header("Level Progress")]
        [SerializeField] private Slider levelProgressBar = null;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel = null;
        [SerializeField] private TextMeshProUGUI finalScoreText = null;
        [SerializeField] private Button restartButton = null;
        [SerializeField] private Button mainMenuButton = null;

        [Header("Animation")]
        [SerializeField] private float scorePopScale = 1.2f;
        [SerializeField] private float scorePopDuration = 0.2f;

        private float comboDisplayTimer = 0f;
        private Coroutine scorePopRoutine = null;

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
            // Subscribe to score manager events
            if (OctagonScoreManager.Instance != null)
            {
                OctagonScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                OctagonScoreManager.Instance.OnLevelChanged += UpdateLevelDisplay;
                OctagonScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreDisplay;
                OctagonScoreManager.Instance.OnComboChanged += UpdateComboDisplay;
            }

            // Setup buttons
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            // Initial UI setup
            if (comboPanel != null)
            {
                comboPanel.SetActive(false);
            }
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            UpdateAllDisplays();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (OctagonScoreManager.Instance != null)
            {
                OctagonScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                OctagonScoreManager.Instance.OnLevelChanged -= UpdateLevelDisplay;
                OctagonScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreDisplay;
                OctagonScoreManager.Instance.OnComboChanged -= UpdateComboDisplay;
            }
        }

        private void Update()
        {
            // Update combo display timer
            if (comboDisplayTimer > 0)
            {
                comboDisplayTimer -= Time.deltaTime;
                if (comboDisplayTimer <= 0 && comboPanel != null)
                {
                    comboPanel.SetActive(false);
                }
            }

            // Update level progress bar
            if (levelProgressBar != null && OctagonScoreManager.Instance != null)
            {
                levelProgressBar.value = OctagonScoreManager.Instance.GetLevelProgress();
            }
        }

        private void UpdateAllDisplays()
        {
            if (OctagonScoreManager.Instance != null)
            {
                UpdateScoreDisplay(OctagonScoreManager.Instance.GetScore());
                UpdateHighScoreDisplay(OctagonScoreManager.Instance.GetHighScore());
                UpdateLevelDisplay(OctagonScoreManager.Instance.GetLevel());
                UpdateMergesDisplay();
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = FormatScore(score);
                
                // Animate score change
                if (scorePopRoutine != null)
                {
                    StopCoroutine(scorePopRoutine);
                }
                scorePopRoutine = StartCoroutine(PopAnimation(scoreText.transform));
            }
        }

        private void UpdateHighScoreDisplay(int highScore)
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"BEST: {FormatScore(highScore)}";
            }
        }

        private void UpdateLevelDisplay(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"LEVEL {level}";
            }
        }

        private void UpdateMergesDisplay()
        {
            if (mergesText != null && OctagonScoreManager.Instance != null)
            {
                int merges = OctagonScoreManager.Instance.GetTotalMerges();
                mergesText.text = $"MERGES: {merges}";
            }
        }

        private void UpdateComboDisplay(int comboCount, int multiplierPercent)
        {
            UpdateMergesDisplay(); // Update merges when combo changes
            
            if (comboPanel != null && comboText != null)
            {
                if (comboCount > 1)
                {
                    comboPanel.SetActive(true);
                    comboText.text = $"COMBO x{comboCount}!\n{multiplierPercent}%";
                    comboDisplayTimer = comboPanelShowDuration;
                    
                    // Animate combo display
                    StartCoroutine(PopAnimation(comboText.transform));
                }
                else
                {
                    comboPanel.SetActive(false);
                }
            }
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (finalScoreText != null && OctagonScoreManager.Instance != null)
                {
                    int finalScore = OctagonScoreManager.Instance.GetScore();
                    int highScore = OctagonScoreManager.Instance.GetHighScore();
                    bool newHighScore = finalScore >= highScore;
                    
                    finalScoreText.text = newHighScore 
                        ? $"NEW HIGH SCORE!\n{FormatScore(finalScore)}"
                        : $"FINAL SCORE\n{FormatScore(finalScore)}";
                }
            }
        }

        public void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private string FormatScore(int score)
        {
            if (score >= 1000000)
            {
                return $"{(score / 1000000f):F1}M";
            }
            else if (score >= 1000)
            {
                return $"{(score / 1000f):F1}K";
            }
            return score.ToString();
        }

        private System.Collections.IEnumerator PopAnimation(Transform target)
        {
            if (target == null)
            {
                yield break;
            }

            Vector3 originalScale = Vector3.one;
            Vector3 popScale = originalScale * scorePopScale;
            float elapsed = 0f;

            // Pop up
            while (elapsed < scorePopDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (scorePopDuration / 2);
                target.localScale = Vector3.Lerp(originalScale, popScale, t);
                yield return null;
            }

            elapsed = 0f;
            // Return to normal
            while (elapsed < scorePopDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (scorePopDuration / 2);
                target.localScale = Vector3.Lerp(popScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private void OnRestartClicked()
        {
            // Reset score and hide game over
            if (OctagonScoreManager.Instance != null)
            {
                OctagonScoreManager.Instance.ResetScore();
            }
            
            HideGameOver();
            
            // Reload scene or restart game logic
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnMainMenuClicked()
        {
            // Return to main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
