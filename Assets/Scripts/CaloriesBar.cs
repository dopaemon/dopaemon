using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    Slider slider;
    public TMP_Text caloriesCounter;
    public GameObject playerState;

    float currentCalories, maxCalories;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        PlayerState state = playerState.GetComponent<PlayerState>();
        currentCalories = state.currentCalories;
        maxCalories = state.maxCalories;

        float fillValue = currentCalories / maxCalories;
        slider.value = fillValue;

        caloriesCounter.text = currentCalories + "/" + maxCalories;
    }
}
