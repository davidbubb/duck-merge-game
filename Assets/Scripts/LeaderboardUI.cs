using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// Displays the local leaderboard and allows showing the global (Google Play) leaderboard
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject leaderboardPanel;

        [Header("Entry Row Prefab")]
        [SerializeField] private GameObject entryRowPrefab;
        [SerializeField] private Transform  entryListContent;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button globalLeaderboardButton;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (globalLeaderboardButton != null)
                globalLeaderboardButton.onClick.AddListener(ShowGlobalLeaderboard);

            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.OnLeaderboardUpdated += OnLeaderboardUpdated;

            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.OnLeaderboardUpdated -= OnLeaderboardUpdated;
        }

        /// <summary>
        /// Show the leaderboard panel and populate the local list
        /// </summary>
        public void Show()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(true);

            if (LeaderboardManager.Instance != null)
                PopulateList(LeaderboardManager.Instance.GetLocalLeaderboard());
        }

        public void Hide()
        {
            if (leaderboardPanel != null)
                leaderboardPanel.SetActive(false);
        }

        private void OnLeaderboardUpdated(List<LeaderboardEntry> entries)
        {
            if (leaderboardPanel != null && leaderboardPanel.activeSelf)
                PopulateList(entries);
        }

        private void PopulateList(List<LeaderboardEntry> entries)
        {
            if (entryListContent == null || entryRowPrefab == null) return;

            foreach (Transform child in entryListContent)
                Destroy(child.gameObject);

            for (int i = 0; i < entries.Count; i++)
            {
                LeaderboardEntry entry = entries[i];
                GameObject row = Instantiate(entryRowPrefab, entryListContent);

                TextMeshProUGUI[] labels = row.GetComponentsInChildren<TextMeshProUGUI>(true);
                if (labels.Length >= 1) labels[0].text = $"{i + 1}.";
                if (labels.Length >= 2) labels[1].text = entry.playerName;
                if (labels.Length >= 3) labels[2].text = entry.score.ToString("N0");
                if (labels.Length >= 4) labels[3].text = entry.date;
            }
        }

        private void ShowGlobalLeaderboard()
        {
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.ShowLeaderboard();
            }
            else
            {
                Debug.Log("LeaderboardUI: Google Play not available");
            }
        }
    }
}
