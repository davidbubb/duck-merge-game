using UnityEngine;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages all audio in the game (music and sound effects)
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Music Clips")]
        [SerializeField] private AudioClip[] backgroundMusicTracks;
        private int currentMusicIndex = 0;
        
        [Header("SFX Clips")]
        [SerializeField] private AudioClip dropSound;
        [SerializeField] private AudioClip[] mergeSounds; // Different pitches for different tiers
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip swanSound; // Special sound for tier 11
        
        [Header("Settings")]
        private bool musicEnabled = true;
        private bool sfxEnabled = true;
        private float musicVolume = 0.7f;
        private float sfxVolume = 0.8f;
        
        private const string MUSIC_ENABLED_KEY = "MusicEnabled";
        private const string SFX_ENABLED_KEY = "SFXEnabled";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.parent = transform;
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.parent = transform;
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            LoadSettings();
            UpdateAudioSettings();
        }
        
        private void Start()
        {
            PlayBackgroundMusic();
        }
        
        private void Update()
        {
            // Check if music track ended and play next one
            if (musicEnabled && musicSource != null && !musicSource.isPlaying && backgroundMusicTracks != null && backgroundMusicTracks.Length > 0)
            {
                PlayNextMusicTrack();
            }
        }
        
        /// <summary>
        /// Play background music
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (!musicEnabled || musicSource == null || backgroundMusicTracks == null || backgroundMusicTracks.Length == 0)
                return;
            
            currentMusicIndex = 0;
            musicSource.clip = backgroundMusicTracks[currentMusicIndex];
            musicSource.Play();
        }
        
        /// <summary>
        /// Play next music track
        /// </summary>
        private void PlayNextMusicTrack()
        {
            if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0) return;
            
            currentMusicIndex = (currentMusicIndex + 1) % backgroundMusicTracks.Length;
            musicSource.clip = backgroundMusicTracks[currentMusicIndex];
            musicSource.Play();
        }
        
        /// <summary>
        /// Stop background music
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Play drop sound
        /// </summary>
        public void PlayDropSound()
        {
            if (sfxEnabled && sfxSource != null && dropSound != null)
            {
                sfxSource.PlayOneShot(dropSound);
            }
        }
        
        /// <summary>
        /// Play merge sound based on tier
        /// </summary>
        public void PlayMergeSound(int tier)
        {
            if (!sfxEnabled || sfxSource == null) return;
            
            // Play special swan sound for tier 11
            if (tier == 10 && swanSound != null)
            {
                sfxSource.PlayOneShot(swanSound);
                return;
            }
            
            // Play tier-appropriate merge sound
            if (mergeSounds != null && mergeSounds.Length > 0)
            {
                int soundIndex = Mathf.Clamp(tier - 1, 0, mergeSounds.Length - 1);
                if (mergeSounds[soundIndex] != null)
                {
                    sfxSource.PlayOneShot(mergeSounds[soundIndex]);
                }
            }
        }
        
        /// <summary>
        /// Play game over sound
        /// </summary>
        public void PlayGameOverSound()
        {
            if (sfxEnabled && sfxSource != null && gameOverSound != null)
            {
                sfxSource.PlayOneShot(gameOverSound);
            }
        }
        
        /// <summary>
        /// Play button click sound
        /// </summary>
        public void PlayButtonClick()
        {
            if (sfxEnabled && sfxSource != null && buttonClickSound != null)
            {
                sfxSource.PlayOneShot(buttonClickSound);
            }
        }
        
        /// <summary>
        /// Toggle music on/off
        /// </summary>
        public void ToggleMusic(bool enabled)
        {
            musicEnabled = enabled;
            UpdateAudioSettings();
            SaveSettings();
            
            if (musicEnabled)
            {
                PlayBackgroundMusic();
            }
            else
            {
                StopBackgroundMusic();
            }
        }
        
        /// <summary>
        /// Toggle sound effects on/off
        /// </summary>
        public void ToggleSFX(bool enabled)
        {
            sfxEnabled = enabled;
            UpdateAudioSettings();
            SaveSettings();
        }
        
        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateAudioSettings();
            SaveSettings();
        }
        
        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateAudioSettings();
            SaveSettings();
        }
        
        /// <summary>
        /// Update audio source settings
        /// </summary>
        private void UpdateAudioSettings()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicEnabled ? musicVolume : 0f;
            }
            
            if (sfxSource != null)
            {
                sfxSource.volume = sfxEnabled ? sfxVolume : 0f;
            }
        }
        
        /// <summary>
        /// Load audio settings from PlayerPrefs
        /// </summary>
        private void LoadSettings()
        {
            musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
            sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.8f);
        }
        
        /// <summary>
        /// Save audio settings to PlayerPrefs
        /// </summary>
        private void SaveSettings()
        {
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, musicEnabled ? 1 : 0);
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, sfxEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
        }
        
        // Public getters
        public bool IsMusicEnabled => musicEnabled;
        public bool IsSFXEnabled => sfxEnabled;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
    }
}
