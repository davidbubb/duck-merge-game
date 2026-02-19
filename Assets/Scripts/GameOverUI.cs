using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the game over screen UI
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        
        private void Start()
        {
            // Setup button listeners
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetryClicked);
            }
            
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuClicked);
            }
            
            // Subscribe to events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnGameOver += OnGameOver;
            }
            
            // Hide by default
            HideGameOver();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnGameOver -= OnGameOver;
            }
        }
        
        private void OnRetryClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }
        
        private void OnMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMainMenu();
            }
        }
        
        private void OnGameOver()
        {
            ShowGameOver();
            UpdateGameOverUI();
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.GameOver)
            {
                ShowGameOver();
                UpdateGameOverUI();
            }
            else
            {
                HideGameOver();
            }
        }
        
        private void UpdateGameOverUI()
        {
            if (GameManager.Instance == null) return;
            
            int finalScore = GameManager.Instance.CurrentScore;
            int highScore = GameManager.Instance.HighScore;
            bool isNewHighScore = finalScore == highScore && finalScore > 0;
            
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {finalScore}";
            }
            
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {highScore}";
            }
            
            if (newHighScoreText != null)
            {
                newHighScoreText.gameObject.SetActive(isNewHighScore);
            }
        }
        
        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
        
        private void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
    }
}
