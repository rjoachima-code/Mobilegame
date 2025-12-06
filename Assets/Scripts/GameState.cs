using UnityEngine;
using UnityEngine.SceneManagement;

namespace JACAMENO
{
    /// <summary>
    /// Manages game states (playing, paused, game over).
    /// </summary>
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        public enum State
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        [Header("Current State")]
        public State CurrentState = State.MainMenu;

        [Header("Scene Names")]
        public string MainMenuScene = "MainMenu";
        public string GameScene = "Game";
        public string GameOverScene = "GameOver";

        public event System.Action<State> OnStateChanged;
        public event System.Action OnGameStart;
        public event System.Action OnGamePause;
        public event System.Action OnGameResume;
        public event System.Action OnGameOver;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void StartGame()
        {
            CurrentState = State.Playing;
            Time.timeScale = 1f;
            
            OnStateChanged?.Invoke(CurrentState);
            OnGameStart?.Invoke();

            SceneManager.LoadScene(GameScene);
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState != State.Playing)
                return;

            CurrentState = State.Paused;
            Time.timeScale = 0f;
            
            OnStateChanged?.Invoke(CurrentState);
            OnGamePause?.Invoke();
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState != State.Paused)
                return;

            CurrentState = State.Playing;
            Time.timeScale = 1f;
            
            OnStateChanged?.Invoke(CurrentState);
            OnGameResume?.Invoke();
        }

        /// <summary>
        /// Toggles between pause and play.
        /// </summary>
        public void TogglePause()
        {
            if (CurrentState == State.Playing)
                PauseGame();
            else if (CurrentState == State.Paused)
                ResumeGame();
        }

        /// <summary>
        /// Sets the game to game over state.
        /// </summary>
        public void SetGameOver()
        {
            CurrentState = State.GameOver;
            Time.timeScale = 1f;
            
            OnStateChanged?.Invoke(CurrentState);
            OnGameOver?.Invoke();

            // Delay scene transition for effect
            StartCoroutine(DelayedGameOver());
        }

        private System.Collections.IEnumerator DelayedGameOver()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(GameOverScene);
        }

        /// <summary>
        /// Returns to main menu.
        /// </summary>
        public void GoToMainMenu()
        {
            CurrentState = State.MainMenu;
            Time.timeScale = 1f;
            
            OnStateChanged?.Invoke(CurrentState);
            SceneManager.LoadScene(MainMenuScene);
        }

        /// <summary>
        /// Restarts the current game.
        /// </summary>
        public void RestartGame()
        {
            // Reset managers
            ScoreManager.Instance?.ResetScore();
            PowerUpManager.Instance?.Reset();
            MergeLogic.Instance?.Reset();

            StartGame();
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Checks if the game is currently playing.
        /// </summary>
        public bool IsPlaying()
        {
            return CurrentState == State.Playing;
        }

        /// <summary>
        /// Checks if the game is paused.
        /// </summary>
        public bool IsPaused()
        {
            return CurrentState == State.Paused;
        }

        /// <summary>
        /// Checks if it's game over.
        /// </summary>
        public bool IsGameOver()
        {
            return CurrentState == State.GameOver;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public State GetCurrentState()
        {
            return CurrentState;
        }
    }
}
