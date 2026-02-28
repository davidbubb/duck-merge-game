using UnityEngine;
using System;

namespace DuckMergeGame
{
    /// <summary>
    /// Available game modes
    /// </summary>
    public enum GameMode
    {
        Classic,    // Standard mode - play until game over
        Timed,      // Score as many points as possible within a time limit
        Zen         // Relaxed mode - no game-over condition
    }

    /// <summary>
    /// Manages game mode selection and mode-specific logic
    /// </summary>
    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager Instance { get; private set; }

        // Fired when the active game mode changes
        public event Action<GameMode> OnGameModeChanged;

        // Fired every second with remaining time in Timed mode
        public event Action<float> OnTimerUpdated;

        // Fired when the Timed mode timer expires
        public event Action OnTimedModeExpired;

        [Header("Timed Mode Settings")]
        [SerializeField] private float timedModeDuration = 60f;

        private GameMode currentMode = GameMode.Classic;
        private float timerRemaining = 0f;
        private bool timerRunning = false;

        private const string SELECTED_MODE_KEY = "SelectedGameMode";

        public GameMode CurrentMode => currentMode;
        public float TimerRemaining => timerRemaining;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Restore last-used mode
            currentMode = (GameMode)PlayerPrefs.GetInt(SELECTED_MODE_KEY, (int)GameMode.Classic);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }

        private void Update()
        {
            if (!timerRunning) return;

            timerRemaining -= Time.deltaTime;
            OnTimerUpdated?.Invoke(timerRemaining);

            if (timerRemaining <= 0f)
            {
                timerRemaining = 0f;
                timerRunning = false;
                OnTimerUpdated?.Invoke(0f);
                OnTimedModeExpired?.Invoke();

                // In Timed mode, expiry triggers game over
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TriggerGameOver();
                }

                // Achievement for completing a Timed mode game
                if (AchievementManager.Instance != null)
                {
                    AchievementManager.Instance.OnTimedModeCompleted();
                }
            }
        }

        /// <summary>
        /// Select the game mode to use for the next game; persists the selection
        /// </summary>
        public void SelectMode(GameMode mode)
        {
            currentMode = mode;
            PlayerPrefs.SetInt(SELECTED_MODE_KEY, (int)mode);
            PlayerPrefs.Save();
            OnGameModeChanged?.Invoke(currentMode);
            Debug.Log($"Game mode selected: {mode}");
        }

        /// <summary>
        /// Returns true when the game-over condition should be suppressed (Zen mode)
        /// </summary>
        public bool IsGameOverSuppressed()
        {
            return currentMode == GameMode.Zen;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                timerRunning = false;
                timerRemaining = 0f;

                if (currentMode == GameMode.Timed)
                {
                    timerRemaining = timedModeDuration;
                    timerRunning = true;
                    OnTimerUpdated?.Invoke(timerRemaining);
                    Debug.Log($"Timed mode started: {timedModeDuration}s");
                }
                else if (currentMode == GameMode.Zen)
                {
                    Debug.Log("Zen mode started - no game over");

                    // Fire achievement for playing Zen mode
                    if (AchievementManager.Instance != null)
                    {
                        AchievementManager.Instance.OnZenModePlayed();
                    }
                }
            }
            else if (state == GameState.Paused)
            {
                timerRunning = false;
            }
            else if (state == GameState.GameOver || state == GameState.MainMenu)
            {
                timerRunning = false;
            }
        }
    }
}
