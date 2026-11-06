using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSystem : MonoBehaviour
{
    [System.Serializable]
    class Blueprint
    {
        public string itemName;
        public string firstRequirement;
        public int firstAmount;
        public string secondRequirement;
        public int secondAmount;

        public Blueprint(string itemName, string firstRequirement, int firstAmount, string secondRequirement, int secondAmount)
        {
            this.itemName = itemName;
            this.firstRequirement = firstRequirement;
            this.firstAmount = firstAmount;
            this.secondRequirement = secondRequirement;
            this.secondAmount = secondAmount;
        }
    }

    static readonly Blueprint AxeBlueprint = new Blueprint("Axe", "Gold", 3, "Stick", 3);

    public static CraftingSystem Instance { get; private set; }
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public Button toolsButton;
    public Button backButton;
    public Button craftAxeButton;
    public TMP_Text goldRequirementText;
    public TMP_Text stickRequirementText;
    public bool isOpen { get; private set; }

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
        BindUiReferences();
        if (toolsButton == null || craftAxeButton == null || goldRequirementText == null || stickRequirementText == null)
        {
            Debug.LogError("Crafting UI is missing a required reference.");
            return;
        }

        toolsButton.onClick.AddListener(OpenToolsCategory);
        if (backButton != null)
        {
            backButton.onClick.AddListener(OpenCraftingScreen);
        }

        craftAxeButton.onClick.AddListener(CraftAxe);
        Close();
    }

    void BindUiReferences()
    {
        if (craftingScreenUI != null && toolsButton == null)
        {
            Transform toolsButtonTransform = craftingScreenUI.transform.Find("ToolsButton");
            if (toolsButtonTransform != null)
            {
                toolsButton = toolsButtonTransform.GetComponent<Button>();
            }
        }

        if (toolsScreenUI == null)
        {
            return;
        }

        Transform axeTransform = toolsScreenUI.transform.Find("Axe");
        if (axeTransform == null)
        {
            return;
        }

        if (goldRequirementText == null)
        {
            goldRequirementText = axeTransform.Find("req1")?.GetComponent<TMP_Text>();
        }

        if (stickRequirementText == null)
        {
            stickRequirementText = axeTransform.Find("req2")?.GetComponent<TMP_Text>();
        }

        if (craftAxeButton == null)
        {
            craftAxeButton = axeTransform.Find("Button")?.GetComponent<Button>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
            return;
        }

        OpenCraftingScreen();
    }

    public void OpenCraftingScreen()
    {
        craftingScreenUI.SetActive(true);
        toolsScreenUI.SetActive(false);
        OpenCursor();
    }

    public void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        RefreshNeededItems();
    }

    public void Close()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        isOpen = false;

        if (InventorySystem.Instance == null || !InventorySystem.Instance.isOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OpenCursor()
    {
        isOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RefreshNeededItems()
    {
        if (goldRequirementText == null || stickRequirementText == null || craftAxeButton == null)
        {
            return;
        }

        int goldCount = GetItemCount(AxeBlueprint.firstRequirement);
        int stickCount = GetItemCount(AxeBlueprint.secondRequirement);
        goldRequirementText.text = $"Gold: {goldCount}/{AxeBlueprint.firstAmount}";
        stickRequirementText.text = $"Stick: {stickCount}/{AxeBlueprint.secondAmount}";
        craftAxeButton.gameObject.SetActive(goldCount >= AxeBlueprint.firstAmount && stickCount >= AxeBlueprint.secondAmount);
    }

    int GetItemCount(string itemName)
    {
        return InventorySystem.Instance == null ? 0 : InventorySystem.Instance.GetItemCount(itemName);
    }

    void CraftAxe()
    {
        CraftAnyItem(AxeBlueprint);
    }

    void CraftAnyItem(Blueprint blueprint)
    {
        if (GetItemCount(blueprint.firstRequirement) < blueprint.firstAmount ||
            GetItemCount(blueprint.secondRequirement) < blueprint.secondAmount)
        {
            RefreshNeededItems();
            return;
        }

        InventorySystem inventory = InventorySystem.Instance;
        if (!inventory.RemoveItem(blueprint.firstRequirement, blueprint.firstAmount) ||
            !inventory.RemoveItem(blueprint.secondRequirement, blueprint.secondAmount))
        {
            inventory.RecalculateList();
            return;
        }

        StartCoroutine(FinishCraft(blueprint));
    }

    IEnumerator FinishCraft(Blueprint blueprint)
    {
        yield return 0;

        InventorySystem inventory = InventorySystem.Instance;
        inventory.RecalculateList();
        if (!inventory.AddToInventory(blueprint.itemName))
        {
            Debug.LogError($"Crafted item prefab not found: Resources/{blueprint.itemName}");
        }

        RefreshNeededItems();
    }
}
