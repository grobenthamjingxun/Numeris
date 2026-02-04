using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Shop Items")]
    public List<ShopItem> availableShopItems = new List<ShopItem>();

    [Header("UI References")]
    public Transform shopItemContent;
    public GameObject shopItemPrefab;
    public TextMeshProUGUI playerCoinsText;
    public GameObject purchaseSuccessPanel;
    public GameObject notEnoughCoinsPanel;

    [Header("Player Data")]
    private int playerCoins;

    private bool isInitialized = false;

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

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("=== ShopManager Initialize ===");
        Debug.Log($"Available shop items count: {availableShopItems.Count}");
        
        if (shopItemContent == null)
        {
            Debug.LogError("Shop Item Content is NULL! Please assign it in the Inspector.");
            return;
        }
        
        if (shopItemPrefab == null)
        {
            Debug.LogError("Shop Item Prefab is NULL! Please assign it in the Inspector.");
            return;
        }

        LoadPlayerCoins();
        isInitialized = true;
        
        Debug.Log("ShopManager initialized successfully");
    }

    // Add this new method
    public void OnPlayerDataLoaded()
    {
        Debug.Log("=== Player data loaded, refreshing shop ===");
        LoadPlayerCoins();
        
        // If shop is currently displayed, refresh the items too
        if (shopItemContent != null && shopItemContent.childCount > 0)
        {
            DisplayShopItems();
        }
    }

    // Also update LoadPlayerCoins to be more defensive
    private void LoadPlayerCoins()
    {
        if (PlayerManager.Instance != null)
        {
            Player player = PlayerManager.Instance.GetPlayerData();
            if (player != null)
            {
                playerCoins = player.coins;
                Debug.Log($"Loaded player coins from Player object: {playerCoins}");
            }
            else
            {
                playerCoins = PlayerManager.Instance.GetCoins();
                Debug.Log($"Loaded player coins from GetCoins (might be default): {playerCoins}");
            }
        }
        else
        {
            Debug.LogWarning("PlayerManager.Instance is null! Using default coins.");
            playerCoins = 100;
        }
        
        UpdateCoinsDisplay();
    }

    public void UpdateCoinsDisplay()
    {
        if (playerCoinsText != null)
        {
            Debug.Log($"Updating coins display: {playerCoins}");
            playerCoinsText.text = playerCoins.ToString();
        }
    }
    public void UpdatePlayerCoinsDisplay(int newCoinAmount)
    {
        playerCoins = newCoinAmount;
        UpdateCoinsDisplay();
    }


    // Call this method when opening the shop UI
    public void OpenShop()
    {
        Debug.Log("=== Opening Shop ===");
        
        if (!isInitialized)
        {
            Initialize();
        }
        
        LoadPlayerCoins();
        DisplayShopItems();
    }

    public void DisplayShopItems()
    {
        Debug.Log("=== DisplayShopItems called ===");
        
        if (shopItemContent == null)
        {
            Debug.LogError("Shop Item Content is NULL!");
            return;
        }
        
        if (shopItemPrefab == null)
        {
            Debug.LogError("Shop Item Prefab is NULL!");
            return;
        }

        if (availableShopItems.Count == 0)
        {
            Debug.LogWarning("No shop items available! Add ShopItem ScriptableObjects to the availableShopItems list in the Inspector.");
            return;
        }

        Debug.Log($"Displaying {availableShopItems.Count} shop items");

        // Clear existing items
        foreach (Transform child in shopItemContent)
        {
            Destroy(child.gameObject);
        }

        // Display all shop items
        int itemIndex = 0;
        foreach (ShopItem shopItem in availableShopItems)
        {
            if (shopItem == null)
            {
                Debug.LogWarning($"Shop item at index {itemIndex} is NULL! Skipping...");
                itemIndex++;
                continue;
            }

            Debug.Log($"Creating UI for: {shopItem.shopItemName} - Price: {shopItem.price}");

            GameObject itemObj = Instantiate(shopItemPrefab, shopItemContent);

            // Get UI components
            TextMeshProUGUI itemName = itemObj.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemPrice = itemObj.transform.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
            Image itemIcon = itemObj.transform.Find("ItemIcon")?.GetComponent<Image>();
            Button buyButton = itemObj.transform.Find("BuyButton")?.GetComponent<Button>();

            // Debug UI components
            if (itemName == null) Debug.LogWarning("ItemName TextMeshProUGUI not found in prefab!");
            if (itemPrice == null) Debug.LogWarning("ItemPrice TextMeshProUGUI not found in prefab!");
            if (itemIcon == null) Debug.LogWarning("ItemIcon Image not found in prefab!");
            if (buyButton == null) Debug.LogWarning("BuyButton Button not found in prefab!");

            // Set values
            if (itemName != null)
            {
                itemName.text = shopItem.shopItemName;
            }
            if (itemPrice != null)
            {
                itemPrice.text = $"{shopItem.price} Coins";
            }
            if (itemIcon != null && shopItem.shopItemIcon != null)
            {
                itemIcon.sprite = shopItem.shopItemIcon;
            }

            // Setup buy button
            if (buyButton != null)
            {
                ShopItem currentItem = shopItem;
                buyButton.onClick.RemoveAllListeners(); // Clear any existing listeners
                buyButton.onClick.AddListener(() => {
                    Debug.Log($"BUY BUTTON CLICKED for {currentItem.shopItemName}!");
                    PurchaseItem(currentItem);
                    });

                buyButton.interactable = (playerCoins >= shopItem.price);
                
                Debug.Log($"Button for {shopItem.shopItemName} - Interactable: {buyButton.interactable}");
            }

            itemIndex++;
        }

        Debug.Log("=== Shop items displayed successfully ===");
    }

    
    public void PurchaseItem(ShopItem shopItem)
    {
        Debug.Log($"=== Purchase attempt: {shopItem.shopItemName} ===");
        Debug.Log($"PlayerManager.Instance is null? {PlayerManager.Instance == null}");

        // Check if player has enough coins
        if (playerCoins < shopItem.price)
        {
            Debug.Log($"Not enough coins! Have: {playerCoins}, Need: {shopItem.price}");
            ShowNotEnoughCoinsPanel();
            return;
        }

        // Deduct coins
        playerCoins -= shopItem.price;
        PlayerManager.Instance.SetCoins(playerCoins);
        UpdateCoinsDisplay();

        Debug.Log($"Coins deducted. Remaining: {playerCoins}");

        // Add item to inventory
        AddItemToInventory(shopItem);

        // Save to Firebase
        SavePurchaseToFirebase(shopItem);

        // Show success message
        ShowPurchaseSuccessPanel(shopItem);

        // Refresh shop display
        DisplayShopItems();

        Debug.Log($"Purchase completed: {shopItem.shopItemName}");
    }

    private void AddItemToInventory(ShopItem shopItem)
    {
        if (InvenManager.instance == null)
        {
            Debug.LogError("InvenManager.instance is NULL!");
            return;
        }

        if (shopItem.correspondingInventoryItem == null)
        {
            Debug.LogWarning($"No corresponding inventory item for {shopItem.shopItemName}");
            return;
        }

        // Find if item already exists in inventory
        InventoryItem existingItem = InvenManager.instance.invenItemList.Find(
            item => item.invenId == shopItem.correspondingInventoryItem.invenId
        );

        if (existingItem != null)
        {
            // Item already exists, just increase quantity
            existingItem.invenQuantity += 1;
            Debug.Log($"Increased quantity of {existingItem.invenItemName} to {existingItem.invenQuantity}");
        }
        else
        {
            // Create a new runtime instance (don't modify the ScriptableObject asset)
            InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
            runtimeItem.invenId = shopItem.correspondingInventoryItem.invenId;
            runtimeItem.invenItemName = shopItem.correspondingInventoryItem.invenItemName;
            runtimeItem.invenIcon = shopItem.correspondingInventoryItem.invenIcon;
            runtimeItem.itemTier = ConvertShopTierToInventoryTier(shopItem.itemTier);
            runtimeItem.invenQuantity = 1;

            // Add to inventory list
            InvenManager.instance.invenItemList.Add(runtimeItem);
            Debug.Log($"Added new item: {runtimeItem.invenItemName}");
        }

        // Save inventory to Firebase
        InvenManager.instance.SaveInventoryToFirebase();
    }

    // Helper method to convert ShopItem tier to InventoryItem tier
    private ItemTier ConvertShopTierToInventoryTier(ShopItem.ItemTier shopTier)
    {
        switch (shopTier)
        {
            case ShopItem.ItemTier.Common:
                return ItemTier.Common;
            case ShopItem.ItemTier.Rare:
                return ItemTier.Rare;
            case ShopItem.ItemTier.Epic:
                return ItemTier.Epic;
            default:
                return ItemTier.Common;
        }
    }

    private void SavePurchaseToFirebase(ShopItem shopItem)
    {
        // The inventory is already saved via InvenManager.SaveInventoryToFirebase()
        // But we also need to update coins in Firebase
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.UpdatePlayerField("coins", playerCoins,
                onSuccess: () => Debug.Log("Coins updated in Firebase"),
                onError: (error) => Debug.LogError("Failed to update coins: " + error)
            );
        }
    }

    private void ShowPurchaseSuccessPanel(ShopItem shopItem)
    {
        if (purchaseSuccessPanel != null)
        {
            purchaseSuccessPanel.SetActive(true);
            
            // Auto-hide after 2 seconds
            Invoke(nameof(HidePurchaseSuccessPanel), 2f);
        }
    }

    private void HidePurchaseSuccessPanel()
    {
        if (purchaseSuccessPanel != null)
        {
            purchaseSuccessPanel.SetActive(false);
        }
    }

    private void ShowNotEnoughCoinsPanel()
    {
        if (notEnoughCoinsPanel != null)
        {
            notEnoughCoinsPanel.SetActive(true);
            
            // Auto-hide after 2 seconds
            Invoke(nameof(HideNotEnoughCoinsPanel), 2f);
        }
    }

    private void HideNotEnoughCoinsPanel()
    {
        if (notEnoughCoinsPanel != null)
        {
            notEnoughCoinsPanel.SetActive(false);
        }
    }
}