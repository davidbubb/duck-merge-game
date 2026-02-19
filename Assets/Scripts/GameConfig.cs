using UnityEngine;

namespace DuckMergeGame
{
    /// <summary>
    /// Central configuration for all game constants and parameters
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Duck Merge/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Bird Tiers Configuration")]
        [Tooltip("Number of bird tiers in the game")]
        public int totalBirdTiers = 11;
        
        [Tooltip("Maximum tier that can be randomly dropped (1-based)")]
        public int maxDropTier = 5;
        
        [Tooltip("Base size for tier 1 bird")]
        public float baseBirdSize = 0.5f;
        
        [Tooltip("Size multiplier per tier")]
        public float sizeMultiplierPerTier = 1.3f;
        
        [Header("Scoring Configuration")]
        [Tooltip("Points awarded for each tier merge (index 0 = tier 1->2)")]
        public int[] mergePoints = new int[]
        {
            10,    // Tier 1->2
            20,    // Tier 2->3
            40,    // Tier 3->4
            80,    // Tier 4->5
            160,   // Tier 5->6
            320,   // Tier 6->7
            640,   // Tier 7->8
            1280,  // Tier 8->9
            2560,  // Tier 9->10
            5120   // Tier 10->11
        };
        
        [Header("Physics Configuration")]
        [Tooltip("Gravity scale for the game")]
        public float gravityScale = 2f;
        
        [Tooltip("Physics material bounciness")]
        public float bounciness = 0.3f;
        
        [Tooltip("Physics material friction")]
        public float friction = 0.4f;
        
        [Tooltip("Target frame rate")]
        public int targetFrameRate = 60;
        
        [Header("Game Over Configuration")]
        [Tooltip("Height ratio of danger line from bottom (0-1)")]
        public float dangerLineHeightRatio = 0.85f;
        
        [Tooltip("Time in seconds a bird must stay above danger line for game over")]
        public float gameOverDelay = 2f;
        
        [Header("Container Configuration")]
        [Tooltip("Container width in world units")]
        public float containerWidth = 6f;
        
        [Tooltip("Container height in world units")]
        public float containerHeight = 10f;
        
        [Header("Audio Configuration")]
        [Tooltip("Master volume (0-1)")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        
        [Tooltip("Music volume (0-1)")]
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        
        [Tooltip("SFX volume (0-1)")]
        [Range(0f, 1f)]
        public float sfxVolume = 0.8f;
        
        [Header("Bird Tier Names")]
        public string[] birdTierNames = new string[]
        {
            "Duckling",
            "Baby Duck",
            "Juvenile Duck",
            "Mallard",
            "Wood Duck",
            "Mandarin Duck",
            "Goose",
            "Canada Goose",
            "Pelican",
            "Great Heron",
            "Swan"
        };
        
        [Header("Bird Tier Colors (for simple visuals)")]
        public Color[] birdTierColors = new Color[]
        {
            new Color(1f, 0.92f, 0.016f),      // Yellow - Duckling
            new Color(1f, 0.84f, 0f),          // Gold - Baby Duck
            new Color(0.82f, 0.71f, 0.55f),    // Tan - Juvenile Duck
            new Color(0.13f, 0.55f, 0.13f),    // Forest Green - Mallard
            new Color(0.55f, 0.27f, 0.07f),    // Brown - Wood Duck
            new Color(1f, 0.5f, 0f),           // Orange - Mandarin Duck
            new Color(0.96f, 0.96f, 0.96f),    // White - Goose
            new Color(0.54f, 0.27f, 0.07f),    // Dark Brown - Canada Goose
            new Color(0.94f, 0.9f, 0.55f),     // Light Yellow - Pelican
            new Color(0.41f, 0.41f, 0.61f),    // Blue Grey - Great Heron
            new Color(1f, 1f, 1f)              // Pure White - Swan
        };
        
        /// <summary>
        /// Get merge points for a specific tier
        /// </summary>
        public int GetMergePoints(int tier)
        {
            if (tier < 1 || tier >= totalBirdTiers)
                return 0;
            return mergePoints[tier - 1];
        }
        
        /// <summary>
        /// Get size for a specific bird tier
        /// </summary>
        public float GetBirdSize(int tier)
        {
            return baseBirdSize * Mathf.Pow(sizeMultiplierPerTier, tier - 1);
        }
        
        /// <summary>
        /// Get color for a specific bird tier
        /// </summary>
        public Color GetBirdColor(int tier)
        {
            if (tier < 1 || tier > totalBirdTiers)
                return Color.white;
            return birdTierColors[tier - 1];
        }
        
        /// <summary>
        /// Get name for a specific bird tier
        /// </summary>
        public string GetBirdName(int tier)
        {
            if (tier < 1 || tier > totalBirdTiers)
                return "Unknown";
            return birdTierNames[tier - 1];
        }
    }
}
