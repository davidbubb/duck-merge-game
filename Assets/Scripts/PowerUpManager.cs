using UnityEngine;
using System;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// Available power-up types
    /// </summary>
    public enum PowerUpType
    {
        Bomb,       // Remove a specific bird from the container
        Downgrade,  // Reduce a random bird's tier by one
        Shuffle     // Randomly reposition all active birds
    }

    /// <summary>
    /// Manages the power-up system: charges, usage and effects
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }

        // Fired when charges change for any power-up type
        public event Action<PowerUpType, int> OnChargesChanged;

        // Fired when a power-up is activated
        public event Action<PowerUpType> OnPowerUpActivated;

        [Header("Starting Charges")]
        [SerializeField] private int bombStartCharges    = 1;
        [SerializeField] private int downgradeStartCharges = 1;
        [SerializeField] private int shuffleStartCharges = 1;

        [Header("Max Charges Per Type")]
        [SerializeField] private int maxCharges = 3;

        private Dictionary<PowerUpType, int> charges = new Dictionary<PowerUpType, int>();

        // PlayerPrefs keys
        private const string PREF_PREFIX = "PowerUp_Charges_";

        public int GetCharges(PowerUpType type) => charges.TryGetValue(type, out int c) ? c : 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadCharges();
        }

        /// <summary>
        /// Attempt to use a power-up; returns false if no charges remain
        /// </summary>
        public bool UsePowerUp(PowerUpType type)
        {
            if (GetCharges(type) <= 0)
            {
                Debug.Log($"PowerUp {type}: no charges remaining");
                return false;
            }

            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            {
                Debug.Log($"PowerUp {type}: can only be used while playing");
                return false;
            }

            charges[type]--;
            SaveCharges();
            OnChargesChanged?.Invoke(type, charges[type]);

            ApplyPowerUp(type);

            // Report to achievement manager
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnPowerUpUsed();
            }

            OnPowerUpActivated?.Invoke(type);
            Debug.Log($"PowerUp used: {type}. Remaining charges: {charges[type]}");
            return true;
        }

        /// <summary>
        /// Grant extra charges to a power-up (e.g. from a reward)
        /// </summary>
        public void AddCharges(PowerUpType type, int amount)
        {
            charges[type] = Mathf.Min(charges[type] + amount, maxCharges);
            SaveCharges();
            OnChargesChanged?.Invoke(type, charges[type]);
        }

        private void ApplyPowerUp(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.Bomb:
                    ApplyBomb();
                    break;
                case PowerUpType.Downgrade:
                    ApplyDowngrade();
                    break;
                case PowerUpType.Shuffle:
                    ApplyShuffle();
                    break;
            }
        }

        /// <summary>
        /// Bomb: destroy the highest-tier bird currently in the container
        /// </summary>
        private void ApplyBomb()
        {
            Bird[] birds = FindObjectsOfType<Bird>();
            Bird target = null;
            int highestTier = 0;

            foreach (Bird bird in birds)
            {
                if (!bird.IsMerging && bird.Tier > highestTier)
                {
                    highestTier = bird.Tier;
                    target = bird;
                }
            }

            if (target != null)
            {
                Debug.Log($"Bomb: destroying Tier {target.Tier} bird");
                target.DestroySelf();
            }
            else
            {
                Debug.Log("Bomb: no eligible bird to destroy");
            }
        }

        /// <summary>
        /// Downgrade: reduce the highest-tier non-merging bird's tier by one
        /// </summary>
        private void ApplyDowngrade()
        {
            Bird[] birds = FindObjectsOfType<Bird>();
            Bird target = null;
            int highestTier = 0;

            foreach (Bird bird in birds)
            {
                if (!bird.IsMerging && bird.Tier > 1 && bird.Tier > highestTier)
                {
                    highestTier = bird.Tier;
                    target = bird;
                }
            }

            if (target != null)
            {
                GameConfig config = GameManager.Instance != null ? GameManager.Instance.Config : null;
                if (config != null)
                {
                    target.Initialize(target.Tier - 1, config);
                    Debug.Log($"Downgrade: reduced bird to Tier {target.Tier}");
                }
            }
            else
            {
                Debug.Log("Downgrade: no eligible bird to downgrade");
            }
        }

        /// <summary>
        /// Shuffle: randomize the X positions of all active birds
        /// </summary>
        private void ApplyShuffle()
        {
            Bird[] birds = FindObjectsOfType<Bird>();
            GameConfig config = GameManager.Instance != null ? GameManager.Instance.Config : null;
            float halfWidth = config != null ? config.containerWidth / 2f - 0.5f : 2.5f;

            foreach (Bird bird in birds)
            {
                if (!bird.IsMerging)
                {
                    Vector3 pos = bird.transform.position;
                    pos.x = UnityEngine.Random.Range(-halfWidth, halfWidth);
                    bird.transform.position = pos;
                }
            }

            Debug.Log($"Shuffle: repositioned {birds.Length} birds");
        }

        private void LoadCharges()
        {
            charges[PowerUpType.Bomb]      = PlayerPrefs.GetInt(PREF_PREFIX + "Bomb",      bombStartCharges);
            charges[PowerUpType.Downgrade] = PlayerPrefs.GetInt(PREF_PREFIX + "Downgrade", downgradeStartCharges);
            charges[PowerUpType.Shuffle]   = PlayerPrefs.GetInt(PREF_PREFIX + "Shuffle",   shuffleStartCharges);
        }

        private void SaveCharges()
        {
            PlayerPrefs.SetInt(PREF_PREFIX + "Bomb",      charges[PowerUpType.Bomb]);
            PlayerPrefs.SetInt(PREF_PREFIX + "Downgrade", charges[PowerUpType.Downgrade]);
            PlayerPrefs.SetInt(PREF_PREFIX + "Shuffle",   charges[PowerUpType.Shuffle]);
            PlayerPrefs.Save();
        }
    }
}
