using UnityEngine;

public class OwnShieldManager : MonoBehaviour
{
    // Reference to the GameState object
    private GameState gameState;

    //Reference to shield object
    public GameObject shieldObject;

    private void Start()
    {
        // Get reference to the GameState singleton
        gameState = GameState.Instance;

        UpdateOwnShield();
    }

    private void Update()
    {
        UpdateOwnShield();
    }

    private void UpdateOwnShield()
    {
        int ownShieldValue = gameState.ShieldValue;

        if (ownShieldValue != 0)
        {
            shieldObject.SetActive(true);
        }
        else
        {
            shieldObject.SetActive(false);
            
        }
    }
}