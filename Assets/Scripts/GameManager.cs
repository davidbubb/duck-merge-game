using UnityEngine;
using System;

namespace DuckMergeGame
{
    /// <summary>
    /// Main game manager - singleton that controls game flow and state
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Configuration")]
        [SerializeField] private GameConfig gameConfig;
        
        [Header("State")]
        private GameState currentState = GameState.MainMenu;
        private int currentScore = 0;
        private int highScore = 0;
        
        // Events
        public event Action<GameState> OnStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnHighScoreChanged;
        public event Action OnGameOver;
        
        public GameConfig Config => gameConfig;
        public GameState CurrentState => currentState;
        public int CurrentScore => currentScore;
        public int HighScore => highScore;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load high score
            LoadHighScore();
            
            // Set target frame rate
            if (gameConfig != null)
            {
                Application.targetFrameRate = gameConfig.targetFrameRate;
            }
        }
        
        private void Start()
        {
            ChangeState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Change game state
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;
            
            currentState = newState;
            OnStateChanged?.Invoke(newState);
            
            Debug.Log($"Game state changed to: {newState}");
        }
        
        /// <summary>
        /// Start a new game
        /// </summary>
        public void StartNewGame()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
            ChangeState(GameState.Playing);
        }
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }
        
        /// <summary>
        /// Trigger game over
        /// </summary>
        public void TriggerGameOver()
        {
            if (currentState != GameState.Playing) return;
            
            ChangeState(GameState.GameOver);
            Time.timeScale = 0f;
            
            // Update high score if needed
            if (currentScore > highScore)
            {
                highScore = currentScore;
                SaveHighScore();
                OnHighScoreChanged?.Invoke(highScore);
            }
            
            OnGameOver?.Invoke();
            Debug.Log($"Game Over! Final Score: {currentScore}");
        }
        
        /// <summary>
        /// Add score from a merge
        /// </summary>
        public void AddScore(int tier)
        {
            if (gameConfig == null || currentState != GameState.Playing) return;
            
            int points = gameConfig.GetMergePoints(tier);
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
            
            Debug.Log($"Merged tier {tier} -> {tier + 1}: +{points} points. Total: {currentScore}");
        }
        
        /// <summary>
        /// Load high score from PlayerPrefs
        /// </summary>
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            OnHighScoreChanged?.Invoke(highScore);
        }
        
        /// <summary>
        /// Save high score to PlayerPrefs
        /// </summary>
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log($"High score saved: {highScore}");
        }
        
        /// <summary>
        /// Reset high score (for testing)
        /// </summary>
        public void ResetHighScore()
        {
            highScore = 0;
            SaveHighScore();
            OnHighScoreChanged?.Invoke(highScore);
        }
        
        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Quit game
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
