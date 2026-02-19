using UnityEngine;
using System.Collections;

namespace DuckMergeGame
{
    /// <summary>
    /// Represents a single bird in the game with merge capabilities
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Bird : MonoBehaviour
    {
        [Header("Bird Properties")]
        [SerializeField] private int tier = 1;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Rigidbody2D rb;
        private CircleCollider2D circleCollider;
        private bool canMerge = false;
        private bool isMerging = false;
        
        public int Tier => tier;
        public bool CanMerge => canMerge;
        public bool IsMerging => isMerging;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
        
        /// <summary>
        /// Initialize the bird with a specific tier
        /// </summary>
        public void Initialize(int birdTier, GameConfig config)
        {
            tier = birdTier;
            
            if (config == null) return;
            
            // Set size based on tier
            float size = config.GetBirdSize(tier);
            transform.localScale = Vector3.one * size;
            
            // Set color based on tier
            if (spriteRenderer != null)
            {
                spriteRenderer.color = config.GetBirdColor(tier);
            }
            
            // Set collider radius
            if (circleCollider != null)
            {
                circleCollider.radius = 0.5f; // Base radius, scaled by transform
            }
            
            // Set physics properties
            if (rb != null)
            {
                rb.gravityScale = config.gravityScale;
                rb.mass = size; // Mass increases with size
            }
            
            // Enable merge after a short delay (prevents immediate merge on spawn)
            StartCoroutine(EnableMergeAfterDelay(0.5f));
        }
        
        /// <summary>
        /// Enable merge capability after delay
        /// </summary>
        private IEnumerator EnableMergeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            canMerge = true;
        }
        
        /// <summary>
        /// Freeze the bird (stop physics)
        /// </summary>
        public void Freeze()
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.isKinematic = true;
            }
        }
        
        /// <summary>
        /// Unfreeze the bird (enable physics)
        /// </summary>
        public void Unfreeze()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
        
        /// <summary>
        /// Mark this bird as merging
        /// </summary>
        public void StartMerge()
        {
            isMerging = true;
            canMerge = false;
        }
        
        /// <summary>
        /// Handle collision with another bird
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if we collided with another bird
            Bird otherBird = collision.gameObject.GetComponent<Bird>();
            
            if (otherBird != null && CanAttemptMerge(otherBird))
            {
                // Notify the gameplay manager about the merge
                GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
                if (gameplayManager != null)
                {
                    gameplayManager.AttemptMerge(this, otherBird);
                }
            }
        }
        
        /// <summary>
        /// Check if we can attempt to merge with another bird
        /// </summary>
        private bool CanAttemptMerge(Bird other)
        {
            // Both birds must be same tier, able to merge, and not already merging
            return other != null &&
                   tier == other.tier &&
                   canMerge &&
                   other.canMerge &&
                   !isMerging &&
                   !other.isMerging &&
                   tier < 11; // Can't merge the final tier (Swan)
        }
        
        /// <summary>
        /// Destroy this bird
        /// </summary>
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
