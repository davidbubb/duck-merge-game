using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// UI for selecting a game mode before starting a game
    /// </summary>
    public class GameModeUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject gameModePanel;

        [Header("Mode Buttons")]
        [SerializeField] private Button classicButton;
        [SerializeField] private Button timedButton;
        [SerializeField] private Button zenButton;

        [Header("Selected Mode Label")]
        [SerializeField] private TextMeshProUGUI selectedModeText;

        [Header("Confirm / Back Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;

        private GameMode pendingMode;

        private void Start()
        {
            if (classicButton != null) classicButton.onClick.AddListener(() => SelectMode(GameMode.Classic));
            if (timedButton   != null) timedButton.onClick.AddListener(() => SelectMode(GameMode.Timed));
            if (zenButton     != null) zenButton.onClick.AddListener(() => SelectMode(GameMode.Zen));

            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
            if (backButton    != null) backButton.onClick.AddListener(Hide);

            // Reflect the currently persisted mode
            if (GameModeManager.Instance != null)
                pendingMode = GameModeManager.Instance.CurrentMode;

            RefreshModeLabel();

            if (gameModePanel != null)
                gameModePanel.SetActive(false);
        }

        /// <summary>
        /// Show the game mode selection panel
        /// </summary>
        public void Show()
        {
            if (gameModePanel != null)
                gameModePanel.SetActive(true);

            if (GameModeManager.Instance != null)
                pendingMode = GameModeManager.Instance.CurrentMode;

            RefreshModeLabel();
        }

        public void Hide()
        {
            if (gameModePanel != null)
                gameModePanel.SetActive(false);
        }

        private void SelectMode(GameMode mode)
        {
            pendingMode = mode;
            RefreshModeLabel();
        }

        private void OnConfirm()
        {
            if (GameModeManager.Instance != null)
                GameModeManager.Instance.SelectMode(pendingMode);

            Hide();

            // Start the game
            if (GameManager.Instance != null)
                GameManager.Instance.StartNewGame();
        }

        private void RefreshModeLabel()
        {
            if (selectedModeText == null) return;

            switch (pendingMode)
            {
                case GameMode.Classic:
                    selectedModeText.text = "Classic – play until game over";
                    break;
                case GameMode.Timed:
                    selectedModeText.text = "Timed – score as high as possible in 60 seconds";
                    break;
                case GameMode.Zen:
                    selectedModeText.text = "Zen – no game over, play at your own pace";
                    break;
            }
        }
    }
}
