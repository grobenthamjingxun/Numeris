using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Shop Items")]
    public List<ShopItem> availableShopItems = new List<ShopItem>();
    private Transform shopItemContent;
    public GameObject shopItemPrefab;
    private TextMeshProUGUI playerCoinsText;

    [Header("Feedback Panels")]
    private GameObject purchaseSuccessPanel;
    private GameObject notEnoughCoinsPanel;

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
        // Only initialize data, not UI references
        Invoke(nameof(Initialize), 0.1f);
    }

    private void Initialize()
    {
        Debug.Log("=== ShopManager Initialize ===");
        Debug.Log($"Available shop items count: {availableShopItems.Count}");

        if (shopItemPrefab == null)
        {
            Debug.LogError("Shop Item Prefab is NULL! Please assign it in the Inspector.");
            return;
        }

        isInitialized = true;
        Debug.Log("ShopManager initialized successfully");
    }

    public void OnPlayerDataLoaded()
    {
        Debug.Log("=== Player data loaded, refreshing shop ===");
        LoadPlayerCoins();
    }

    private void LoadPlayerCoins()
    {
        if (PlayerManager.Instance != null)
        {
            Player player = PlayerManager.Instance.GetPlayerData();
            if (player != null)
            {
                playerCoins = player.coins;
                Debug.Log($"Loaded player coins: {playerCoins}");
            }
            else
            {
                playerCoins = PlayerManager.Instance.GetCoins();
                Debug.Log($"Loaded default coins: {playerCoins}");
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
            playerCoinsText.text = playerCoins.ToString();
            Debug.Log($"Updated coin display to: {playerCoins}");
        }
        else
        {
            Debug.LogWarning("playerCoinsText is null - UI not found yet");
        }
    }

    public void UpdatePlayerCoinsDisplay(int newCoinAmount)
    {
        playerCoins = newCoinAmount;
        UpdateCoinsDisplay();
    }

    // CRITICAL: This must be called EVERY TIME the shop opens
    public void OpenShop()
    {
        Debug.Log("=== Opening Shop ===");

        if (!isInitialized)
        {
            Initialize();
        }

        // Find all UI references in the ACTIVE scene
        FindUIReferences();

        // Load fresh coin data
        LoadPlayerCoins();

        // Display items
        DisplayShopItems();
    }

    // NEW METHOD: Find UI elements in the active scene
    private void FindUIReferences()
    {
        Debug.Log("Finding UI references in scene...");

        // Find ShopCanvas in the scene
        GameObject shopCanvas = GameObject.Find("ShopCanvas");
        if (shopCanvas == null)
        {
            Debug.LogError("ShopCanvas not found in scene! Make sure it's active.");
            return;
        }

        // Find ShopItemContent
        Transform shop = shopCanvas.transform.Find("Shop");
        if (shop != null)
        {
            Transform viewport = shop.Find("Viewport");
            if (viewport != null)
            {
                shopItemContent = viewport.Find("ShopItemContent");
                if (shopItemContent != null)
                {
                    Debug.Log("Found ShopItemContent");
                }
            }
        }

        // Find PlayerCoinsText - adjust this path to match YOUR hierarchy
        // Common locations:
        TextMeshProUGUI[] allTexts = shopCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in allTexts)
        {
            // Look for text component that should display coins
            if (text.name.Contains("Coin") || text.name.Contains("coin"))
            {
                playerCoinsText = text;
                Debug.Log($"Found coin text: {text.name}");
                break;
            }
        }

        if (playerCoinsText == null)
        {
            Debug.LogWarning("Could not auto-find coin text. Manually find it:");
            // Manual search - adjust the path to YOUR scene structure
            Transform coinsTransform = shopCanvas.transform.Find("CoinPanel/CoinsText"); // Example path
            if (coinsTransform != null)
            {
                playerCoinsText = coinsTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        // Find feedback panels
        Transform successPanel = shopCanvas.transform.Find("PurchaseSuccessPanel");
        if (successPanel != null) purchaseSuccessPanel = successPanel.gameObject;

        Transform noCoinsPanel = shopCanvas.transform.Find("NotEnoughCoinsPanel");
        if (noCoinsPanel != null) notEnoughCoinsPanel = noCoinsPanel.gameObject;

        Debug.Log($"UI References Found - Content: {shopItemContent != null}, Coins: {playerCoinsText != null}");
    }

    public void DisplayShopItems()
    {
        Debug.Log("=== DisplayShopItems called ===");

        if (shopItemContent == null)
        {
            Debug.LogError("Shop Item Content is NULL! Call FindUIReferences() first.");
            return;
        }

        if (shopItemPrefab == null)
        {
            Debug.LogError("Shop Item Prefab is NULL!");
            return;
        }

        if (availableShopItems.Count == 0)
        {
            Debug.LogWarning("No shop items available!");
            return;
        }

        Debug.Log($"Displaying {availableShopItems.Count} shop items");

        // Clear existing items
        foreach (Transform child in shopItemContent)
        {
            Destroy(child.gameObject);
        }

        // Display all shop items
        for (int i = 0; i < availableShopItems.Count; i++)
        {
            ShopItem shopItem = availableShopItems[i];

            if (shopItem == null)
            {
                Debug.LogWarning($"Shop item at index {i} is NULL!");
                continue;
            }

            // Instantiate
            GameObject itemObj = Instantiate(shopItemPrefab);
            itemObj.transform.SetParent(shopItemContent, false);

            // Setup UI
            SetupShopItemUI(itemObj, shopItem);
        }

        Debug.Log("Shop items displayed successfully");
    }

    private void SetupShopItemUI(GameObject itemObj, ShopItem shopItem)
    {
        TextMeshProUGUI itemName = itemObj.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI itemPrice = itemObj.transform.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
        Image itemIcon = itemObj.transform.Find("ItemIcon")?.GetComponent<Image>();
        Button buyButton = itemObj.transform.Find("BuyButton")?.GetComponent<Button>();

        if (itemName != null) itemName.text = shopItem.shopItemName;
        if (itemPrice != null) itemPrice.text = $"{shopItem.price} Coins";
        if (itemIcon != null && shopItem.shopItemIcon != null) itemIcon.sprite = shopItem.shopItemIcon;

        if (buyButton != null)
        {
            ShopItem currentItem = shopItem;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => PurchaseItem(currentItem));
            buyButton.interactable = (playerCoins >= shopItem.price);
        }
    }

    public void PurchaseItem(ShopItem shopItem)
    {
        Debug.Log($"=== Purchase attempt: {shopItem.shopItemName} ===");
        Debug.Log($"Current coins: {playerCoins}, Price: {shopItem.price}");

        if (shopItem == null)
        {
            Debug.LogError("ShopItem is null!");
            return;
        }

        if (playerCoins < shopItem.price)
        {
            Debug.Log("Not enough coins!");
            ShowNotEnoughCoinsPanel();
            return;
        }

        // Deduct coins
        playerCoins -= shopItem.price;
        Debug.Log($"Coins after purchase: {playerCoins}");

        // Update PlayerManager
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.SetCoins(playerCoins);
            Debug.Log("PlayerManager coins updated");
        }

        // Update UI immediately
        UpdateCoinsDisplay();

        // Add to inventory
        AddItemToInventory(shopItem);

        // Save to Firebase
        SavePurchaseToFirebase();

        // Show success
        ShowPurchaseSuccessPanel();

        // Refresh shop
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

        InventoryItem existingItem = InvenManager.instance.invenItemList.Find(
            item => item.invenId == shopItem.correspondingInventoryItem.invenId
        );

        if (existingItem != null)
        {
            existingItem.invenQuantity += 1;
            Debug.Log($"Increased quantity to {existingItem.invenQuantity}");
        }
        else
        {
            InventoryItem runtimeItem = ScriptableObject.CreateInstance<InventoryItem>();
            runtimeItem.invenId = shopItem.correspondingInventoryItem.invenId;
            runtimeItem.invenItemName = shopItem.correspondingInventoryItem.invenItemName;
            runtimeItem.invenIcon = shopItem.correspondingInventoryItem.invenIcon;
            runtimeItem.itemTier = shopItem.correspondingInventoryItem.itemTier;
            runtimeItem.powerUpType = shopItem.correspondingInventoryItem.powerUpType;
            runtimeItem.invenQuantity = 1;

            InvenManager.instance.invenItemList.Add(runtimeItem);
            Debug.Log($"Added new item: {runtimeItem.invenItemName}");
        }

        InvenManager.instance.SaveInventoryToFirebase();
    }

    private void SavePurchaseToFirebase()
    {
        if (FirebaseManager.Instance != null)
        {
            Debug.Log($"Saving coins to Firebase: {playerCoins}");
            FirebaseManager.Instance.UpdatePlayerField("coins", playerCoins,
                onSuccess: () => {
                    Debug.Log($"✓ Coins saved to Firebase: {playerCoins}");
                },
                onError: (error) => {
                    Debug.LogError($"✗ Failed to save coins: {error}");
                }
            );
        }
    }

    private void ShowPurchaseSuccessPanel()
    {
        if (purchaseSuccessPanel != null)
        {
            purchaseSuccessPanel.SetActive(true);
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