using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the main menu UI
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private GameObject mainMenuPanel;

        [Header("Phase 2 Buttons")]
        [SerializeField] private Button achievementsButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button dailyChallengeButton;
        [SerializeField] private Button gameModeButton;

        [Header("Phase 2 Panels")]
        [SerializeField] private AchievementsUI  achievementsUI;
        [SerializeField] private LeaderboardUI   leaderboardUI;
        [SerializeField] private DailyChallengeUI dailyChallengeUI;
        [SerializeField] private GameModeUI       gameModeUI;

        [Header("Daily Challenge Badge")]
        [SerializeField] private GameObject dailyChallengeBadge;
        
        private void Start()
        {
            // Setup button listeners
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }

            // Phase 2 buttons
            if (achievementsButton  != null) achievementsButton.onClick.AddListener(OnAchievementsClicked);
            if (leaderboardButton   != null) leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
            if (dailyChallengeButton != null) dailyChallengeButton.onClick.AddListener(OnDailyChallengeClicked);
            if (gameModeButton      != null) gameModeButton.onClick.AddListener(OnGameModeClicked);
            
            // Subscribe to game state changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnHighScoreChanged += UpdateHighScore;
                UpdateHighScore(GameManager.Instance.HighScore);
            }

            // Show new-challenge badge when applicable
            RefreshDailyChallengeBadge();
            
            // Show main menu
            ShowMainMenu();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameManager.Instance.OnHighScoreChanged -= UpdateHighScore;
            }
        }
        
        private void OnPlayClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }
        
        private void OnSettingsClicked()
        {
            // TODO: Open settings menu
            Debug.Log("Settings clicked");
        }
        
        private void OnQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }

        private void OnAchievementsClicked()
        {
            if (achievementsUI != null) achievementsUI.Show();
        }

        private void OnLeaderboardClicked()
        {
            if (leaderboardUI != null) leaderboardUI.Show();
        }

        private void OnDailyChallengeClicked()
        {
            if (dailyChallengeUI != null) dailyChallengeUI.Show();
        }

        private void OnGameModeClicked()
        {
            if (gameModeUI != null)
                gameModeUI.Show();
            else
                OnPlayClicked(); // Fallback: start Classic mode directly
        }
        
        private void UpdateHighScore(int score)
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {score}";
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.MainMenu)
            {
                ShowMainMenu();
                RefreshDailyChallengeBadge();
            }
            else
            {
                HideMainMenu();
            }
        }

        /// <summary>
        /// Show a badge dot on the Daily Challenge button when the challenge is not yet completed
        /// </summary>
        private void RefreshDailyChallengeBadge()
        {
            if (dailyChallengeBadge == null) return;
            bool hasIncomplete = DailyChallengeManager.Instance != null &&
                                 DailyChallengeManager.Instance.CurrentChallenge != null &&
                                 !DailyChallengeManager.Instance.CurrentChallenge.isCompleted;
            dailyChallengeBadge.SetActive(hasIncomplete);
        }
        
        private void ShowMainMenu()
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }
        }
        
        private void HideMainMenu()
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
            }
        }
    }
}

