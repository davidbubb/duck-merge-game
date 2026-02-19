using UnityEngine;
using UnityEngine.UI;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages the tutorial/onboarding screen
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Image demonstrationImage;
        
        [Header("Tutorial Pages")]
        [SerializeField] private TutorialPage[] pages;
        
        private int currentPage = 0;
        private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";
        
        [System.Serializable]
        public class TutorialPage
        {
            public string title;
            [TextArea(3, 6)]
            public string instruction;
            public Sprite demonstrationSprite;
        }
        
        private void Start()
        {
            // Setup button listeners
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnNextClicked);
            }
            
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipClicked);
            }
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
            
            // Check if tutorial should be shown on first launch
            if (!HasCompletedTutorial() && GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
            
            // Hide by default
            HideTutorial();
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            // Show tutorial on first time reaching main menu
            if (newState == GameState.MainMenu && !HasCompletedTutorial())
            {
                ShowTutorial();
            }
        }
        
        /// <summary>
        /// Show tutorial from beginning
        /// </summary>
        public void ShowTutorial()
        {
            currentPage = 0;
            UpdateTutorialPage();
            
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hide tutorial
        /// </summary>
        public void HideTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update tutorial page content
        /// </summary>
        private void UpdateTutorialPage()
        {
            if (pages == null || pages.Length == 0) return;
            if (currentPage < 0 || currentPage >= pages.Length) return;
            
            TutorialPage page = pages[currentPage];
            
            if (titleText != null)
            {
                titleText.text = page.title;
            }
            
            if (instructionText != null)
            {
                instructionText.text = page.instruction;
            }
            
            if (demonstrationImage != null && page.demonstrationSprite != null)
            {
                demonstrationImage.sprite = page.demonstrationSprite;
                demonstrationImage.gameObject.SetActive(true);
            }
            else if (demonstrationImage != null)
            {
                demonstrationImage.gameObject.SetActive(false);
            }
            
            // Update button visibility
            if (nextButton != null)
            {
                if (currentPage < pages.Length - 1)
                {
                    nextButton.GetComponentInChildren<Text>().text = "NEXT";
                }
                else
                {
                    nextButton.GetComponentInChildren<Text>().text = "GOT IT!";
                }
            }
        }
        
        private void OnNextClicked()
        {
            currentPage++;
            
            if (currentPage >= pages.Length)
            {
                // Tutorial complete
                CompleteTutorial();
            }
            else
            {
                UpdateTutorialPage();
            }
        }
        
        private void OnSkipClicked()
        {
            CompleteTutorial();
        }
        
        private void OnCloseClicked()
        {
            CompleteTutorial();
        }
        
        /// <summary>
        /// Mark tutorial as completed
        /// </summary>
        private void CompleteTutorial()
        {
            PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
            PlayerPrefs.Save();
            
            HideTutorial();
            
            Debug.Log("Tutorial completed");
        }
        
        /// <summary>
        /// Check if player has completed tutorial
        /// </summary>
        private bool HasCompletedTutorial()
        {
            return PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 1;
        }
        
        /// <summary>
        /// Reset tutorial (for testing)
        /// </summary>
        public void ResetTutorial()
        {
            PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
            PlayerPrefs.Save();
            Debug.Log("Tutorial reset");
        }
        
        /// <summary>
        /// Create default tutorial pages in editor
        /// </summary>
        private void OnValidate()
        {
            // This runs in editor to help set up default pages
            #if UNITY_EDITOR
            if (pages == null || pages.Length == 0)
            {
                pages = new TutorialPage[]
                {
                    new TutorialPage
                    {
                        title = "Welcome to Duck Merge!",
                        instruction = "Merge identical ducks to evolve them into larger water birds!\n\n" +
                                    "Start with tiny ducklings and work your way up to majestic swans."
                    },
                    new TutorialPage
                    {
                        title = "How to Play",
                        instruction = "1. Move your finger/mouse to position the duck horizontally\n\n" +
                                    "2. Tap/click to drop the duck into the container\n\n" +
                                    "3. Watch as physics makes the ducks bounce and settle"
                    },
                    new TutorialPage
                    {
                        title = "Merge to Evolve",
                        instruction = "When two identical ducks touch, they merge into the next evolution!\n\n" +
                                    "Each merge gives you points:\n" +
                                    "• Higher tier merges = more points\n" +
                                    "• Create the Swan for maximum points!"
                    },
                    new TutorialPage
                    {
                        title = "Avoid Game Over",
                        instruction = "Don't let ducks pile up too high!\n\n" +
                                    "If a duck stays above the red danger line for 2 seconds, it's game over.\n\n" +
                                    "Plan your drops carefully!"
                    },
                    new TutorialPage
                    {
                        title = "Ready to Play!",
                        instruction = "That's all you need to know!\n\n" +
                                    "• Try to beat your high score\n" +
                                    "• Aim to create all 11 bird tiers\n" +
                                    "• Have fun merging ducks!\n\n" +
                                    "Good luck! 🦆"
                    }
                };
            }
            #endif
        }
    }
}
