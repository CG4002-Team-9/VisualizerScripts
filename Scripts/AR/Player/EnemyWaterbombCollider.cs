using UnityEngine;
using TMPro;

public class PlayerWaterbombCollider : MonoBehaviour
{
    [Header("Collision Settings")]
    public string waterbombTag = "Waterbomb"; // Tag assigned to waterbombs

    [Header("UI Elements (Optional)")]
    public TextMeshProUGUI collisionCountText;

    public void ResetOverlapCount()
    {
        GameState.Instance.EnemyInWaterBombCount = 0;
        UpdateOverlapCountUI();
    }

    // Called when a waterbomb starts overlapping with the player
    void OnTriggerEnter(Collider other)
    {
        // Check if the player is active and the other collider has the waterbomb tag
        if (gameObject.activeInHierarchy && other.gameObject.CompareTag(waterbombTag))
        {
            // Increment the overlap count
            GameState.Instance.EnemyInWaterBombCount += 1;

            // Log the overlap
            Debug.Log($"Waterbomb entered overlap. Current overlaps: {GameState.Instance.EnemyInWaterBombCount}");

            // Update the UI if necessary
            UpdateOverlapCountUI();
        }
    }

    // Called when a waterbomb stops overlapping with the player
    void OnTriggerExit(Collider other)
    {
        // Check if the player is active and the other collider has the waterbomb tag
        if (gameObject.activeInHierarchy && other.gameObject.CompareTag(waterbombTag))
        {
            // Decrement the game state's enemyInWaterBombCount
            GameState.Instance.EnemyInWaterBombCount -= 1;

            // Log the overlap
            Debug.Log($"Waterbomb exited overlap. Current overlaps: {GameState.Instance.EnemyInWaterBombCount}");

            // Update the UI if necessary
            UpdateOverlapCountUI();
        }
    }

    // Method to update the overlap count on the UI
    void UpdateOverlapCountUI()
    {
        if (collisionCountText != null)
        {
            collisionCountText.text = $"Current Waterbomb Collisions: {GameState.Instance.EnemyInWaterBombCount}";
        }
    }
}