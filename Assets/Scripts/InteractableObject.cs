using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    [Min(0f)] public float InteractionDistance = 3f;
    public bool CanBePickedUp;
    public bool playerInRange;
    [Min(1)] public int Quantity = 1;
 
    public string GetItemName()
    {
        return ItemName;
    }

public bool IsInRange(float distance)
    {
        return distance <= InteractionDistance;
    }

public bool TryPickUp(out string itemName, out int quantity)
    {
        itemName = ItemName;
        quantity = Quantity;
        InventorySystem inventory = InventorySystem.Instance;

        if (!CanBePickedUp || SelectionManager.Instance == null ||
            !SelectionManager.Instance.IsTarget(this) || inventory == null ||
            !inventory.HasSpace(quantity))
        {
            if (inventory != null && !inventory.HasSpace(quantity))
            {
                Debug.Log("Inventory Full");
            }

            return false;
        }

        for (int itemIndex = 0; itemIndex < quantity; itemIndex++)
        {
            if (!inventory.AddToInventory(itemName))
            {
                return false;
            }
        }

        Destroy(gameObject);
        return true;
    }




void Update()
    {
        if (!CanBePickedUp || !Input.GetMouseButtonDown(0) ||
            SelectionManager.Instance == null || !SelectionManager.Instance.IsTarget(this))
        {
            return;
        }

        if (TryPickUp(out string itemName, out int quantity))
        {
            Debug.Log($"Item added to inventory: {itemName} x{quantity}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
