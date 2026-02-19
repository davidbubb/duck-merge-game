using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DuckMergeGame
{
    /// <summary>
    /// Manages core gameplay mechanics: dropping birds, merging, game over detection
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject birdPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform container;
        
        [Header("Danger Line")]
        [SerializeField] private Transform dangerLine;
        [SerializeField] private float dangerLineY;
        
        [Header("Drop Settings")]
        [SerializeField] private float minDropX = -2.5f;
        [SerializeField] private float maxDropX = 2.5f;
        [SerializeField] private LineRenderer dropIndicator;
        
        private Bird currentBird;
        private int nextBirdTier;
        private bool isDropping = false;
        private float dropPositionX = 0f;
        
        // Game over tracking
        private Dictionary<Bird, float> birdsAboveDangerLine = new Dictionary<Bird, float>();
        private List<Bird> activeBirds = new List<Bird>();
        
        // Merge tracking
        private HashSet<Bird> mergingBirds = new HashSet<Bird>();
        
        private void Start()
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned!");
                return;
            }
            
            SetupDangerLine();
            GenerateNextBird();
            SpawnBirdForDrop();
            
            // Subscribe to game state changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += OnGameStateChanged;
            }
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;
            
            HandleDropInput();
            CheckGameOver();
        }
        
        /// <summary>
        /// Setup danger line position
        /// </summary>
        private void SetupDangerLine()
        {
            if (gameConfig != null && container != null)
            {
                float containerHeight = gameConfig.containerHeight;
                dangerLineY = container.position.y - containerHeight / 2f + containerHeight * gameConfig.dangerLineHeightRatio;
                
                if (dangerLine != null)
                {
                    dangerLine.position = new Vector3(0f, dangerLineY, 0f);
                }
            }
        }
        
        /// <summary>
        /// Handle player input for dropping birds
        /// </summary>
        private void HandleDropInput()
        {
            if (isDropping || currentBird == null) return;
            
            // Get mouse/touch position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dropPositionX = Mathf.Clamp(mousePos.x, minDropX, maxDropX);
            
            // Update current bird position
            Vector3 newPos = currentBird.transform.position;
            newPos.x = dropPositionX;
            currentBird.transform.position = newPos;
            
            // Update drop indicator
            if (dropIndicator != null)
            {
                dropIndicator.SetPosition(0, new Vector3(dropPositionX, spawnPoint.position.y, 0f));
                dropIndicator.SetPosition(1, new Vector3(dropPositionX, container.position.y - gameConfig.containerHeight / 2f, 0f));
            }
            
            // Drop bird on click/tap
            if (Input.GetMouseButtonDown(0))
            {
                DropBird();
            }
        }
        
        /// <summary>
        /// Drop the current bird
        /// </summary>
        private void DropBird()
        {
            if (currentBird == null || isDropping) return;
            
            isDropping = true;
            
            // Unfreeze physics
            currentBird.Unfreeze();
            
            // Add to active birds list
            activeBirds.Add(currentBird);
            
            // Hide drop indicator
            if (dropIndicator != null)
            {
                dropIndicator.enabled = false;
            }
            
            // Spawn next bird after delay
            StartCoroutine(SpawnNextBirdAfterDelay(0.5f));
        }
        
        /// <summary>
        /// Spawn next bird after a delay
        /// </summary>
        private IEnumerator SpawnNextBirdAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            GenerateNextBird();
            SpawnBirdForDrop();
            
            isDropping = false;
            
            // Show drop indicator
            if (dropIndicator != null)
            {
                dropIndicator.enabled = true;
            }
        }
        
        /// <summary>
        /// Generate the next bird tier randomly (1-5)
        /// </summary>
        private void GenerateNextBird()
        {
            if (gameConfig != null)
            {
                nextBirdTier = Random.Range(1, gameConfig.maxDropTier + 1);
            }
        }
        
        /// <summary>
        /// Spawn a bird for dropping
        /// </summary>
        private void SpawnBirdForDrop()
        {
            if (birdPrefab == null || spawnPoint == null || gameConfig == null) return;
            
            GameObject birdObj = Instantiate(birdPrefab, spawnPoint.position, Quaternion.identity, container);
            currentBird = birdObj.GetComponent<Bird>();
            
            if (currentBird != null)
            {
                currentBird.Initialize(nextBirdTier, gameConfig);
                currentBird.Freeze(); // Keep it frozen until dropped
            }
        }
        
        /// <summary>
        /// Attempt to merge two birds
        /// </summary>
        public void AttemptMerge(Bird bird1, Bird bird2)
        {
            // Validate merge conditions
            if (bird1 == null || bird2 == null) return;
            if (bird1.Tier != bird2.Tier) return;
            if (bird1.IsMerging || bird2.IsMerging) return;
            if (mergingBirds.Contains(bird1) || mergingBirds.Contains(bird2)) return;
            if (bird1.Tier >= gameConfig.totalBirdTiers) return; // Can't merge max tier
            
            // Mark as merging
            bird1.StartMerge();
            bird2.StartMerge();
            mergingBirds.Add(bird1);
            mergingBirds.Add(bird2);
            
            // Calculate merge position (midpoint)
            Vector3 mergePosition = (bird1.transform.position + bird2.transform.position) / 2f;
            
            // Start merge coroutine
            StartCoroutine(PerformMerge(bird1, bird2, mergePosition));
        }
        
        /// <summary>
        /// Perform the merge animation and create new bird
        /// </summary>
        private IEnumerator PerformMerge(Bird bird1, Bird bird2, Vector3 mergePosition)
        {
            int newTier = bird1.Tier + 1;
            
            // Add score
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(bird1.Tier);
            }
            
            // TODO: Play merge animation/particles
            
            // Remove old birds from tracking
            activeBirds.Remove(bird1);
            activeBirds.Remove(bird2);
            birdsAboveDangerLine.Remove(bird1);
            birdsAboveDangerLine.Remove(bird2);
            
            // Destroy old birds
            bird1.DestroySelf();
            bird2.DestroySelf();
            
            yield return new WaitForSeconds(0.1f);
            
            // Create new bird at merge position
            if (birdPrefab != null && gameConfig != null)
            {
                GameObject newBirdObj = Instantiate(birdPrefab, mergePosition, Quaternion.identity, container);
                Bird newBird = newBirdObj.GetComponent<Bird>();
                
                if (newBird != null)
                {
                    newBird.Initialize(newTier, gameConfig);
                    activeBirds.Add(newBird);
                }
            }
            
            // Remove from merging set
            mergingBirds.Remove(bird1);
            mergingBirds.Remove(bird2);
        }
        
        /// <summary>
        /// Check for game over condition
        /// </summary>
        private void CheckGameOver()
        {
            if (gameConfig == null) return;
            
            // Update tracking for birds above danger line
            List<Bird> birdsToRemove = new List<Bird>();
            
            foreach (Bird bird in activeBirds)
            {
                if (bird == null)
                {
                    birdsToRemove.Add(bird);
                    continue;
                }
                
                // Check if bird is above danger line
                if (bird.transform.position.y > dangerLineY)
                {
                    // Add or update time tracking
                    if (!birdsAboveDangerLine.ContainsKey(bird))
                    {
                        birdsAboveDangerLine[bird] = Time.time;
                    }
                    else
                    {
                        // Check if bird has been above line for too long
                        float timeAbove = Time.time - birdsAboveDangerLine[bird];
                        if (timeAbove >= gameConfig.gameOverDelay)
                        {
                            TriggerGameOver();
                            return;
                        }
                    }
                }
                else
                {
                    // Bird is below danger line, remove from tracking
                    if (birdsAboveDangerLine.ContainsKey(bird))
                    {
                        birdsAboveDangerLine.Remove(bird);
                    }
                }
            }
            
            // Clean up null birds
            foreach (Bird bird in birdsToRemove)
            {
                activeBirds.Remove(bird);
                birdsAboveDangerLine.Remove(bird);
            }
        }
        
        /// <summary>
        /// Trigger game over
        /// </summary>
        private void TriggerGameOver()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
        
        /// <summary>
        /// Handle game state changes
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    // Reset for new game
                    ClearAllBirds();
                    GenerateNextBird();
                    SpawnBirdForDrop();
                    break;
                    
                case GameState.Paused:
                    // Pause handled by Time.timeScale
                    break;
                    
                case GameState.GameOver:
                    // Stop accepting input
                    break;
                    
                case GameState.MainMenu:
                    ClearAllBirds();
                    break;
            }
        }
        
        /// <summary>
        /// Clear all birds from the scene
        /// </summary>
        private void ClearAllBirds()
        {
            foreach (Bird bird in activeBirds)
            {
                if (bird != null)
                {
                    Destroy(bird.gameObject);
                }
            }
            
            activeBirds.Clear();
            birdsAboveDangerLine.Clear();
            mergingBirds.Clear();
            
            if (currentBird != null)
            {
                Destroy(currentBird.gameObject);
                currentBird = null;
            }
        }
    }
}
