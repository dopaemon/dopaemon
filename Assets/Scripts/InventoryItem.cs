using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public string itemName;

    // --- Is this item trashable --- //
    public bool isTrashable;

    // --- Item Info UI --- //
    GameObject itemInfoUI;
    TMP_Text itemInfoUI_itemName;
    TMP_Text itemInfoUI_itemDescription;
    TMP_Text itemInfoUI_itemFunctionality;

    public string thisName, thisDescription, thisFunctionality;

    // --- Consumption --- //
    GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUI;
        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<TMP_Text>();
        itemInfoUI_itemDescription = itemInfoUI.transform.Find("itemDescription").GetComponent<TMP_Text>();
        itemInfoUI_itemFunctionality = itemInfoUI.transform.Find("itemFunctionality").GetComponent<TMP_Text>();
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    // Triggered when the mouse is clicked over the item that has this script.
    public void OnPointerDown(PointerEventData eventData)
    {
        // Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                // Setting this specific gameobject to be the item we want to destroy later
                itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }
        }
    }

    // Triggered when the mouse button is released over the item that has this script.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable && itemPendingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.RecalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }
        }
    }

    void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        itemInfoUI.SetActive(false);

        healthEffectCalculation(healthEffect);
        caloriesEffectCalculation(caloriesEffect);
        hydrationEffectCalculation(hydrationEffect);
    }

    static void healthEffectCalculation(float healthEffect)
    {
        // --- Health --- //
        float healthBeforeConsumption = PlayerState.Instance.currentHealth;
        float maxHealth = PlayerState.Instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerState.Instance.setHealth(maxHealth);
            }
            else
            {
                PlayerState.Instance.setHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }

    static void caloriesEffectCalculation(float caloriesEffect)
    {
        // --- Calories --- //
        float caloriesBeforeConsumption = PlayerState.Instance.currentCalories;
        float maxCalories = PlayerState.Instance.maxCalories;

        if (caloriesEffect != 0)
        {
            if ((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                PlayerState.Instance.setCalories(maxCalories);
            }
            else
            {
                PlayerState.Instance.setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }

    static void hydrationEffectCalculation(float hydrationEffect)
    {
        // --- Hydration --- //
        float hydrationBeforeConsumption = PlayerState.Instance.currentHydrationPercent;
        float maxHydration = PlayerState.Instance.maxHydrationPercent;

        if (hydrationEffect != 0)
        {
            if ((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                PlayerState.Instance.setHydration(maxHydration);
            }
            else
            {
                PlayerState.Instance.setHydration(hydrationBeforeConsumption + hydrationEffect);
            }
        }
    }
}
