using UnityEngine;
using System;

namespace DuckMergeGame
{
    /// <summary>
    /// Defines the objective type for a daily challenge
    /// </summary>
    public enum ChallengeType
    {
        ReachScore,         // Reach a target score in one game
        PerformMerges,      // Perform N merges in one game
        CreateTier,         // Create a bird of the specified tier
        SurviveSeconds      // Stay alive for N seconds
    }

    /// <summary>
    /// Represents a single daily challenge
    /// </summary>
    [System.Serializable]
    public class DailyChallenge
    {
        public ChallengeType type;
        public int targetValue;
        public string description;
        public bool isCompleted;
        public int currentProgress;

        public DailyChallenge(ChallengeType type, int targetValue, string description)
        {
            this.type = type;
            this.targetValue = targetValue;
            this.description = description;
            this.isCompleted = false;
            this.currentProgress = 0;
        }

        /// <summary>
        /// Returns a 0-1 float representing completion progress
        /// </summary>
        public float Progress => isCompleted ? 1f : Mathf.Clamp01((float)currentProgress / targetValue);
    }

    /// <summary>
    /// Manages daily challenges that rotate each calendar day
    /// </summary>
    public class DailyChallengeManager : MonoBehaviour
    {
        public static DailyChallengeManager Instance { get; private set; }

        // Fired when challenge progress changes
        public event Action<DailyChallenge> OnChallengeProgressUpdated;

        // Fired when the current daily challenge is completed
        public event Action<DailyChallenge> OnChallengeCompleted;

        private DailyChallenge currentChallenge;

        // PlayerPrefs keys
        private const string CHALLENGE_DATE_KEY    = "DailyChallengeDate";
        private const string CHALLENGE_TYPE_KEY    = "DailyChallengeType";
        private const string CHALLENGE_TARGET_KEY  = "DailyChallengeTarget";
        private const string CHALLENGE_PROGRESS_KEY = "DailyChallengeProgress";
        private const string CHALLENGE_DONE_KEY    = "DailyChallengeCompleted";

        // All possible challenge templates (type, target)
        private static readonly (ChallengeType type, int target)[] ChallengeTemplates =
        {
            (ChallengeType.ReachScore,       500),
            (ChallengeType.ReachScore,      1000),
            (ChallengeType.ReachScore,      2500),
            (ChallengeType.PerformMerges,      5),
            (ChallengeType.PerformMerges,     15),
            (ChallengeType.PerformMerges,     30),
            (ChallengeType.CreateTier,         5),
            (ChallengeType.CreateTier,         7),
            (ChallengeType.CreateTier,         9),
            (ChallengeType.SurviveSeconds,    60),
            (ChallengeType.SurviveSeconds,   120),
            (ChallengeType.SurviveSeconds,   180),
        };

        public DailyChallenge CurrentChallenge => currentChallenge;

        // Survival timer
        private float survivalTimer = 0f;
        private bool trackingSurvival = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            RefreshChallenge();
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
            if (trackingSurvival && currentChallenge != null &&
                currentChallenge.type == ChallengeType.SurviveSeconds &&
                !currentChallenge.isCompleted)
            {
                survivalTimer += Time.deltaTime;
                int seconds = Mathf.FloorToInt(survivalTimer);
                UpdateProgress(seconds);
            }
        }

        /// <summary>
        /// Generate or reload today's challenge
        /// </summary>
        private void RefreshChallenge()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string savedDate = PlayerPrefs.GetString(CHALLENGE_DATE_KEY, "");

