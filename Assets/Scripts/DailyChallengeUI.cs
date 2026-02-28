using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// Displays today's daily challenge and tracks its progress in the UI
    /// </summary>
    public class DailyChallengeUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject challengePanel;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI completedBadgeText;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnChallengeProgressUpdated += RefreshUI;
                DailyChallengeManager.Instance.OnChallengeCompleted        += OnCompleted;
            }

            if (challengePanel != null)
                challengePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.OnChallengeProgressUpdated -= RefreshUI;
                DailyChallengeManager.Instance.OnChallengeCompleted        -= OnCompleted;
            }
        }

        /// <summary>
        /// Show the daily challenge panel
        /// </summary>
        public void Show()
        {
            if (challengePanel != null)
                challengePanel.SetActive(true);

            RefreshUI(DailyChallengeManager.Instance != null ? DailyChallengeManager.Instance.CurrentChallenge : null);
        }

        public void Hide()
        {
            if (challengePanel != null)
                challengePanel.SetActive(false);
        }

        private void RefreshUI(DailyChallenge challenge)
        {
            if (challenge == null) return;

            if (descriptionText != null)
                descriptionText.text = challenge.description;

            if (progressText != null)
                progressText.text = $"{challenge.currentProgress} / {challenge.targetValue}";

            if (progressBar != null)
                progressBar.value = challenge.Progress;

            if (completedBadgeText != null)
                completedBadgeText.gameObject.SetActive(challenge.isCompleted);
        }

        private void OnCompleted(DailyChallenge challenge)
        {
            RefreshUI(challenge);
        }
    }
}
