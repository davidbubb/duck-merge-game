using UnityEngine;
using UnityEngine.UI;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the settings menu UI
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button signOutButton;
        [SerializeField] private Text playerNameText;
        
        private void Start()
        {
            // Setup button listeners
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideSettings);
            }
            
            if (signInButton != null)
            {
                signInButton.onClick.AddListener(OnSignInClicked);
            }
            
            if (signOutButton != null)
            {
                signOutButton.onClick.AddListener(OnSignOutClicked);
            }
            
            // Setup toggle listeners
            if (musicToggle != null)
            {
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
            }
            
            // Setup slider listeners
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
            
            // Load current settings
            LoadSettings();
            UpdateGooglePlayUI();
            
            // Subscribe to Google Play auth changes
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.OnAuthenticationChanged += OnAuthenticationChanged;
            }
            
            // Hide by default
            HideSettings();
        }
        
        private void OnDestroy()
        {
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.OnAuthenticationChanged -= OnAuthenticationChanged;
            }
        }
        
        /// <summary>
        /// Show settings panel
        /// </summary>
        public void ShowSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
            
            LoadSettings();
            UpdateGooglePlayUI();
        }
        
        /// <summary>
        /// Hide settings panel
        /// </summary>
        public void HideSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Load current settings from AudioManager
        /// </summary>
        private void LoadSettings()
        {
            if (AudioManager.Instance == null) return;
            
            if (musicToggle != null)
            {
                musicToggle.isOn = AudioManager.Instance.IsMusicEnabled;
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.isOn = AudioManager.Instance.IsSFXEnabled;
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
            }
        }
        
        /// <summary>
        /// Update Google Play UI elements
        /// </summary>
        private void UpdateGooglePlayUI()
        {
            if (GooglePlayManager.Instance == null) return;
            
            bool isSignedIn = GooglePlayManager.Instance.IsAuthenticated;
            
            if (signInButton != null)
            {
                signInButton.gameObject.SetActive(!isSignedIn);
            }
            
            if (signOutButton != null)
            {
                signOutButton.gameObject.SetActive(isSignedIn);
            }
            
            if (playerNameText != null)
            {
                if (isSignedIn)
                {
                    playerNameText.text = GooglePlayManager.Instance.PlayerName;
                }
                else
                {
                    playerNameText.text = "Guest";
                }
            }
        }
        
        private void OnMusicToggleChanged(bool enabled)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleMusic(enabled);
            }
        }
        
        private void OnSFXToggleChanged(bool enabled)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ToggleSFX(enabled);
            }
        }
        
        private void OnMusicVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(volume);
            }
        }
        
        private void OnSFXVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(volume);
            }
        }
        
        private void OnSignInClicked()
        {
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.SignIn();
            }
        }
        
        private void OnSignOutClicked()
        {
            if (GooglePlayManager.Instance != null)
            {
                GooglePlayManager.Instance.SignOut();
            }
        }
        
        private void OnAuthenticationChanged(bool isAuthenticated)
        {
            UpdateGooglePlayUI();
        }
    }
}
