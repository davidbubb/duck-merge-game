using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the in-game UI during gameplay
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject gamePanel;
        
        private void Start()
        {
            // Setup button listeners
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }
            
            // Subscribe to events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnHighScoreChanged += UpdateHighScore;
                
                UpdateScore(GameManager.Instance.CurrentScore);
                UpdateHighScore(GameManager.Instance.HighScore);
            }
            
            // Hide by default
            HideGameUI();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnScoreChanged -= UpdateScore;
                GameManager.Instance.OnHighScoreChanged -= UpdateHighScore;
            }
        }
        
        private void OnPauseClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }
        
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }
        
        private void UpdateHighScore(int score)
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"Best: {score}";
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
            {
                ShowGameUI();
            }
            else
            {
                HideGameUI();
            }
        }
        
        private void ShowGameUI()
        {
            if (gamePanel != null)
            {
                gamePanel.SetActive(true);
            }
        }
        
        private void HideGameUI()
        {
            if (gamePanel != null)
            {
                gamePanel.SetActive(false);
            }
        }
    }
}
