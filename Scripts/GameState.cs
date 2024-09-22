using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    // Singleton instance
    public static GameState Instance { get; private set; }

    // Private variables
    private int healthValue, shieldValue, scoreValue, shieldCount, ammoCount, waterbombCount, enemyHealthValue, enemyShieldValue, enemyInWaterBombCount;

    private bool enemyActive, enemyHit;
    

    // Constants to define min and max values
    private const int MAX_SHIELD_COUNT = 3;
    private const int MIN_SHIELD_COUNT = 0;

    private const int MAX_AMMO_COUNT = 6;
    private const int MIN_AMMO_COUNT = 0;

    private const int MAX_WATERBOMB_COUNT = 2;
    private const int MIN_WATERBOMB_COUNT = 0;

    private const int MAX_HEALTH_VALUE = 100;
    private const int MIN_HEALTH_VALUE = 0;

    private const int MAX_SHIELD_VALUE = 30;
    private const int MIN_SHIELD_VALUE = 0;

    // Events for property changes
    public event Action EnemyActiveChanged;
    public event Action EnemyInWaterBombCountChanged;

    // Getter setters with validation and clamping

    public bool EnemyActive
    {
        get { return enemyActive; }
        set
        {
            if (enemyActive != value)
            {
                enemyActive = value;
                EnemyActiveChanged.Invoke();
            }
        }
    }

    public int EnemyInWaterBombCount
    {
        get { return enemyInWaterBombCount; }
        set
        {
            int newValue = Mathf.Max(0, value); // Ensure the value is not negative
            if (enemyInWaterBombCount != newValue)
            {
                enemyInWaterBombCount = newValue;
                EnemyInWaterBombCountChanged.Invoke();
            }
        }
    }
    public bool EnemyHit
    {
        get { return enemyHit; }
        set { enemyHit = value; }
    }
    public int HealthValue
    {
        get { return healthValue; }
        set { healthValue = Mathf.Clamp(value, MIN_HEALTH_VALUE, MAX_HEALTH_VALUE); } // Clamp healthValue
    }

    public int ShieldValue
    {
        get { return shieldValue; }
        set { shieldValue = Mathf.Clamp(value, MIN_SHIELD_VALUE, MAX_SHIELD_VALUE); } // Clamp shieldValue
    }

    public int EnemyHealthValue
    {
        get { return enemyHealthValue; }
        set { enemyHealthValue = Mathf.Clamp(value, MIN_HEALTH_VALUE, MAX_HEALTH_VALUE); } // Clamp enemyHealthValue
    }

    public int EnemyShieldValue
    {
        get { return enemyShieldValue; }
        set { enemyShieldValue = Mathf.Clamp(value, MIN_SHIELD_VALUE, MAX_SHIELD_VALUE); } // Clamp enemyShieldValue
    }

    public int ScoreValue
    {
        get { return scoreValue; }
        set { scoreValue = Mathf.Max(0, value); } // Score should not be negative
    }

    public int ShieldCount
    {
        get { return shieldCount; }
        set { shieldCount = Mathf.Clamp(value, MIN_SHIELD_COUNT, MAX_SHIELD_COUNT); } // Clamp shieldCount
    }

    public int AmmoCount
    {
        get { return ammoCount; }
        set { ammoCount = Mathf.Clamp(value, MIN_AMMO_COUNT, MAX_AMMO_COUNT); } // Clamp ammoCount
    }

    public int WaterbombCount
    {
        get { return waterbombCount; }
        set { waterbombCount = Mathf.Clamp(value, MIN_WATERBOMB_COUNT, MAX_WATERBOMB_COUNT); } // Clamp waterbombCount
    }

    // Increment and decrement methods for shieldCount
    public void IncrementShieldCount()
    {
        ShieldCount = Mathf.Min(ShieldCount + 1, MAX_SHIELD_COUNT);
    }

    public void DecrementShieldCount()
    {
        ShieldCount = Mathf.Max(ShieldCount - 1, MIN_SHIELD_COUNT);
    }

    // Increment and decrement methods for ammoCount
    public void IncrementAmmoCount()
    {
        AmmoCount = Mathf.Min(AmmoCount + 1, MAX_AMMO_COUNT);
    }

    public void DecrementAmmoCount()
    {
        AmmoCount = Mathf.Max(AmmoCount - 1, MIN_AMMO_COUNT);
    }

    // Increment and decrement methods for waterbombCount
    public void IncrementWaterbombCount()
    {
        WaterbombCount = Mathf.Min(WaterbombCount + 1, MAX_WATERBOMB_COUNT);
    }

    public void DecrementWaterbombCount()
    {
        WaterbombCount = Mathf.Max(WaterbombCount - 1, MIN_WATERBOMB_COUNT);
    }

    public void IncrementHealthValue()
    {
        HealthValue = Mathf.Min(HealthValue + 5, MAX_HEALTH_VALUE);
    }

    public void DecrementHealthValue()
    {
        HealthValue = Mathf.Max(HealthValue - 5, MIN_HEALTH_VALUE);
    }

    public void IncrementEnemyHealthValue()
    {
        EnemyHealthValue = Mathf.Min(EnemyHealthValue + 5, MAX_HEALTH_VALUE);
    }

    public void DecrementEnemyHealthValue()
    {
        EnemyHealthValue = Mathf.Max(EnemyHealthValue - 5, MIN_HEALTH_VALUE);
    }

    public void IncrementShieldValue()
    {
        ShieldValue = Mathf.Min(ShieldValue + 5, MAX_SHIELD_VALUE);
    }

    public void DecrementShieldValue()
    {
        ShieldValue = Mathf.Max(ShieldValue - 5, MIN_SHIELD_VALUE);
    }

    public void IncrementEnemyShieldValue()
    {
        EnemyShieldValue = Mathf.Min(EnemyShieldValue + 5, MAX_SHIELD_VALUE);
    }

    public void DecrementEnemyShieldValue()
    {
        EnemyShieldValue = Mathf.Max(EnemyShieldValue - 5, MIN_SHIELD_VALUE);
    }

    // Unity lifecycle methods

    private void Awake()
    {
        // Singleton pattern enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Set initial values
        enemyActive = false;
        healthValue = 50; // Example initial health value
        shieldValue = 15; // Example initial shield value
        scoreValue = 0; // Example initial score value
        shieldCount = 1; // Example initial shield count
        ammoCount = 2; // Example initial ammo count
        waterbombCount = 2; // Example initial waterbomb count
        enemyHealthValue = 40; // Example initial enemy health value
        enemyShieldValue = 10; // Example initial enemy shield value
    }

}