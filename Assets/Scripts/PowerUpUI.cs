using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DuckMergeGame
{
    /// <summary>
    /// UI for in-game power-up buttons and Timed mode countdown
    /// </summary>
    public class PowerUpUI : MonoBehaviour
    {
        [Header("Power-Up Buttons")]
        [SerializeField] private Button bombButton;
        [SerializeField] private Button downgradeButton;
        [SerializeField] private Button shuffleButton;

        [Header("Charge Labels")]
        [SerializeField] private TextMeshProUGUI bombChargesText;
        [SerializeField] private TextMeshProUGUI downgradeChargesText;
        [SerializeField] private TextMeshProUGUI shuffleChargesText;

        [Header("Timed Mode Timer")]
        [SerializeField] private GameObject timerPanel;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Panel")]
        [SerializeField] private GameObject powerUpPanel;

        private void Start()
        {
            if (bombButton != null)
                bombButton.onClick.AddListener(OnBombClicked);

            if (downgradeButton != null)
                downgradeButton.onClick.AddListener(OnDowngradeClicked);

            if (shuffleButton != null)
                shuffleButton.onClick.AddListener(OnShuffleClicked);

            if (PowerUpManager.Instance != null)
                PowerUpManager.Instance.OnChargesChanged += RefreshChargeLabel;

            if (GameModeManager.Instance != null)
            {
                GameModeManager.Instance.OnTimerUpdated += UpdateTimer;
                GameModeManager.Instance.OnGameModeChanged += OnGameModeChanged;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += OnGameStateChanged;

            // Initialize charge labels
            RefreshAllChargeLabels();

            if (powerUpPanel != null) powerUpPanel.SetActive(false);
            if (timerPanel != null)   timerPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (PowerUpManager.Instance != null)
                PowerUpManager.Instance.OnChargesChanged -= RefreshChargeLabel;

            if (GameModeManager.Instance != null)
            {
                GameModeManager.Instance.OnTimerUpdated   -= UpdateTimer;
                GameModeManager.Instance.OnGameModeChanged -= OnGameModeChanged;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }

        // ── Button handlers ────────────────────────────────────────────────

        private void OnBombClicked()
        {
            if (PowerUpManager.Instance != null)
                PowerUpManager.Instance.UsePowerUp(PowerUpType.Bomb);
        }

        private void OnDowngradeClicked()
        {
            if (PowerUpManager.Instance != null)
                PowerUpManager.Instance.UsePowerUp(PowerUpType.Downgrade);
        }

        private void OnShuffleClicked()
        {
            if (PowerUpManager.Instance != null)
                PowerUpManager.Instance.UsePowerUp(PowerUpType.Shuffle);
        }

        // ── Charge display ─────────────────────────────────────────────────

        private void RefreshChargeLabel(PowerUpType type, int charges)
        {
            switch (type)
            {
                case PowerUpType.Bomb:
                    SetChargeText(bombChargesText, charges);
                    if (bombButton != null) bombButton.interactable = charges > 0;
                    break;
                case PowerUpType.Downgrade:
                    SetChargeText(downgradeChargesText, charges);
                    if (downgradeButton != null) downgradeButton.interactable = charges > 0;
                    break;
                case PowerUpType.Shuffle:
                    SetChargeText(shuffleChargesText, charges);
                    if (shuffleButton != null) shuffleButton.interactable = charges > 0;
                    break;
            }
        }

        private void RefreshAllChargeLabels()
        {
            if (PowerUpManager.Instance == null) return;
            RefreshChargeLabel(PowerUpType.Bomb,      PowerUpManager.Instance.GetCharges(PowerUpType.Bomb));
            RefreshChargeLabel(PowerUpType.Downgrade, PowerUpManager.Instance.GetCharges(PowerUpType.Downgrade));
            RefreshChargeLabel(PowerUpType.Shuffle,   PowerUpManager.Instance.GetCharges(PowerUpType.Shuffle));
        }

        private void SetChargeText(TextMeshProUGUI label, int charges)
        {
            if (label != null) label.text = charges.ToString();
        }

        // ── Timer display (Timed mode) ──────────────────────────────────────

        private void UpdateTimer(float seconds)
        {
            if (timerText == null) return;
            int s = Mathf.CeilToInt(seconds);
            timerText.text = $"{s}s";
        }

        private void OnGameModeChanged(GameMode mode)
        {
            if (timerPanel != null)
                timerPanel.SetActive(mode == GameMode.Timed);
        }

        // ── State visibility ───────────────────────────────────────────────

        private void OnGameStateChanged(GameState state)
        {
            bool isPlaying = state == GameState.Playing;
            if (powerUpPanel != null) powerUpPanel.SetActive(isPlaying);

            bool isTimed = GameModeManager.Instance != null &&
                           GameModeManager.Instance.CurrentMode == GameMode.Timed;
            if (timerPanel != null) timerPanel.SetActive(isPlaying && isTimed);

            if (isPlaying) RefreshAllChargeLabels();
        }
    }
}
