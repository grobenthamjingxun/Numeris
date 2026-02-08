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

    public void AddItem(InventoryItem itemToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogError("Cannot add null item to inventory!");
            return;
        }

        // Find existing item by ID
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemToAdd.invenId);

        if (existingItem != null)
        {
            existingItem.invenQuantity += itemToAdd.invenQuantity;
            Debug.Log($"Increased quantity of: {existingItem.invenItemName} to {existingItem.invenQuantity}");
        }
        else
        {
            // Create runtime copy to avoid modifying ScriptableObject asset
            InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
            runtimeItem.invenId = itemToAdd.invenId;
            runtimeItem.invenItemName = itemToAdd.invenItemName;
            runtimeItem.invenIcon = itemToAdd.invenIcon;
            runtimeItem.itemTier = itemToAdd.itemTier;
            runtimeItem.powerUpType = itemToAdd.powerUpType; // CRITICAL: Copy powerUpType
            runtimeItem.invenQuantity = itemToAdd.invenQuantity;

            invenItemList.Add(runtimeItem);
            Debug.Log($"Added new item: {runtimeItem.invenItemName} (ID: {runtimeItem.invenId})");
        }

        // Sync to Firebase and refresh UI
        DisplayInventory();
        SaveInventoryToFirebase();
    }

    public void UseItem(int itemId)
    {
        Debug.Log($"=== UseItem called with ID: {itemId} ===");

        // Find the item BEFORE removing
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemId);

        if (existingItem == null)
        {
            Debug.LogError($"Item with ID {itemId} NOT FOUND in inventory!");
            DebugInventory();
            return;
        }

        Debug.Log($"Found item: {existingItem.invenItemName}, Quantity: {existingItem.invenQuantity}, PowerUpType: {existingItem.powerUpType}");

        // Apply item effect BEFORE removing
        ApplyItemEffect(existingItem);

        // Now remove the item
        RemoveItem(itemId, 1);

        Debug.Log($"Used item: {existingItem.invenItemName} (ID: {itemId})");
    }

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
    }

    public void RemoveItem(int itemId, int quantity = 1)
    {
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemId);
        if (existingItem != null)
        {
            existingItem.invenQuantity -= quantity;
            Debug.Log($"Removed {quantity} of {existingItem.invenItemName}. Remaining: {existingItem.invenQuantity}");

            if (existingItem.invenQuantity <= 0)
            {
                invenItemList.Remove(existingItem);
                Debug.Log($"Item {existingItem.invenItemName} completely removed from inventory");
            }

            // Sync to Firebase and refresh UI
            DisplayInventory();
            SaveInventoryToFirebase();
        }
        else
        {
            Debug.LogWarning($"Attempted to remove item with ID {itemId} but it wasn't found");
        }
    }

    public void DisplayInventory()
    {
        Debug.Log($"=== DisplayInventory - Total items: {invenItemList.Count} ===");

        // Check if UI references exist
        if (invenItemContent == null)
        {
            Debug.LogWarning("invenItemContent is null - UI may not be loaded yet");
            return;
        }

        if (invenItemPrefab == null)
        {
            Debug.LogError("invenItemPrefab is NULL! Assign it in the Inspector.");
            return;
        }

        // Clear existing items
        foreach (Transform child in invenItemContent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new items
        foreach (InventoryItem invenItem in invenItemList)
        {
            if (invenItem == null)
            {
                Debug.LogError("Null item found in inventory list!");
                continue;
            }

            // Instantiate without parent first
            GameObject itemObj = Instantiate(invenItemPrefab);

            if (itemObj == null)
            {
                Debug.LogError("Failed to instantiate inventory item prefab!");
                continue;
            }

            // Set parent AFTER instantiation with worldPositionStays = false
            itemObj.transform.SetParent(invenItemContent, false);

            // Setup the UI
            SetupInventoryItemUI(itemObj, invenItem);
        }

        Debug.Log($"Successfully displayed {invenItemList.Count} inventory items");
    }

    private void SetupInventoryItemUI(GameObject itemObj, InventoryItem invenItem)
    {
        if (itemObj == null || invenItem == null)
        {
            Debug.LogError("Null reference in SetupInventoryItemUI!");
            return;
        }

        // Find UI components with error checking
        TextMeshProUGUI invenItemName = FindChildComponent<TextMeshProUGUI>(itemObj.transform, "InvenItemName");
        Image invenItemImage = FindChildComponent<Image>(itemObj.transform, "InvenImage");
        TextMeshProUGUI invenItemQuantity = FindChildComponent<TextMeshProUGUI>(itemObj.transform, "InvenItemQuantity");
        Button useButton = itemObj.GetComponent<Button>();

        // Set values with null checks
        if (invenItemName != null)
        {
            invenItemName.text = invenItem.invenItemName;
        }

        if (invenItemImage != null && invenItem.invenIcon != null)
        {
            invenItemImage.sprite = invenItem.invenIcon;
        }
        else if (invenItemImage != null)
        {
            Debug.LogWarning($"Icon is null for item: {invenItem.invenItemName}");
        }

        if (invenItemQuantity != null)
        {
            invenItemQuantity.text = invenItem.invenQuantity.ToString();
        }

        // Setup button
        if (useButton != null)
        {
            // Capture variables in local scope to avoid closure issues
            int currentItemId = invenItem.invenId;
            string currentItemName = invenItem.invenItemName;

            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => {
                Debug.Log($"=== USE BUTTON CLICKED - ID: {currentItemId}, Name: {currentItemName} ===");
                UseItem(currentItemId);
            });

            Debug.Log($"Button setup complete for {currentItemName} (ID: {currentItemId})");
        }
        else
        {
            Debug.LogError("Button component not found on InvenItem prefab!");
        }
    }

    // Helper method to find child components with error reporting
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

    public void SaveInventoryToFirebase()
    {
        Debug.Log("SaveInventoryToFirebase called");

        if (FirebaseInventoryManager.Instance == null)
        {
            Debug.LogError("FirebaseInventoryManager.Instance is NULL!");
            return;
        }

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

        FirebaseInventoryManager.Instance.SaveInventory(inventoryList,
            onSuccess: () => Debug.Log("Inventory saved to Firebase successfully"),
            onError: (error) => Debug.LogError($"Failed to save inventory: {error}")
        );
    }

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

    public void OpenInventoryUI()
    {
        Debug.Log("Opening inventory - Loading from Firebase...");

        // Try to find inventory panel
        GameObject inventoryPanel = GameObject.Find("InventoryPanel");
        if (inventoryPanel != null)
        {
            Transform content = inventoryPanel.transform.Find("Content");
            if (content != null)
            {
                invenItemContent = content;
                Debug.Log("Found inventory content panel");
            }
            else
            {
                Debug.LogError("Content child not found in InventoryPanel!");
            }
        }
        else
        {
            Debug.LogWarning("InventoryPanel GameObject not found in scene");
        }

        LoadInventoryFromFirebase();
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
            onSuccess: (inventoryList) =>
            {
                // Clear current inventory
                invenItemList.Clear();

                Debug.Log($"Received {inventoryList.Count} items from Firebase");

                // Load items from Firebase
                foreach (Inventory inventory in inventoryList)
                {
                    if (inventory == null || inventory.collectibleDetails == null)
                    {
                        Debug.LogWarning("Null inventory or collectible details, skipping");
                        continue;
                    }

                    Collectible collectible = inventory.collectibleDetails;

                    // Parse item ID
                    if (!int.TryParse(collectible.collectibleID, out int itemId))
                    {
                        Debug.LogWarning($"Invalid collectible ID: {collectible.collectibleID}");
                        continue;
                    }

                    // Find the ScriptableObject asset by ID
                    InventoryItem itemAsset = GetInventoryItemAsset(itemId);

                    if (itemAsset != null)
                    {
                        // Create a runtime instance
                        InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
                        runtimeItem.invenId = itemAsset.invenId;
                        runtimeItem.invenItemName = itemAsset.invenItemName;
                        runtimeItem.invenIcon = itemAsset.invenIcon;
                        runtimeItem.itemTier = itemAsset.itemTier;
                        runtimeItem.powerUpType = itemAsset.powerUpType; // CRITICAL: Copy powerUpType
                        runtimeItem.invenQuantity = collectible.quantity;

                        invenItemList.Add(runtimeItem);
                        Debug.Log($"Loaded item: {runtimeItem.invenItemName} (ID: {runtimeItem.invenId}, PowerUp: {runtimeItem.powerUpType})");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find inventory item asset with ID: {collectible.collectibleID}");
                    }
                }

                Debug.Log($"Loaded {invenItemList.Count} items from Firebase");
                DebugInventory();
                DisplayInventory();

                isLoadingFromFirebase = false;
            },
            onError: (error) =>
            {
                Debug.LogError($"Failed to load inventory: {error}");
                isLoadingFromFirebase = false;
            }
        );
    }

    // Helper method to get ScriptableObject by ID
    private InventoryItem GetInventoryItemAsset(int itemId)
    {
        // Load all InventoryItem assets from Resources folder
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
}
