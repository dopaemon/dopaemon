using UnityEngine;

public class Inventory : MonoBehaviour
{
    static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<Inventory>();
            }

            return instance;
        }
        private set { instance = value; }
    }

    public GameObject inventoryScreen;
    public GameObject goldCoinPrefab;
    public bool isOpen { get; private set; }

    ItemSlot[] slots;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        slots = inventoryScreen.GetComponentsInChildren<ItemSlot>(true);
        SetOpen(false);
    }

void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetOpen(!isOpen);
        }
    }

    public bool HasSpace(int quantity = 1)
    {
        int emptySlots = 0;
        foreach (ItemSlot slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                emptySlots++;
            }
        }

        return emptySlots >= quantity;
    }

    public bool AddItem(string itemName, int quantity)
    {
        if (itemName != "Gold" || goldCoinPrefab == null || !HasSpace(quantity))
        {
            return false;
        }

        for (int itemIndex = 0; itemIndex < quantity; itemIndex++)
        {
            foreach (ItemSlot slot in slots)
            {
                if (slot.transform.childCount != 0)
                {
                    continue;
                }

                GameObject item = Instantiate(goldCoinPrefab, slot.transform);
                item.name = "GoldCoin_INV";
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                break;
            }
        }

        return true;
    }

    void SetOpen(bool open)
    {
        isOpen = open;
        inventoryScreen.SetActive(open);
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }
}
