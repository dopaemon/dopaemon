using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    Slider slider;
    public TMP_Text hydrationCounter;
    public GameObject playerState;

    float currentHydration, maxHydration;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        PlayerState state = playerState.GetComponent<PlayerState>();
        currentHydration = state.currentHydrationPercent;
        maxHydration = state.maxHydrationPercent;

        float fillValue = currentHydration / maxHydration;
        slider.value = fillValue;

        hydrationCounter.text = currentHydration + "%";
    }
}
