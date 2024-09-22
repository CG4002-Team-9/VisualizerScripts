using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    // Renderer component of the enemy
    private Renderer enemyRenderer;

    // The MaterialPropertyBlock used to modify material properties without creating new material instances
    private MaterialPropertyBlock materialPropertyBlock;

    // Original color of the enemy
    private Color originalColor;

    // Current color of the enemy
    private Color currentColor;

    // Duration for which the redness stays before subsiding
    public float rednessDuration = 0.2f;

    // How quickly the color returns to the original
    public float fadeSpeed = 5f;

    // Coroutine reference to handle redness fading
    private Coroutine fadeCoroutine;

    // Reference to the GameState
    private GameState gameState;

    void Start()
    {
        // Get the Renderer component
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer == null)
        {
            Debug.LogError("EnemyHit script requires a Renderer component on the same GameObject.");
            return;
        }

        // Initialize the MaterialPropertyBlock
        materialPropertyBlock = new MaterialPropertyBlock();

        // Get the initial color of the material
        originalColor = enemyRenderer.material.color;
        currentColor = originalColor;

        // Get the GameState instance
        gameState = GameState.Instance;
    }

    void Update()
    {
        // Check if the enemy was hit
        if (gameState.EnemyHit)
        {
            // Trigger the redness effect
            TriggerRedness();

            // Reset the enemyHit value
            gameState.EnemyHit = false;
        }
    }

    public void TriggerRedness()
    {
        // Stop any ongoing color fading
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Set the color to red
        currentColor = Color.red;
        UpdateMaterialColor();

        // Start the fade back coroutine after a delay
        fadeCoroutine = StartCoroutine(FadeBackToOriginal());
    }

    // Coroutine to fade the color back to the original color after rednessDuration.
    private System.Collections.IEnumerator FadeBackToOriginal()
    {
        // Wait for the redness duration to expire
        yield return new WaitForSeconds(rednessDuration);

        // Gradually fade the color back to the original
        while (Vector4.Distance(currentColor, originalColor) > 0.01f)
        {
            currentColor = Color.Lerp(currentColor, originalColor, fadeSpeed * Time.deltaTime);
            UpdateMaterialColor();
            yield return null;
        }

        // Ensure the color is set back to the original at the end
        currentColor = originalColor;
        UpdateMaterialColor();
    }

    private void UpdateMaterialColor()
    {
        materialPropertyBlock.SetColor("_Color", currentColor);
        enemyRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}