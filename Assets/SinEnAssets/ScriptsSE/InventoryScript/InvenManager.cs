using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InvenManager : MonoBehaviour
{
    public static InvenManager instance;
    public List<InventoryItem> invenItemList = new List<InventoryItem>();

    public Transform invenItemContent;
    public GameObject invenItemPrefab;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(InventoryItem itemToAdd)
    {
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemToAdd.invenId);
        if (existingItem != null)
        {
            existingItem.invenQuantity += itemToAdd.invenQuantity;
            Debug.Log("Increased quantity of: " + existingItem.invenItemName + " to " + existingItem.invenQuantity);
        }
        else
        {
            invenItemList.Add(itemToAdd);
            Debug.Log("Added new item: " + itemToAdd.invenItemName);
        }
        // Sync to Firebase after adding
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
            Debug.Log("Current inventory items:");
            foreach (var item in invenItemList)
            {
                Debug.Log($"  - ID: {item.invenId}, Name: {item.invenItemName}, Quantity: {item.invenQuantity}");
            }
            return;
        }
        
        Debug.Log($"Found item: {existingItem.invenItemName}, Current quantity: {existingItem.invenQuantity}");
        
        // Apply item effect BEFORE removing
        ApplyItemEffect(existingItem);
        
        // Now remove the item
        RemoveItem(itemId, 1);
        
        Debug.Log($"Used item: {existingItem.invenItemName} (ID: {itemId})");
    }
    private void ApplyItemEffect(InventoryItem item)
    {
        // Apply the item's effect based on its ID or type
        switch (item.invenId)
        {
            case 1: // Health Potion
                Debug.Log("Applying health potion effect");
                // Add your health restoration logic here
                if (PlayerManager.Instance != null)
                {
                    int currentHealth = PlayerManager.Instance.GetHealth();
                    PlayerManager.Instance.SetHealth(Mathf.Min(currentHealth + 50, 100)); // Example: heal 50
                }
                break;
                
            case 2: // Shield
                Debug.Log("Applying shield effect");
                // Add your shield logic here
                break;
                
            case 3: // Slow Time
                Debug.Log("Applying slow time effect");
                // Add your slow time logic here
                break;
                
            default:
                Debug.LogWarning($"No effect defined for item ID: {item.invenId}");
                break;
        }
    }
    public void RemoveItem(int itemId, int quantity = 1)
    {
        InventoryItem existingItem = invenItemList.Find(item => item.invenId == itemId);
        if (existingItem != null)
        {
            existingItem.invenQuantity -= quantity;
            if (existingItem.invenQuantity <= 0)
            {
                invenItemList.Remove(existingItem);
            }

            // Sync to Firebase after removing
            DisplayInventory();
            SaveInventoryToFirebase();
        }
    }
    public void DisplayInventory()
    {
        Debug.Log($"=== DisplayInventory - Total items: {invenItemList.Count} ===");
        
        foreach (Transform child in invenItemContent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invenItem in invenItemList)
        {
            GameObject itemObj = Instantiate(invenItemPrefab, invenItemContent);

            // Find and check InvenItemName
            Transform nameTransform = itemObj.transform.Find("InvenItemName");
            if (nameTransform == null)
            {
                Debug.LogError("InvenItemName not found in prefab!");
                continue;
            }
            TextMeshProUGUI invenItemName = nameTransform.GetComponent<TextMeshProUGUI>();
            if (invenItemName == null)
            {
                Debug.LogError("Text component not found on InvenItemName!");
                continue;
            }

            // Find and check InvenImage
            Transform imageTransform = itemObj.transform.Find("InvenImage");
            if (imageTransform == null)
            {
                Debug.LogError("InvenImage not found in prefab!");
                continue;
            }
            Image invenItemImage = imageTransform.GetComponent<Image>();
            if (invenItemImage == null)
            {
                Debug.LogError("Image component not found on InvenImage!");
                continue;
            }

            // Find and check InvenItemQuantity
            Transform quantityTransform = itemObj.transform.Find("InvenItemQuantity");
            if (quantityTransform == null)
            {
                Debug.LogError("InvenItemQuantity not found in prefab!");
                continue;
            }
            TextMeshProUGUI invenItemQuantity = quantityTransform.GetComponent<TextMeshProUGUI>();
            if (invenItemQuantity == null)
            {
                Debug.LogError("Text component not found on InvenItemQuantity!");
                continue;
            }

            // Set values
            invenItemName.text = invenItem.invenItemName;
            if (invenItem.invenIcon != null)
            {
                invenItemImage.sprite = invenItem.invenIcon;
            }
            else
            {
                Debug.LogWarning($"Icon is null for item: {invenItem.invenItemName}");
            }
            invenItemQuantity.text = invenItem.invenQuantity.ToString();

            Button useButton = itemObj.GetComponent<Button>();
        
            if (useButton != null)
            {
                // Capture the current item's ID
                int currentItemId = invenItem.invenId;
                string currentItemName = invenItem.invenItemName;
                
                // Remove any Inspector-assigned listeners
                useButton.onClick.RemoveAllListeners();
                
                // Add the correct listener with the correct ID
                useButton.onClick.AddListener(() => {
                    Debug.Log($"=== USE BUTTON CLICKED - ID: {currentItemId}, Name: {currentItemName} ===");
                    UseItem(currentItemId);
                });
                
                Debug.Log($"Button setup for {currentItemName} with ID: {currentItemId}");
            }
            else
            {
                Debug.LogError("Button component not found on InvenItem prefab!");
            }
        }
    }

    public void SaveInventoryToFirebase()
    {
        Debug.Log("SaveInventoryToFirebase called");
        List<Inventory> inventoryList = new List<Inventory>();

        foreach (InventoryItem item in invenItemList)
        {
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
            onSuccess: () => Debug.Log("Inventory saved to Firebase"),
            onError: (error) => Debug.LogError("Failed to save inventory: " + error)
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
                Debug.Log($"ID: {item.invenId} | Name: {item.invenItemName} | Quantity: {item.invenQuantity}");
            }
        }
        Debug.Log("========================");
    }
    public void OpenInventoryUI()
    {
       Debug.Log("Opening inventory - Loading from Firebase...");
        LoadInventoryFromFirebase();
    }
    public void LoadInventoryFromFirebase()
    {
        Debug.Log("=== LoadInventoryFromFirebase called ===");
        
        if (FirebaseInventoryManager.Instance == null)
        {
            Debug.LogError("FirebaseInventoryManager.Instance is NULL!");
            return;
        }
        FirebaseInventoryManager.Instance.LoadInventory(
            onSuccess: (inventoryList) =>
            {
                // Clear current inventory
                invenItemList.Clear();

                // Load items from Firebase
                foreach (Inventory inventory in inventoryList)
                {
                    Collectible collectible = inventory.collectibleDetails;

                    if (collectible != null)
                    {
                        // Find the ScriptableObject asset by ID
                        int itemId = int.Parse(collectible.collectibleID);
                        InventoryItem itemAsset = GetInventoryItemAsset(itemId);

                        if (itemAsset != null)
                        {
                            // Create a runtime instance
                            InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
                            runtimeItem.invenId = itemAsset.invenId;
                            runtimeItem.invenItemName = itemAsset.invenItemName;
                            runtimeItem.invenIcon = itemAsset.invenIcon;
                            runtimeItem.itemTier = itemAsset.itemTier;
                            runtimeItem.invenQuantity = collectible.quantity;

                            invenItemList.Add(runtimeItem);
                        }
                        else
                        {
                            Debug.LogWarning($"Could not find inventory item asset with ID: {collectible.collectibleID}");
                        }
                    }
                }
                Debug.Log($"Loaded {invenItemList.Count} items from Firebase");
                DebugInventory();
                DisplayInventory();
            },
            onError: (error) => Debug.LogError("Failed to load inventory: " + error)
        );
    }

    // Helper method to get ScriptableObject by ID
    private InventoryItem GetInventoryItemAsset(int itemId)
    {
        // Load all InventoryItem assets from Resources folder
        InventoryItem[] allItems = Resources.LoadAll<InventoryItem>("InventoryItems");

        foreach (InventoryItem item in allItems)
        {
            if (item.invenId == itemId)
            {
                return item;
            }
        }
        return null;
    }
}
