using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public bool onTarget { get; private set; }
    public GameObject selectedObject { get; private set; }

    public GameObject interaction_info_ui;
    TMP_Text interaction_text;

    public Image centerDotImage;
    public Image handIcon;

    private void Start()
    {
        interaction_text = interaction_info_ui.GetComponent<TMP_Text>();
        interaction_info_ui.SetActive(false);
        SetHandIcon(false);
    }

    void SetHandIcon(bool visible)
    {
        if (centerDotImage != null) centerDotImage.gameObject.SetActive(!visible);
        if (handIcon != null) handIcon.gameObject.SetActive(visible);
    }
 
    void Update()
    {
        Camera playerCamera = Camera.main;
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        onTarget = false;
        selectedObject = null;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            InteractableObject interactableObject = hit.transform.GetComponentInParent<InteractableObject>();

            if (interactableObject != null && interactableObject.IsInRange(hit.distance))
            {
                onTarget = true;
                selectedObject = interactableObject.gameObject;
                interaction_text.text = interactableObject.CanBePickedUp
                    ? $"{interactableObject.GetItemName()} - Chuột trái để nhặt"
                    : interactableObject.GetItemName();
                interaction_info_ui.SetActive(true);

                // Show the hand icon only for items that can be picked up
                SetHandIcon(interactableObject.CompareTag("pickable"));
            }
            else
            {
                interaction_info_ui.SetActive(false);
                SetHandIcon(false);
            }

        }
        else
        {
            interaction_info_ui.SetActive(false);
            SetHandIcon(false);
        }
    }


public bool IsTarget(InteractableObject interactableObject)
    {
        return onTarget && selectedObject == interactableObject.gameObject;
    }

public bool AddToInventory(string itemName, int quantity)
    {
        InventorySystem inventory = InventorySystem.Instance;
        if (inventory == null || !inventory.HasSpace(quantity))
        {
            Debug.Log("Inventory Full");
            return false;
        }

        for (int itemIndex = 0; itemIndex < quantity; itemIndex++)
        {
            if (!inventory.AddToInventory(itemName))
            {
                return false;
            }
        }

        return true;
    }

public int GetItemQuantity(string itemName)
    {
        InventorySystem inventory = InventorySystem.Instance;
        if (inventory == null)
        {
            return 0;
        }

        int quantity = 0;
        foreach (string inventoryItem in inventory.itemList)
        {
            if (inventoryItem == itemName)
            {
                quantity++;
            }
        }

        return quantity;
    }


void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
