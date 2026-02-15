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
            
            // Subscribe to game state changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
                GameManager.Instance.OnHighScoreChanged += UpdateHighScore;
                UpdateHighScore(GameManager.Instance.HighScore);
            }
            
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
            }
            else
            {
                HideMainMenu();
            }
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
