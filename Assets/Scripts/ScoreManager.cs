using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Manages scoring, levels, and score-related events.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Settings")]
        public int ScorePerRow = 100;
        public int ScorePerLevel = 1000;
        public float[] RowClearMultipliers = { 1f, 3f, 5f, 8f }; // 1, 2, 3, 4 rows

        [Header("Level Settings")]
        public int StartingLevel = 1;
        public int MaxLevel = 20;
        public int LinesPerLevel = 10;

        private int score = 0;
        private int highScore = 0;
        private int level = 1;
        private int linesCleared = 0;
        private int totalLinesCleared = 0;

        public event System.Action<int> OnScoreChanged;
        public event System.Action<int> OnLevelChanged;
        public event System.Action<int> OnHighScoreChanged;
        public event System.Action<int> OnLinesCleared;

        private const string HighScoreKey = "JACAMENO_HighScore";

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

            LoadHighScore();
        }

        private void Start()
        {
            level = StartingLevel;
            OnLevelChanged?.Invoke(level);
        }

        /// <summary>
        /// Adds score with optional multiplier.
        /// </summary>
        public void AddScore(int points)
        {
            score += points;
            OnScoreChanged?.Invoke(score);

            if (score > highScore)
            {
                highScore = score;
                SaveHighScore();
                OnHighScoreChanged?.Invoke(highScore);
            }
        }

        /// <summary>
        /// Adds score for row clears with combo multiplier.
        /// </summary>
        public void AddRowClearScore(int rowsCleared, int combo)
        {
            if (rowsCleared <= 0)
                return;

            // Base score with row multiplier
            int multiplierIndex = Mathf.Clamp(rowsCleared - 1, 0, RowClearMultipliers.Length - 1);
            float rowMultiplier = RowClearMultipliers[multiplierIndex];
            
            // Combo multiplier
            float comboMultiplier = 1f + (combo * 0.25f);
            
            // Level multiplier
            float levelMultiplier = 1f + ((level - 1) * 0.1f);

            int rowScore = Mathf.RoundToInt(ScorePerRow * rowsCleared * rowMultiplier * comboMultiplier * levelMultiplier);
            AddScore(rowScore);

            // Update lines and level
            linesCleared += rowsCleared;
            totalLinesCleared += rowsCleared;
            OnLinesCleared?.Invoke(totalLinesCleared);

            // Check for level up
            if (linesCleared >= LinesPerLevel && level < MaxLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Increases the level.
        /// </summary>
        private void LevelUp()
        {
            linesCleared -= LinesPerLevel;
            level++;
            
            if (level > MaxLevel)
                level = MaxLevel;

            OnLevelChanged?.Invoke(level);

            // Bonus score for leveling up
            AddScore(ScorePerLevel);
        }

        /// <summary>
        /// Gets the current score.
        /// </summary>
        public int GetScore()
        {
            return score;
        }

        /// <summary>
        /// Gets the high score.
        /// </summary>
        public int GetHighScore()
        {
            return highScore;
        }

        /// <summary>
        /// Gets the current level.
        /// </summary>
        public int GetLevel()
        {
            return level;
        }

        /// <summary>
        /// Gets total lines cleared.
        /// </summary>
        public int GetTotalLinesCleared()
        {
            return totalLinesCleared;
        }

        /// <summary>
        /// Resets the score for a new game.
        /// </summary>
        public void ResetScore()
        {
            score = 0;
            level = StartingLevel;
            linesCleared = 0;
            totalLinesCleared = 0;

            OnScoreChanged?.Invoke(score);
            OnLevelChanged?.Invoke(level);
            OnLinesCleared?.Invoke(totalLinesCleared);
        }

        /// <summary>
        /// Saves the high score to PlayerPrefs.
        /// </summary>
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the high score from PlayerPrefs.
        /// </summary>
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        /// <summary>
        /// Gets the drop speed based on current level.
        /// </summary>
        public float GetDropSpeed()
        {
            // Speed increases with level (time between drops decreases)
            float baseInterval = 1f;
            float minInterval = 0.05f;
            float speedIncrease = (level - 1) * 0.05f;
            
            return Mathf.Max(minInterval, baseInterval - speedIncrease);
        }

        /// <summary>
        /// Resets the high score (for testing).
        /// </summary>
        public void ResetHighScore()
        {
            highScore = 0;
            PlayerPrefs.DeleteKey(HighScoreKey);
            OnHighScoreChanged?.Invoke(highScore);
        }
    }
}
