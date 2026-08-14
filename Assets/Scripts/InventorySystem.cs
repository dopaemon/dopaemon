using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    public GameObject inventoryScreenUI;
    public bool isFull;
    public bool isOpen { get; private set; }

    GameObject itemToAdd;
    GameObject slotToEquip;

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
        PopulateSlotList();
        SetOpen(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetOpen(!isOpen);
        }
    }

    public void PopulateSlotList()
    {
        slotList.Clear();

        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    public bool CheckIfFull()
    {
        int occupiedSlots = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                occupiedSlots++;
            }
        }

        isFull = occupiedSlots >= slotList.Count;
        return isFull;
    }

    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return null;
    }

    public bool AddToInventory(string itemName)
    {
        if (CheckIfFull())
        {
            Debug.Log("Inventory Full");
            return false;
        }

        slotToEquip = FindNextEmptySlot();
        GameObject itemPrefab = Resources.Load<GameObject>(itemName);

        if (slotToEquip == null || itemPrefab == null)
        {
            Debug.LogError($"Inventory item prefab not found: Resources/{itemName}");
            return false;
        }

        itemToAdd = Instantiate(itemPrefab, slotToEquip.transform);
        itemToAdd.name = itemName;
        itemToAdd.transform.localPosition = Vector3.zero;
        itemToAdd.transform.localRotation = Quaternion.identity;
        itemToAdd.transform.localScale = Vector3.one;
        InventoryItem inventoryItem = itemToAdd.GetComponent<InventoryItem>();
        if (inventoryItem != null)
        {
            inventoryItem.itemName = itemName;
        }

        itemList.Add(itemName);
        CheckIfFull();
        return true;
    }

    public bool HasSpace(int quantity = 1)
    {
        int emptySlots = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                emptySlots++;
            }
        }

        return emptySlots >= quantity;
    }

    void SetOpen(bool open)
    {
        isOpen = open;
        inventoryScreenUI.SetActive(open);

        bool craftingOpen = CraftingSystem.Instance != null && CraftingSystem.Instance.isOpen;
        Cursor.lockState = open || craftingOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open || craftingOpen;
    }

    public int GetItemCount(string itemName)
    {
        RecalculateList();
        return itemList.FindAll(item => item == itemName).Count;
    }

    public bool RemoveItem(string itemName, int amount)
    {
        if (amount <= 0 || GetItemCount(itemName) < amount)
        {
            return false;
        }

        int removed = 0;
        for (int slotIndex = slotList.Count - 1; slotIndex >= 0 && removed < amount; slotIndex--)
        {
            Transform slot = slotList[slotIndex].transform;
            if (slot.childCount == 0 || NormalizeItemName(slot.GetChild(0).gameObject) != itemName)
            {
                continue;
            }

            Destroy(slot.GetChild(0).gameObject);
            removed++;
        }

        return removed == amount;
    }

    public void RecalculateList()
    {
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                itemList.Add(NormalizeItemName(slot.transform.GetChild(0).gameObject));
            }
        }

        CheckIfFull();
    }

    string NormalizeItemName(GameObject item)
    {
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
        return inventoryItem != null && !string.IsNullOrEmpty(inventoryItem.itemName)
            ? inventoryItem.itemName
            : item.name.Replace("(Clone)", string.Empty).Trim();
    }


public bool DropItem(string itemName, GameObject inventoryItem)
    {
        GameObject worldPrefab = Resources.Load<GameObject>("WorldItems/" + itemName);
        Camera playerCamera = Camera.main;

        if (worldPrefab == null || playerCamera == null || inventoryItem == null)
        {
            Debug.LogError($"World item prefab not found: Resources/WorldItems/{itemName}");
            return false;
        }

        Vector3 dropPosition = playerCamera.transform.position + playerCamera.transform.forward * 2f;
        if (Physics.Raycast(dropPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            dropPosition.y = hit.point.y + 0.03f;
        }

        Instantiate(worldPrefab, dropPosition, Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y, 0f));
        itemList.Remove(itemName);
        Destroy(inventoryItem);
        CheckIfFull();
        return true;
    }
}
