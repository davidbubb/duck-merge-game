using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

        [Header("Phase 2 – Leaderboard Rank")]
        [SerializeField] private TextMeshProUGUI leaderboardRankText;

        [Header("Phase 2 – Achievements Unlocked This Game")]
        [SerializeField] private GameObject newAchievementsPanel;
        [SerializeField] private TextMeshProUGUI newAchievementsText;

        // Achievements unlocked since the last game start
        private List<string> newlyUnlockedAchievements = new List<string>();
        
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

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked += OnAchievementUnlocked;
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

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked -= OnAchievementUnlocked;
            }
        }
        
        private void OnRetryClicked()
        {
            newlyUnlockedAchievements.Clear();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }
        
        private void OnMenuClicked()
        {
            newlyUnlockedAchievements.Clear();
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
                if (newState == GameState.Playing)
                    newlyUnlockedAchievements.Clear();
            }
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            // Only track achievements unlocked while a game is in progress
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                newlyUnlockedAchievements.Add(achievement.title);
            }
        }
        
        private void UpdateGameOverUI()
        {
            if (GameManager.Instance == null) return;
            
            int finalScore = GameManager.Instance.CurrentScore;
            int highScore  = GameManager.Instance.HighScore;
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

            // Leaderboard rank
            if (leaderboardRankText != null)
            {
                int rank = LeaderboardManager.Instance != null
                    ? LeaderboardManager.Instance.GetRankForScore(finalScore)
                    : -1;

                leaderboardRankText.gameObject.SetActive(rank > 0);
                if (rank > 0)
                    leaderboardRankText.text = $"Leaderboard Rank: #{rank}";
            }

            // Show newly unlocked achievements
            if (newAchievementsPanel != null)
            {
                bool hasNew = newlyUnlockedAchievements.Count > 0;
                newAchievementsPanel.SetActive(hasNew);
                if (hasNew && newAchievementsText != null)
                {
                    newAchievementsText.text = "Achievements unlocked:\n" +
                                              string.Join("\n", newlyUnlockedAchievements);
                }
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
