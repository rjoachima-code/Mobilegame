using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JACAMENO
{
    /// <summary>
    /// Manages the game over scene UI.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Display")]
        public TextMeshProUGUI GameOverText;
        public TextMeshProUGUI FinalScoreText;
        public TextMeshProUGUI HighScoreText;
        public TextMeshProUGUI LinesText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI NewHighScoreText;

        [Header("Buttons")]
        public Button RetryButton;
        public Button MainMenuButton;
        public Button ShareButton;

        [Header("Animation")]
        public float FadeInDuration = 1f;
        public CanvasGroup MainCanvasGroup;

        private int finalScore;
        private int highScore;
        private bool isNewHighScore;

        private void Start()
        {
            SetupButtons();
            LoadGameData();
            StartCoroutine(FadeIn());
        }

        private void SetupButtons()
        {
            if (RetryButton != null)
            {
                RetryButton.onClick.AddListener(OnRetryClicked);
            }

            if (MainMenuButton != null)
            {
                MainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }

            if (ShareButton != null)
            {
                ShareButton.onClick.AddListener(OnShareClicked);
            }
        }

        private void LoadGameData()
        {
            // Get final score from ScoreManager or PlayerPrefs
            if (ScoreManager.Instance != null)
            {
                finalScore = ScoreManager.Instance.GetScore();
                highScore = ScoreManager.Instance.GetHighScore();
                
                if (LinesText != null)
                    LinesText.text = $"LINES: {ScoreManager.Instance.GetTotalLinesCleared()}";
                
                if (LevelText != null)
                    LevelText.text = $"LEVEL: {ScoreManager.Instance.GetLevel()}";
            }
            else
            {
                // Fallback to PlayerPrefs
                finalScore = PlayerPrefs.GetInt("LastScore", 0);
                highScore = PlayerPrefs.GetInt("JACAMENO_HighScore", 0);
            }

            isNewHighScore = finalScore >= highScore && finalScore > 0;

            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            if (FinalScoreText != null)
            {
                FinalScoreText.text = finalScore.ToString("N0");
            }

            if (HighScoreText != null)
            {
                HighScoreText.text = $"BEST: {highScore:N0}";
            }

            if (NewHighScoreText != null)
            {
                NewHighScoreText.gameObject.SetActive(isNewHighScore);
            }
        }

        private System.Collections.IEnumerator FadeIn()
        {
            if (MainCanvasGroup == null)
                yield break;

            MainCanvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < FadeInDuration)
            {
                elapsed += Time.deltaTime;
                MainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / FadeInDuration);
                yield return null;
            }

            MainCanvasGroup.alpha = 1f;

            // Animate new high score text
            if (isNewHighScore && NewHighScoreText != null)
            {
                StartCoroutine(PulseText(NewHighScoreText.transform));
            }
        }

        private System.Collections.IEnumerator PulseText(Transform textTransform)
        {
            Vector3 originalScale = textTransform.localScale;
            
            while (true)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
                textTransform.localScale = originalScale * pulse;
                yield return null;
            }
        }

        private void OnRetryClicked()
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.RestartGame();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
        }

        private void OnMainMenuClicked()
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.GoToMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        private void OnShareClicked()
        {
            string shareText = $"I scored {finalScore:N0} points in JACAMENO! Can you beat my score?";
            
            // Copy to clipboard - works on all platforms
            GUIUtility.systemCopyBuffer = shareText;
            Debug.Log("Score copied to clipboard!");
#endif
        }
    }
}
