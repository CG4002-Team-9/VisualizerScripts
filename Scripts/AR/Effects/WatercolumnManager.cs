using UnityEngine;

public class WatercolumnManager : MonoBehaviour
{
    private Renderer[] renderers;

    void Start()
    {
        // Get all Renderer components attached to the GameObject
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Update()
    {
        if (GameState.Instance.EnemyActive)
        {
            SetRenderersActive(true);
        }
        else
        {
            SetRenderersActive(false);
        }
    }

    private void SetRenderersActive(bool isActive)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isActive;
        }
    }
}