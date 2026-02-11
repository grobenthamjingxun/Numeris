using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvenManager : MonoBehaviour
{
    public static InvenManager instance;
    public List<InventoryItem> invenItemList = new List<InventoryItem>();

    public Transform invenItemContent;
    public GameObject invenItemPrefab;

    private bool isLoadingFromFirebase = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Item Management

    public void AddItem(InventoryItem itemToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogError("Cannot add null item to inventory!");
            return;
        }

        InventoryItem existingItem = FindItemById(itemToAdd.invenId);
        AudioManager.Instance.PlayAddItem();
        if (existingItem != null)
        {
            existingItem.invenQuantity += itemToAdd.invenQuantity;
            Debug.Log($"Increased quantity of: {existingItem.invenItemName} to {existingItem.invenQuantity}");
        }
        else
        {
            InventoryItem runtimeItem = CreateRuntimeItem(itemToAdd, itemToAdd.invenQuantity);
            invenItemList.Add(runtimeItem);
            Debug.Log($"Added new item: {runtimeItem.invenItemName} (ID: {runtimeItem.invenId})");
        }

        RefreshInventory();
    }

    public void UseItem(int itemId)
    {
        Debug.Log($"=== UseItem called with ID: {itemId} ===");

        InventoryItem existingItem = FindItemById(itemId);

        if (existingItem == null)
        {
            Debug.LogError($"Item with ID {itemId} NOT FOUND in inventory!");
            DebugInventory();
            return;
        }

        Debug.Log($"Found item: {existingItem.invenItemName}, Quantity: {existingItem.invenQuantity}, PowerUpType: {existingItem.powerUpType}");

        ApplyItemEffect(existingItem);
        RemoveItem(itemId, 1);

        Debug.Log($"Used item: {existingItem.invenItemName} (ID: {itemId})");
    }

    public void RemoveItem(int itemId, int quantity = 1)
    {
        InventoryItem existingItem = FindItemById(itemId);

        if (existingItem == null)
        {
            Debug.LogWarning($"Attempted to remove item with ID {itemId} but it wasn't found");
            return;
        }

        existingItem.invenQuantity -= quantity;
        Debug.Log($"Removed {quantity} of {existingItem.invenItemName}. Remaining: {existingItem.invenQuantity}");

        if (existingItem.invenQuantity <= 0)
        {
            invenItemList.Remove(existingItem);
            Debug.Log($"Item {existingItem.invenItemName} completely removed from inventory");
        }

        RefreshInventory();
    }

    private InventoryItem FindItemById(int itemId)
    {
        return invenItemList.Find(item => item.invenId == itemId);
    }

    private InventoryItem CreateRuntimeItem(InventoryItem sourceItem, int quantity)
    {
        InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
        runtimeItem.invenId = sourceItem.invenId;
        runtimeItem.invenItemName = sourceItem.invenItemName;
        runtimeItem.invenIcon = sourceItem.invenIcon;
        runtimeItem.itemTier = sourceItem.itemTier;
        runtimeItem.powerUpType = sourceItem.powerUpType;
        runtimeItem.invenQuantity = quantity;
        return runtimeItem;
    }

    #endregion

    #region Item Effects

    private void ApplyItemEffect(InventoryItem item)
    {
        if (PowerUpManager.Instance == null)
        {
            Debug.LogError("PowerUpManager.Instance is null! Make sure PowerUpManager exists in the scene.");
            return;
        }

        Debug.Log($"Applying effect for PowerUpType: {item.powerUpType}");

        switch (item.powerUpType)
        {
            case PowerUpType.HealthPotion:
                Debug.Log("Using Health Potion");
                PowerUpManager.Instance.UseHealth();
                break;

            case PowerUpType.FiftyFifty:
                Debug.Log("Using 50:50 Power-Up");
                PowerUpManager.Instance.UseFiftyFifty();
                break;

            case PowerUpType.SwitchQuestion:
                Debug.Log("Using Switch Question Power-Up");
                PowerUpManager.Instance.UseSwitchQuestion();
                break;

            case PowerUpType.None:
            default:
                Debug.LogWarning($"No power-up assigned for item: {item.invenItemName}");
                break;
        }
        AudioManager.Instance.PlayUseItem();
    }

    #endregion

    #region UI Management

    public void DisplayInventory()
    {
        Debug.Log($"=== DisplayInventory - Total items: {invenItemList.Count} ===");

        if (!ValidateUIReferences())
        {
            return;
        }
        // Clear Inventory UI
        foreach (Transform child in invenItemContent)
        {
            Destroy(child.gameObject);
        }
        // Populate Inventory UI
        foreach (InventoryItem invenItem in invenItemList)
        {
            if (invenItem == null)
            {
                Debug.LogError("Null item found in inventory list!");
                continue;
            }

            GameObject itemObj = Instantiate(invenItemPrefab);

            if (itemObj == null)
            {
                Debug.LogError("Failed to instantiate inventory item prefab!");
                continue;
            }

            itemObj.transform.SetParent(invenItemContent, false);
            SetupInventoryItemUI(itemObj, invenItem);
        }
        Debug.Log($"Successfully displayed {invenItemList.Count} inventory items");
    }

    private bool ValidateUIReferences()
    {
        if (invenItemContent == null)
        {
            Debug.LogWarning("invenItemContent is null - UI may not be loaded yet");
            return false;
        }
        if (invenItemPrefab == null)
        {
            Debug.LogError("invenItemPrefab is NULL! Assign it in the Inspector.");
            return false;
        }
        return true;
    }

    private void SetupInventoryItemUI(GameObject itemObj, InventoryItem invenItem)
    {
        if (itemObj == null || invenItem == null)
        {
            Debug.LogError("Null reference in SetupInventoryItemUI!");
            return;
        }
        // Setup UI Elements
        TextMeshProUGUI itemName = FindChildComponent<TextMeshProUGUI>(itemObj.transform, "InvenItemName");
        Image itemImage = FindChildComponent<Image>(itemObj.transform, "InvenImage");
        TextMeshProUGUI itemQuantity = FindChildComponent<TextMeshProUGUI>(itemObj.transform, "InvenItemQuantity");
        if (itemName != null)
        {
            itemName.text = invenItem.invenItemName;
        }
        if (itemImage != null)
        {
            if (invenItem.invenIcon != null)
            {
                itemImage.sprite = invenItem.invenIcon;
            }
            else
            {
                Debug.LogWarning($"Icon is null for item: {invenItem.invenItemName}");
            }
        }
        if (itemQuantity != null)
        {
            itemQuantity.text = invenItem.invenQuantity.ToString();
        }
        // Setup Use Button
        Button useButton = itemObj.GetComponent<Button>();
        if (useButton == null)
        {
            Debug.LogError("Button component not found on InvenItem prefab!");
            return;
        }
        int currentItemId = invenItem.invenId;
        string currentItemName = invenItem.invenItemName;
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => {
            Debug.Log($"=== USE BUTTON CLICKED - ID: {currentItemId}, Name: {currentItemName} ===");
            UseItem(currentItemId);
        });
        Debug.Log($"Button setup complete for {currentItemName} (ID: {currentItemId})");
    }

    // Helper method to find child components
    private T FindChildComponent<T>(Transform parent, string childName) where T : Component
    {
        Transform child = parent.Find(childName);
        if (child == null)
        {
            Debug.LogError($"{childName} not found in prefab! Check your prefab structure.");
            return null;
        }

        T component = child.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"{typeof(T).Name} component not found on {childName}!");
            return null;
        }

        return component;
    }

    public void SetInventoryUIReference(Transform contentTransform)
    {
        invenItemContent = contentTransform;
        Debug.Log("Inventory UI reference set");
    }

    public void OpenInventoryUI()
    {
        Debug.Log("Opening inventory - Loading from Firebase...");

        if (TryFindInventoryPanel())
        {
            LoadInventoryFromFirebase();
        }
        AudioManager.Instance.PlayOpenInventory();
    }

    private bool TryFindInventoryPanel()
    {
        GameObject inventoryPanel = GameObject.Find("InventoryPanel");
        if (inventoryPanel == null)
        {
            Debug.LogWarning("InventoryPanel GameObject not found in scene");
            return false;
        }
        Transform content = inventoryPanel.transform.Find("Content");

        if (content == null)
        {
            Debug.LogError("Content child not found in InventoryPanel!");
            return false;
        }
        invenItemContent = content;
        Debug.Log("Found inventory content panel");
        return true;
    }

    #endregion

    #region Firebase Integration

    public void SaveInventoryToFirebase()
    {
        Debug.Log("SaveInventoryToFirebase called");
        if (FirebaseInventoryManager.Instance == null)
        {
            Debug.LogError("FirebaseInventoryManager.Instance is NULL!");
            return;
        }
        List<Inventory> inventoryList = ConvertToFirebaseFormat();
        FirebaseInventoryManager.Instance.SaveInventory(inventoryList,
            onSuccess: () => Debug.Log("Inventory saved to Firebase successfully"),
            onError: (error) => Debug.LogError($"Failed to save inventory: {error}")
        );
    }

    private List<Inventory> ConvertToFirebaseFormat()
    {
        List<Inventory> inventoryList = new List<Inventory>();
        foreach (InventoryItem item in invenItemList)
        {
            if (item == null) continue;
            Collectible collectible = new Collectible(
                item.invenId.ToString(),
                item.invenItemName,
                item.itemTier.ToString(),
                item.invenQuantity
            );
            Inventory inventory = new Inventory(item.invenId.ToString());
            inventory.collectibleDetails = collectible;
            inventoryList.Add(inventory);
        }
        return inventoryList;
    }

    public void LoadInventoryFromFirebase()
    {
        Debug.Log("=== LoadInventoryFromFirebase called ===");
        if (isLoadingFromFirebase)
        {
            Debug.LogWarning("Already loading inventory, skipping duplicate call");
            return;
        }
        if (FirebaseInventoryManager.Instance == null)
        {
            Debug.LogError("FirebaseInventoryManager.Instance is NULL!");
            return;
        }
        isLoadingFromFirebase = true;
        FirebaseInventoryManager.Instance.LoadInventory(
            onSuccess: (inventoryList) => OnInventoryLoadSuccess(inventoryList),
            onError: (error) => OnInventoryLoadError(error)
        );
    }

    private void OnInventoryLoadSuccess(List<Inventory> inventoryList)
    {
        invenItemList.Clear();
        Debug.Log($"Received {inventoryList.Count} items from Firebase");
        foreach (Inventory inventory in inventoryList)
        {
            if (!ValidateFirebaseInventoryItem(inventory))
            {
                continue;
            }
            LoadInventoryItemFromFirebase(inventory);
        }
        Debug.Log($"Loaded {invenItemList.Count} items from Firebase");
        DebugInventory();
        DisplayInventory();
        isLoadingFromFirebase = false;
    }

    private void OnInventoryLoadError(string error)
    {
        Debug.LogError($"Failed to load inventory: {error}");
        isLoadingFromFirebase = false;
    }

    private bool ValidateFirebaseInventoryItem(Inventory inventory)
    {
        if (inventory == null || inventory.collectibleDetails == null)
        {
            Debug.LogWarning("Null inventory or collectible details, skipping");
            return false;
        }
        return true;
    }

    private void LoadInventoryItemFromFirebase(Inventory inventory)
    {
        Collectible collectible = inventory.collectibleDetails;
        if (!int.TryParse(collectible.collectibleID, out int itemId))
        {
            Debug.LogWarning($"Invalid collectible ID: {collectible.collectibleID}");
            return;
        }
        InventoryItem itemAsset = GetInventoryItemAsset(itemId);
        if (itemAsset != null)
        {
            InventoryItem runtimeItem = CreateRuntimeItem(itemAsset, collectible.quantity);
            invenItemList.Add(runtimeItem);
            Debug.Log($"Loaded item: {runtimeItem.invenItemName} (ID: {runtimeItem.invenId}, PowerUp: {runtimeItem.powerUpType})");
        }
        else
        {
            Debug.LogWarning($"Could not find inventory item asset with ID: {collectible.collectibleID}");
        }
    }

    private InventoryItem GetInventoryItemAsset(int itemId)
    {
        InventoryItem[] allItems = Resources.LoadAll<InventoryItem>("InventoryItems");
        if (allItems == null || allItems.Length == 0)
        {
            Debug.LogError("No InventoryItems found in Resources/InventoryItems folder!");
            return null;
        }
        foreach (InventoryItem item in allItems)
        {
            if (item != null && item.invenId == itemId)
            {
                return item;
            }
        }
        Debug.LogWarning($"No InventoryItem found with ID {itemId}. Available IDs: {string.Join(", ", Array.ConvertAll(allItems, i => i.invenId.ToString()))}");
        return null;
    }

    private void RefreshInventory()
    {
        DisplayInventory();
        SaveInventoryToFirebase();
    }

    #endregion

    #region Debug

    public void DebugInventory()
    {
        Debug.Log("=== CURRENT INVENTORY ===");
        if (invenItemList.Count == 0)
        {
            Debug.Log("Inventory is EMPTY!");
        }
        else
        {
            foreach (var item in invenItemList)
            {
                if (item != null)
                {
                    Debug.Log($"ID: {item.invenId} | Name: {item.invenItemName} | Quantity: {item.invenQuantity} | PowerUp: {item.powerUpType}");
                }
                else
                {
                    Debug.LogError("NULL ITEM in inventory list!");
                }
            }
        }
        Debug.Log("========================");
    }
    #endregion
}