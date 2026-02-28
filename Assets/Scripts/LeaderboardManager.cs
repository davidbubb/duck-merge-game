using UnityEngine;
using System;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// A single leaderboard entry
    /// </summary>
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public string date;

        public LeaderboardEntry(string playerName, int score, string date)
        {
            this.playerName = playerName;
            this.score = score;
            this.date = date;
        }
    }

    /// <summary>
    /// Manages local and cloud leaderboards
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        // Fired when the local leaderboard is updated
        public event Action<List<LeaderboardEntry>> OnLeaderboardUpdated;

        [SerializeField] private int maxLocalEntries = 10;

        private List<LeaderboardEntry> localLeaderboard = new List<LeaderboardEntry>();

        private const string LEADERBOARD_COUNT_KEY = "Leaderboard_Count";
        private const string LEADERBOARD_ENTRY_PREFIX = "Leaderboard_Entry_";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadLocalLeaderboard();
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
        /// Submit a score to the local (and optionally cloud) leaderboard
        /// </summary>
        public void SubmitScore(int score)
        {
            if (score <= 0) return;

            string playerName = "You";
            if (GooglePlayManager.Instance != null && GooglePlayManager.Instance.IsAuthenticated)
            {
                playerName = GooglePlayManager.Instance.PlayerName;
            }

            string date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var entry = new LeaderboardEntry(playerName, score, date);

            // Insert in sorted order
            localLeaderboard.Add(entry);
            localLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));

            // Keep only the top N entries
            if (localLeaderboard.Count > maxLocalEntries)
            {
                localLeaderboard.RemoveRange(maxLocalEntries, localLeaderboard.Count - maxLocalEntries);
            }

            SaveLocalLeaderboard();
            OnLeaderboardUpdated?.Invoke(localLeaderboard);

            // Also report to cloud leaderboard via GooglePlayManager
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.SubmitLeaderboardScore(score);
            }

            Debug.Log($"Leaderboard: submitted score {score} for {playerName}");
        }

        /// <summary>
        /// Returns a copy of the local leaderboard sorted by score descending
        /// </summary>
        public List<LeaderboardEntry> GetLocalLeaderboard()
        {
            return new List<LeaderboardEntry>(localLeaderboard);
        }

        /// <summary>
        /// Returns the local rank (1-based) of a given score, or -1 if not on the board
        /// </summary>
        public int GetRankForScore(int score)
        {
            for (int i = 0; i < localLeaderboard.Count; i++)
            {
                if (localLeaderboard[i].score == score)
                    return i + 1;
            }
            return -1;
        }

        private void OnGameOver()
        {
            if (GameManager.Instance != null)
            {
                SubmitScore(GameManager.Instance.CurrentScore);
            }
        }

        private void LoadLocalLeaderboard()
        {
            localLeaderboard.Clear();
            int count = PlayerPrefs.GetInt(LEADERBOARD_COUNT_KEY, 0);

            for (int i = 0; i < count; i++)
            {
                string key = LEADERBOARD_ENTRY_PREFIX + i;
                string json = PlayerPrefs.GetString(key, "");
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                        if (entry != null) localLeaderboard.Add(entry);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"LeaderboardManager: Failed to parse entry {i}: {e.Message}");
                    }
                }
            }

            // Ensure sorted order after load
            localLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));
        }

        private void SaveLocalLeaderboard()
        {
            PlayerPrefs.SetInt(LEADERBOARD_COUNT_KEY, localLeaderboard.Count);
            for (int i = 0; i < localLeaderboard.Count; i++)
            {
                string key = LEADERBOARD_ENTRY_PREFIX + i;
                PlayerPrefs.SetString(key, JsonUtility.ToJson(localLeaderboard[i]));
            }
            PlayerPrefs.Save();
        }
    }
}
