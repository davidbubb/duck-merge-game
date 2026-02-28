using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// Displays the player's achievements list
    /// </summary>
    public class AchievementsUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject achievementsPanel;

        [Header("Achievement Row Prefab")]
        [SerializeField] private GameObject achievementRowPrefab;
        [SerializeField] private Transform  achievementListContent;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        [Header("Toast Notification")]
        [SerializeField] private GameObject toastPanel;
        [SerializeField] private TextMeshProUGUI toastText;
        [SerializeField] private float toastDuration = 3f;
        private float toastTimer = 0f;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (AchievementManager.Instance != null)
                AchievementManager.Instance.OnAchievementUnlocked += ShowToast;

            // Start hidden
            if (achievementsPanel != null)
                achievementsPanel.SetActive(false);

            if (toastPanel != null)
                toastPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (AchievementManager.Instance != null)
                AchievementManager.Instance.OnAchievementUnlocked -= ShowToast;
        }

        private void Update()
        {
            if (toastTimer > 0f)
            {
                toastTimer -= Time.unscaledDeltaTime;
                if (toastTimer <= 0f && toastPanel != null)
                    toastPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Show the achievements panel and populate the list
        /// </summary>
        public void Show()
        {
            if (achievementsPanel != null)
                achievementsPanel.SetActive(true);

            PopulateList();
        }

        public void Hide()
        {
            if (achievementsPanel != null)
                achievementsPanel.SetActive(false);
        }

        private void PopulateList()
        {
            if (achievementListContent == null || achievementRowPrefab == null) return;
            if (AchievementManager.Instance == null) return;

            // Clear existing rows
            foreach (Transform child in achievementListContent)
                Destroy(child.gameObject);

            foreach (Achievement a in AchievementManager.Instance.GetAllAchievements())
            {
                GameObject row = Instantiate(achievementRowPrefab, achievementListContent);

                // Attempt to fill title / description / locked state using TextMeshPro labels by name
                TextMeshProUGUI[] labels = row.GetComponentsInChildren<TextMeshProUGUI>(true);
                if (labels.Length >= 1) labels[0].text = a.title;
                if (labels.Length >= 2) labels[1].text = a.description;

                // Dim locked achievements
                CanvasGroup cg = row.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = a.isUnlocked ? 1f : 0.45f;
            }
        }

        /// <summary>
        /// Show a brief toast notification when an achievement is unlocked
        /// </summary>
        private void ShowToast(Achievement achievement)
        {
            if (toastPanel == null || toastText == null) return;

            toastText.text = $"Achievement Unlocked!\n{achievement.title}";
            toastPanel.SetActive(true);
            toastTimer = toastDuration;
        }
    }
}
