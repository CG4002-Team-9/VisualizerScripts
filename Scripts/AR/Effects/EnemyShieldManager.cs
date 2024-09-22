using UnityEngine;

public class EnemyShieldManager : MonoBehaviour
{
    // Reference to the GameState object
    private GameState gameState;

    //Reference to shield object
    public GameObject enemyShieldObject;

    private void Start()
    {
        // Get reference to the GameState singleton
        gameState = GameState.Instance;

        UpdateEnemyShield();
    }

    private void Update()
    {
        UpdateEnemyShield();
    }

    private void UpdateEnemyShield()
    {
        int enemyShieldValue = gameState.EnemyShieldValue;

        if (enemyShieldValue != 0)
        {
            enemyShieldObject.SetActive(true);
        }
        else
        {
            enemyShieldObject.SetActive(false);
            
        }
    }
}