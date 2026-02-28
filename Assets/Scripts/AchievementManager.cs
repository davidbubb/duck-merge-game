using UnityEngine;
using System;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// Defines a single achievement with its unlock conditions and state
    /// </summary>
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool isUnlocked;

        public Achievement(string id, string title, string description)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.isUnlocked = false;
        }
    }

    /// <summary>
    /// Manages all achievements - tracking progress and unlocking rewards
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        // Fired when any achievement is unlocked; passes the unlocked Achievement
        public event Action<Achievement> OnAchievementUnlocked;

        private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

        // Cumulative stats tracked for achievements
        private int totalMerges = 0;
        private int highestTierCreated = 0;

        // PlayerPrefs key prefix
        private const string PREF_PREFIX = "Achievement_";
        private const string TOTAL_MERGES_KEY = "AchievementTotalMerges";
        private const string HIGHEST_TIER_KEY = "AchievementHighestTier";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            RegisterAchievements();
            LoadProgress();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += OnGameOver;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver -= OnGameOver;
            }
        }

        /// <summary>
        /// Define all achievements in the game
        /// </summary>
        private void RegisterAchievements()
        {
            AddAchievement("first_merge",    "First Merge",      "Perform your first bird merge");
            AddAchievement("tier_5",         "Wood Duck",        "Create a Wood Duck (Tier 5)");
            AddAchievement("tier_8",         "Canada Goose",     "Create a Canada Goose (Tier 8)");
            AddAchievement("tier_11",        "Swan Song",        "Create the majestic Swan (Tier 11)");
            AddAchievement("score_1000",     "Thousand Points",  "Reach a score of 1,000 in a single game");
            AddAchievement("score_5000",     "Five Thousand",    "Reach a score of 5,000 in a single game");
            AddAchievement("score_10000",    "Ten Thousand",     "Reach a score of 10,000 in a single game");
            AddAchievement("merge_10",       "Getting Warmed Up","Perform 10 merges total");
            AddAchievement("merge_50",       "Merge Expert",     "Perform 50 merges total");
            AddAchievement("merge_100",      "Merge Master",     "Perform 100 merges total");
            AddAchievement("daily_complete", "Daily Challenger", "Complete your first daily challenge");
            AddAchievement("powerup_use",    "Power Player",     "Use a power-up for the first time");
            AddAchievement("timed_mode",     "Against the Clock","Complete a game in Timed Mode");
            AddAchievement("zen_mode",       "Peaceful Quacker", "Play a game in Zen Mode");
        }

        private void AddAchievement(string id, string title, string description)
        {
            achievements[id] = new Achievement(id, title, description);
        }

        /// <summary>
        /// Called whenever a merge happens; updates cumulative stats and checks achievements
        /// </summary>
        public void OnMergePerformed(int resultTier)
        {
            totalMerges++;
            PlayerPrefs.SetInt(TOTAL_MERGES_KEY, totalMerges);

            if (resultTier > highestTierCreated)
            {
                highestTierCreated = resultTier;
                PlayerPrefs.SetInt(HIGHEST_TIER_KEY, highestTierCreated);
            }

            PlayerPrefs.Save();

            // Check merge-count achievements
            TryUnlock("first_merge");
            if (totalMerges >= 10)  TryUnlock("merge_10");
            if (totalMerges >= 50)  TryUnlock("merge_50");
            if (totalMerges >= 100) TryUnlock("merge_100");

            // Check tier achievements
            if (resultTier >= 5)  TryUnlock("tier_5");
            if (resultTier >= 8)  TryUnlock("tier_8");
            if (resultTier >= 11) TryUnlock("tier_11");
        }

        /// <summary>
        /// Called each time the score changes during a game; checks score achievements
        /// </summary>
        public void OnScoreReached(int score)
        {
            if (score >= 1000)  TryUnlock("score_1000");
            if (score >= 5000)  TryUnlock("score_5000");
            if (score >= 10000) TryUnlock("score_10000");
        }

        /// <summary>
        /// Unlock the achievement for completing a daily challenge
        /// </summary>
        public void OnDailyChallengeCompleted()
        {
            TryUnlock("daily_complete");
        }

        /// <summary>
        /// Unlock the achievement for using a power-up
        /// </summary>
        public void OnPowerUpUsed()
        {
            TryUnlock("powerup_use");
        }

        /// <summary>
        /// Unlock the achievement for finishing a timed-mode game
        /// </summary>
        public void OnTimedModeCompleted()
        {
            TryUnlock("timed_mode");
        }

        /// <summary>
        /// Unlock the achievement for playing a zen-mode game
        /// </summary>
        public void OnZenModePlayed()
        {
            TryUnlock("zen_mode");
        }

        /// <summary>
        /// Try to unlock an achievement by ID; fires the event only on first unlock
        /// </summary>
        public void TryUnlock(string achievementId)
        {
            if (!achievements.TryGetValue(achievementId, out Achievement achievement))
            {
                Debug.LogWarning($"AchievementManager: Unknown achievement '{achievementId}'");
                return;
            }

            if (achievement.isUnlocked) return;

            achievement.isUnlocked = true;
            PlayerPrefs.SetInt(PREF_PREFIX + achievementId, 1);
            PlayerPrefs.Save();

            OnAchievementUnlocked?.Invoke(achievement);
            Debug.Log($"Achievement unlocked: {achievement.title}");

            // Report to Google Play if signed in
            if (GooglePlayManager.Instance != null && GooglePlayManager.Instance.IsAuthenticated)
            {
                GooglePlayManager.Instance.ReportAchievement(achievementId);
            }
        }

        /// <summary>
        /// Returns all defined achievements (for the achievements UI)
        /// </summary>
        public IEnumerable<Achievement> GetAllAchievements()
        {
            return achievements.Values;
        }

        /// <summary>
        /// Returns only unlocked achievements
        /// </summary>
        public List<Achievement> GetUnlockedAchievements()
        {
            var unlocked = new List<Achievement>();
            foreach (var a in achievements.Values)
            {
                if (a.isUnlocked) unlocked.Add(a);
            }
            return unlocked;
        }

        private void OnGameOver()
        {
            // Persist any unsaved data
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load persisted achievement unlock states and cumulative stats
        /// </summary>
        private void LoadProgress()
        {
            totalMerges       = PlayerPrefs.GetInt(TOTAL_MERGES_KEY, 0);
            highestTierCreated = PlayerPrefs.GetInt(HIGHEST_TIER_KEY, 0);

            foreach (var achievement in achievements.Values)
            {
                achievement.isUnlocked = PlayerPrefs.GetInt(PREF_PREFIX + achievement.id, 0) == 1;
            }
        }
    }
}
