using UnityEngine;

public class DestructibleTarget : MonoBehaviour
{
    // Feature: Durability metrics
    [Header("Health Settings")]
    public float maxHealth = 30f;
    private float currentHealth;

    // Feature: Front-only spawn boundary controls
    [Header("Forward Spawn Range")]
    public float minForwardDistance = 6f;   // How close in front the target can appear
    public float maxForwardDistance = 18f;  // How far down the range it can appear
    public float horizontalSpread = 6f;     // Left/Right spread along the shooting corridor
    public float minHeight = 1f;            // Lowest spawn height
    public float maxHeight = 3f;            // Highest spawn height

    [Header("References")]
    public Transform playerTransform;

    void Start()
    {
        currentHealth = maxHealth;

        // Auto-find player if not explicitly assigned in Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Place target in a valid forward position immediately on start
        RelocateTarget();
    }

    // Functionality: Deducts health and relocates target upon destruction
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            RelocateTarget();
        }
    }

    // Functionality: Teleports target strictly in front of player's current view corridor
    void RelocateTarget()
    {
        currentHealth = maxHealth;

        if (playerTransform != null)
        {
            // Pick random lateral offset (left/right) and forward distance
            float randomXOffset = Random.Range(-horizontalSpread, horizontalSpread);
            float randomZForward = Random.Range(minForwardDistance, maxForwardDistance);
            float randomY = Random.Range(minHeight, maxHeight);

            // Compute position using player orientation (transform.forward and transform.right)
            Vector3 spawnPosition = playerTransform.position
                                  + (playerTransform.forward * randomZForward)
                                  + (playerTransform.right * randomXOffset);

            spawnPosition.y = randomY;

            transform.position = spawnPosition;
        }
        else
        {
            // Fallback: spawn along global positive Z
            transform.position = new Vector3(
                Random.Range(-horizontalSpread, horizontalSpread),
                Random.Range(minHeight, maxHeight),
                Random.Range(minForwardDistance, maxForwardDistance)
            );
        }
    }
}