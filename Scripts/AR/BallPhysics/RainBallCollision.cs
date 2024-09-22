using UnityEngine;
using Vuforia; // Import Vuforia namespace

public class RainBallCollision : MonoBehaviour
{
    public GameObject rainCloudPrefab; // Assign in the Inspector

    private bool isBeingDestroyed = false;

    void Start()
    {
        // Schedule the destruction of the ball after 4 seconds
        Invoke(nameof(DestroyBall), 4f);
        Debug.Log("Ball will be destroyed in 4 seconds if no collision occurs.");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBeingDestroyed) return;

        Debug.Log("Collision detected with: " + collision.gameObject.name);

        // Instantiate rain cloud at collision point
        ContactPoint contact = collision.contacts[0];
        Vector3 collisionPoint = contact.point;

        Debug.Log("Collision point: " + collisionPoint);

        // Instantiate and anchor the rain cloud
        InstantiateRainCloud(collisionPoint);

        // Destroy the water bomb
        DestroyBall();
    }

    void InstantiateRainCloud(Vector3 position)
    {
        if (rainCloudPrefab == null)
        {
            Debug.LogError("Rain Cloud Prefab is not assigned.");
            return;
        }

        // Instantiate the rain cloud at the collision point
        GameObject rainCloud = Instantiate(rainCloudPrefab, position, Quaternion.identity);

        // Anchor the rain cloud using Vuforia's Ground Plane
        AnchorRainCloud(rainCloud, position);
    }

    void AnchorRainCloud(GameObject rainCloud, Vector3 position)
    {
        // Add the Content Positioning Behaviour
        ContentPositioningBehaviour contentPositioning = FindObjectOfType<ContentPositioningBehaviour>();
        if (contentPositioning == null)
        {
            Debug.LogError("ContentPositioningBehaviour not found in the scene.");
            return;
        }
        
        contentPositioning.PositionContentAtMidAirAnchor(rainCloud.transform);
    }

    void DestroyBall()
    {
        if (isBeingDestroyed) return;

        isBeingDestroyed = true;
        CancelInvoke();
        Destroy(gameObject);
        Debug.Log("Ball destroyed.");
    }

    void OnDestroy()
    {
        isBeingDestroyed = true;
        CancelInvoke();
        Debug.Log("Ball OnDestroy called.");
    }

    void OnDisable()
    {
        isBeingDestroyed = true;
        CancelInvoke();
        Debug.Log("Ball OnDisable called.");
    }
}