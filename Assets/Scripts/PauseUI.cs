using UnityEngine;
using UnityEngine.UI;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the pause menu UI
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        
        private void Start()
        {
            // Setup button listeners
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeClicked);
            }
            
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
            
            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuClicked);
            }
            
            // Subscribe to events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
            
            // Hide by default
            HidePause();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnResumeClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }
        
        private void OnRestartClicked()
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
        
        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Paused)
            {
                ShowPause();
            }
            else
            {
                HidePause();
            }
        }
        
        private void ShowPause()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }
        
        private void HidePause()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }
}
