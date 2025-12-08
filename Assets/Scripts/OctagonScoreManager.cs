using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// Manages scoring for the octagon merge game.
    /// Scoring is based on:
    /// - Shape merges (higher vertex count = more points)
    /// - Combo chains (consecutive merges multiply score)
    /// - Shape evolution level (advanced shapes worth more)
    /// - Magnet pulls (multi-column merges bonus)
    /// </summary>
    public class OctagonScoreManager : MonoBehaviour
    {
        public static OctagonScoreManager Instance { get; private set; }

        [Header("Score Settings")]
        [SerializeField] private int baseScorePerMerge = 50;
        [SerializeField] private float vertexMultiplier = 10f;  // Each vertex adds points
        [SerializeField] private float comboMultiplier = 1.5f;  // Each combo level multiplies
        [SerializeField] private int magnetBonusPerShape = 100; // Bonus for multi-shape merges

        [Header("Level Settings")]
        [SerializeField] private int pointsPerLevel = 1000;
        [SerializeField] private int maxLevel = 20;
        [SerializeField] private int levelUpBonus = 100; // Multiplied by level number
        
        [Header("Combo Settings")]
        [SerializeField] private int comboBonusPerLevel = 50; // Multiplied by combo count

        private int score = 0;
        private int highScore = 0;
        private int level = 1;
        private int comboCount = 0;
        private float lastMergeTime = 0f;
        private float comboTimeWindow = 2f;
        private int totalMerges = 0;

        public event System.Action<int> OnScoreChanged;
        public event System.Action<int> OnLevelChanged;
        public event System.Action<int> OnHighScoreChanged;
        public event System.Action<int, int> OnComboChanged; // combo count, multiplier

        private const string HighScoreKey = "JACAMENO_OctagonHighScore";

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

        private void Update()
        {
            // Check for combo timeout
            if (comboCount > 0 && Time.time - lastMergeTime > comboTimeWindow)
            {
                ResetCombo();
            }
        }

        /// <summary>
        /// Calculates and adds score for a shape merge.
        /// </summary>
        /// <param name="mergedShape">The resulting shape data after merge</param>
        /// <param name="shapesInvolved">Number of shapes that were merged (2+ for magnet pulls)</param>
        public void AddMergeScore(ShapeData mergedShape, int shapesInvolved = 2)
        {
            if (mergedShape == null)
            {
                Debug.LogWarning("Attempting to score with null shape data");
                return;
            }

            // Base score from shape
            int shapeScore = baseScorePerMerge + (int)(mergedShape.VertexCount * vertexMultiplier);

            // Combo multiplier
            float currentComboMult = 1f + (comboCount * (comboMultiplier - 1f));
            shapeScore = Mathf.RoundToInt(shapeScore * currentComboMult);

            // Magnet bonus for multi-shape merges
            if (shapesInvolved > 2)
            {
                int magnetBonus = (shapesInvolved - 2) * magnetBonusPerShape;
                shapeScore += magnetBonus;
            }

            // Level multiplier
            float levelMult = 1f + ((level - 1) * 0.1f);
            shapeScore = Mathf.RoundToInt(shapeScore * levelMult);

            // Add to total score
            AddScore(shapeScore);

            // Update combo
            comboCount++;
            lastMergeTime = Time.time;
            totalMerges++;
            
            OnComboChanged?.Invoke(comboCount, Mathf.RoundToInt(currentComboMult * 100));
        }

        /// <summary>
        /// Adds raw score points.
        /// </summary>
        private void AddScore(int points)
        {
            score += points;
            OnScoreChanged?.Invoke(score);

            // Check for high score
            if (score > highScore)
            {
                highScore = score;
                SaveHighScore();
                OnHighScoreChanged?.Invoke(highScore);
            }

            // Check for level up
            CheckLevelUp();
        }

        /// <summary>
        /// Checks if player should level up based on score.
        /// </summary>
        private void CheckLevelUp()
        {
            int targetLevel = Mathf.Min((score / pointsPerLevel) + 1, maxLevel);
            
            if (targetLevel > level)
            {
                level = targetLevel;
                OnLevelChanged?.Invoke(level);
                
                // Bonus for leveling up
                AddScore(level * levelUpBonus);
            }
        }

        /// <summary>
        /// Resets the combo counter.
        /// </summary>
        private void ResetCombo()
        {
            if (comboCount > 1)
            {
                // Award combo bonus
                int comboBonus = comboCount * comboBonusPerLevel;
                AddScore(comboBonus);
            }
            
            comboCount = 0;
            OnComboChanged?.Invoke(0, 100);
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
        /// Gets the current combo count.
        /// </summary>
        public int GetComboCount()
        {
            return comboCount;
        }

        /// <summary>
        /// Gets total merges performed.
        /// </summary>
        public int GetTotalMerges()
        {
            return totalMerges;
        }

        /// <summary>
        /// Resets score for a new game.
        /// </summary>
        public void ResetScore()
        {
            score = 0;
            level = 1;
            comboCount = 0;
            totalMerges = 0;
            lastMergeTime = 0f;

            OnScoreChanged?.Invoke(score);
            OnLevelChanged?.Invoke(level);
            OnComboChanged?.Invoke(0, 100);
        }

        /// <summary>
        /// Saves high score to PlayerPrefs.
        /// </summary>
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads high score from PlayerPrefs.
        /// </summary>
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        /// <summary>
        /// Resets high score (for testing).
        /// </summary>
        public void ResetHighScore()
        {
            highScore = 0;
            PlayerPrefs.DeleteKey(HighScoreKey);
            OnHighScoreChanged?.Invoke(highScore);
        }

        /// <summary>
        /// Gets score progress to next level.
        /// </summary>
        public float GetLevelProgress()
        {
            int currentLevelScore = (level - 1) * pointsPerLevel;
            int nextLevelScore = level * pointsPerLevel;
            return (float)(score - currentLevelScore) / (nextLevelScore - currentLevelScore);
        }
    }
}
