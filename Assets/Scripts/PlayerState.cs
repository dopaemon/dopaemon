using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    // ---- Player Health ---- //
    public float currentHealth;
    public float maxHealth = 100f;

    // ---- Player Calories ---- //
    public float currentCalories;
    public float maxCalories = 2000f;

    float distanceTravelled = 0f;
    Vector3 lastPosition;
    public GameObject playerBody;

    // ---- Player Hydration ---- //
    public float currentHydrationPercent;
    public float maxHydrationPercent = 100f;
    public bool isHydrationActive = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;

        if (playerBody != null)
        {
            lastPosition = playerBody.transform.position;
        }

        StartCoroutine(DecreaseHydration());
    }

    IEnumerator DecreaseHydration()
    {
        while (true)
        {
            if (isHydrationActive && currentHydrationPercent > 0f)
            {
                currentHydrationPercent -= 1f;
            }

            yield return new WaitForSeconds(10f);
        }
    }

    void Update()
    {
        if (playerBody != null)
        {
            distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
            lastPosition = playerBody.transform.position;

            if (distanceTravelled >= 5f)
            {
                distanceTravelled = 0f;
                currentCalories = Mathf.Max(0f, currentCalories - 1f);
            }
        }

        // Testing the health bar
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth = Mathf.Max(0f, currentHealth - 10f);
        }
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;
    }

    public void setHydration(float newHydration)
    {
        currentHydrationPercent = newHydration;
    }
}