            if (savedDate == today)
            {
                // Load existing challenge for today
                ChallengeType savedType     = (ChallengeType)PlayerPrefs.GetInt(CHALLENGE_TYPE_KEY, 0);
                int           savedTarget   = PlayerPrefs.GetInt(CHALLENGE_TARGET_KEY, 0);
                int           savedProgress = PlayerPrefs.GetInt(CHALLENGE_PROGRESS_KEY, 0);
                bool          savedDone     = PlayerPrefs.GetInt(CHALLENGE_DONE_KEY, 0) == 1;

                currentChallenge = new DailyChallenge(savedType, savedTarget, BuildDescription(savedType, savedTarget))
                {
                    currentProgress = savedProgress,
                    isCompleted     = savedDone
                };
            }
            else
            {
                // New day - generate a new challenge deterministically from the date
                int dayIndex = DaysSinceEpoch(DateTime.UtcNow);
                var template = ChallengeTemplates[dayIndex % ChallengeTemplates.Length];
                currentChallenge = new DailyChallenge(template.type, template.target, BuildDescription(template.type, template.target));

                // Persist
                PlayerPrefs.SetString(CHALLENGE_DATE_KEY,     today);
                PlayerPrefs.SetInt(CHALLENGE_TYPE_KEY,        (int)template.type);
                PlayerPrefs.SetInt(CHALLENGE_TARGET_KEY,      template.target);
                PlayerPrefs.SetInt(CHALLENGE_PROGRESS_KEY,    0);
                PlayerPrefs.SetInt(CHALLENGE_DONE_KEY,        0);
                PlayerPrefs.Save();
            }

            Debug.Log($"Daily challenge: {currentChallenge.description}");
        }

        private string BuildDescription(ChallengeType type, int target)
        {
            switch (type)
            {
                case ChallengeType.ReachScore:      return $"Reach a score of {target} in one game";
                case ChallengeType.PerformMerges:   return $"Perform {target} merges in one game";
                case ChallengeType.CreateTier:      return $"Create a Tier {target} bird";
                case ChallengeType.SurviveSeconds:  return $"Survive for {target} seconds";
                default:                            return "Complete the challenge";
            }
        }

        /// <summary>
        /// Report merge event to the daily challenge
        /// </summary>
        public void OnMergePerformed(int resultTier)
        {
            if (currentChallenge == null || currentChallenge.isCompleted) return;

            switch (currentChallenge.type)
            {
                case ChallengeType.PerformMerges:
                    UpdateProgress(currentChallenge.currentProgress + 1);
                    break;
                case ChallengeType.CreateTier:
                    if (resultTier >= currentChallenge.targetValue)
                        UpdateProgress(currentChallenge.targetValue);
                    break;
            }
        }

        /// <summary>
        /// Report score update to the daily challenge
        /// </summary>
        public void OnScoreChanged(int score)
        {
            if (currentChallenge == null || currentChallenge.isCompleted) return;

            if (currentChallenge.type == ChallengeType.ReachScore)
            {
                UpdateProgress(score);
            }
        }

        private void UpdateProgress(int newProgress)
        {
            if (currentChallenge == null || currentChallenge.isCompleted) return;

            currentChallenge.currentProgress = newProgress;
            PlayerPrefs.SetInt(CHALLENGE_PROGRESS_KEY, newProgress);
            PlayerPrefs.Save();

            OnChallengeProgressUpdated?.Invoke(currentChallenge);

            if (newProgress >= currentChallenge.targetValue)
            {
                CompleteChallenge();
            }
        }

        private void CompleteChallenge()
        {
            if (currentChallenge == null || currentChallenge.isCompleted) return;

            currentChallenge.isCompleted = true;
            currentChallenge.currentProgress = currentChallenge.targetValue;
            PlayerPrefs.SetInt(CHALLENGE_DONE_KEY, 1);
            PlayerPrefs.Save();

            OnChallengeCompleted?.Invoke(currentChallenge);
            Debug.Log($"Daily challenge completed: {currentChallenge.description}");

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnDailyChallengeCompleted();
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                // Reset per-game counters for per-game challenge types
                if (currentChallenge != null &&
                    (currentChallenge.type == ChallengeType.PerformMerges ||
                     currentChallenge.type == ChallengeType.ReachScore))
                {
                    // Don't reset - allow accumulation across attempts
                }

                survivalTimer = 0f;
                trackingSurvival = currentChallenge != null &&
                                   currentChallenge.type == ChallengeType.SurviveSeconds &&
                                   !currentChallenge.isCompleted;
            }
            else
            {
                trackingSurvival = false;
            }
        }

        private static int DaysSinceEpoch(DateTime date)
        {
            return (int)(date.Date - new DateTime(2026, 1, 1)).TotalDays;
        }
    }
}
